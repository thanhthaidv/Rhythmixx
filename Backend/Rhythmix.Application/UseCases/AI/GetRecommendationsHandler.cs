using Dapper;
using MediatR;
using Rhythmix.Application.Common.Interfaces;
using Rhythmix.Domain.Entities;
using Rhythmix.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rhythmix.Application.UseCases.AI;

public class GetRecommendationsHandler : IRequestHandler<GetRecommendationsQuery, List<MediaItem>>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IAnthropicService _anthropicService;

    public GetRecommendationsHandler(
        IDbConnectionFactory connectionFactory,
        IAnthropicService anthropicService)
    {
        _connectionFactory = connectionFactory;
        _anthropicService = anthropicService;
    }

    public async Task<List<MediaItem>> Handle(GetRecommendationsQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 20);

        // 1. Lấy lịch sử nghe gần nhất
        var recentHistory = await GetRecentHistoryAsync(request.UserId, 20);

        // 2. Lấy danh sách yêu thích
        var favorites = await GetFavoritesAsync(request.UserId);

        // 3. Gọi Claude API để gợi ý
        var recommendedSongs = await _anthropicService.GetRecommendationsAsync(
            recentHistory, favorites, limit);

        // 4. Tra cứu trong CSDL
        return await SearchMediaItemsAsync(recommendedSongs);
    }

    private async Task<List<(string Title, string Artist)>> GetRecentHistoryAsync(string userId, int limit)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
            SELECT TOP (@Limit) m.Title, COALESCE(m.Description, 'Unknown Artist') AS Artist
            FROM PlayHistory ph
            INNER JOIN MediaItems m ON ph.MediaId = m.MediaId
            WHERE ph.UserId = @UserId
            ORDER BY ph.PlayedAt DESC";

        var result = await connection.QueryAsync<(string Title, string Artist)>(sql, new { UserId = userId, Limit = limit });
        return result.ToList();
    }

    private async Task<List<(string Title, string Artist)>> GetFavoritesAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
            SELECT m.Title, COALESCE(m.Description, 'Unknown Artist') AS Artist
            FROM Favorites f
            INNER JOIN MediaItems m ON f.MediaId = m.MediaId
            WHERE f.UserId = @UserId
            ORDER BY f.CreatedAt DESC";

        var result = await connection.QueryAsync<(string Title, string Artist)>(sql, new { UserId = userId });
        return result.ToList();
    }

    private async Task<List<MediaItem>> SearchMediaItemsAsync(List<(string Title, string Artist)> songs)
    {
        if (!songs.Any())
            return new List<MediaItem>();

        using var connection = _connectionFactory.CreateConnection();

        // Build OR query for multiple songs
        var conditions = string.Join(" OR ",
            songs.Select((s, i) => $"(Title LIKE @Title{i} OR Description LIKE @Title{i})"));

        if (string.IsNullOrEmpty(conditions))
            return new List<MediaItem>();

        var sql = $@"
            SELECT TOP 10 MediaId, Title, Description, MediaType, Duration,
                   FilePath, ThumbnailUrl, MimeType, FileSize, AlbumId, GenreId,
                   OwnerId, IsPublic, ViewCount, CreatedAt
            FROM MediaItems
            WHERE {conditions}";

        var parameters = new DynamicParameters();
        for (int i = 0; i < songs.Count; i++)
        {
            parameters.Add($"Title{i}", $"%{songs[i].Title}%");
        }

        var result = await connection.QueryAsync<MediaItem>(sql, parameters);
        return result.ToList();
    }
}