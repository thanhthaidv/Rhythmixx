using MediatR;
using Rhythmix.Application.DTOs.Playlist;
using Rhythmix.Domain.Interfaces;

namespace Rhythmix.Application.UseCases.Playlist.Handlers;

public sealed class GetPublicPlaylistsQueryHandler : IRequestHandler<GetPublicPlaylistsQuery, IEnumerable<PlaylistSummaryDto>>
{
    private readonly IPlaylistRepository _playlistRepository;
    private readonly IPlaylistTrackRepository _playlistTrackRepository;
    private readonly IMediaRepository _mediaRepository;

    public GetPublicPlaylistsQueryHandler(
        IPlaylistRepository playlistRepository,
        IPlaylistTrackRepository playlistTrackRepository,
        IMediaRepository mediaRepository)
    {
        _playlistRepository = playlistRepository;
        _playlistTrackRepository = playlistTrackRepository;
        _mediaRepository = mediaRepository;
    }

    public async Task<IEnumerable<PlaylistSummaryDto>> Handle(GetPublicPlaylistsQuery request, CancellationToken cancellationToken)
    {
        var playlists = await _playlistRepository.GetPublicAsync();
        var result = new List<PlaylistSummaryDto>();

        foreach (var playlist in playlists)
        {
            var tracks = (await _playlistTrackRepository.GetTracksAsync(playlist.Id)).ToList();
            var firstTrack = tracks.FirstOrDefault();
            var firstMedia = firstTrack == null ? null : await _mediaRepository.GetByIdAsync(firstTrack.MediaId);

            result.Add(new PlaylistSummaryDto
            {
                PlaylistId = playlist.Id,
                Name = playlist.Name,
                Description = playlist.Description,
                CoverImageUrl = playlist.CoverImageUrl,
                IsPublic = playlist.IsPublic,
                OwnerId = playlist.OwnerId,
                TrackCount = tracks.Count,
                ThumbnailUrl = playlist.CoverImageUrl ?? firstMedia?.ThumbnailUrl,
                CreatedAt = playlist.CreatedAt
            });
        }

        return result;
    }
}
