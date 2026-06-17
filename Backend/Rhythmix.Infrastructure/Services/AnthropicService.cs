using Rhythmix.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rhythmix.Infrastructure.Services;

public class AnthropicService : IAnthropicService
{
    private readonly AnthropicOptions _options;

    public AnthropicService(AnthropicOptions options)
    {
        _options = options;
    }

    public async Task<List<(string Title, string Artist)>> GetRecommendationsAsync(
        List<(string Title, string Artist)> history,
        List<(string Title, string Artist)> favorites,
        int limit)
    {
        if (string.IsNullOrEmpty(_options.ApiKey))
            throw new InvalidOperationException("Anthropic API key not configured");

        // Build prompt
        var historyList = history.Any()
            ? string.Join(", ", history.Select(h => $"\"{h.Title}\" by {h.Artist}"))
            : "None";
        var favList = favorites.Any()
            ? string.Join(", ", favorites.Select(f => $"\"{f.Title}\" by {f.Artist}"))
            : "None";

        var prompt = $@"You are a music recommendation system. Based on the user's listening history and favorites:

Listening history: {historyList}
Favorites: {favList}

Suggest {limit} songs that the user might enjoy. Consider:
- Similar genres or styles
- Artists the user already listens to
- Songs that match the user's taste patterns

Respond ONLY with a JSON array of objects with 'title' and 'artist' fields. Example: [{{""title"": ""Song Name"", ""artist"": ""Artist Name""}}]";

        // Call Claude API
        using var client = new HttpClient();
        var requestBody = new
        {
            model = _options.Model,
            max_tokens = 1024,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages")
        {
            Content = JsonContent.Create(requestBody)
        };
        request.Headers.Add("x-api-key", _options.ApiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadFromJsonAsync<ClaudeResponse>();

        // Parse JSON từ Claude response
        var suggestedSongs = new List<(string Title, string Artist)>();
        if (responseJson?.content != null && responseJson.content.Length > 0)
        {
            var contentText = responseJson.content[0].text;
            suggestedSongs = ParseJsonResponse(contentText);
        }

        return suggestedSongs;
    }

    private List<(string Title, string Artist)> ParseJsonResponse(string jsonText)
    {
        var result = new List<(string Title, string Artist)>();

        try
        {
            // Tìm JSON array trong response
            var startIndex = jsonText.IndexOf('[');
            var endIndex = jsonText.LastIndexOf(']');

            if (startIndex >= 0 && endIndex > startIndex)
            {
                var jsonArray = jsonText.Substring(startIndex, endIndex - startIndex + 1);
                var songs = JsonSerializer.Deserialize<List<SongSuggestion>>(jsonArray, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (songs != null)
                {
                    foreach (var song in songs)
                    {
                        if (!string.IsNullOrEmpty(song.Title))
                            result.Add((song.Title, song.Artist ?? "Unknown"));
                    }
                }
            }
        }
        catch
        {
            // If JSON parsing fails, try simple line parsing
            var lines = jsonText.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains("title", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = line.Split(':');
                    if (parts.Length >= 2)
                    {
                        var title = parts[1].Trim().Trim('"', ',');
                        if (!string.IsNullOrEmpty(title))
                            result.Add((title, "Unknown"));
                    }
                }
            }
        }

        return result;
    }

    // DTOs for Claude API
    private class ClaudeResponse
    {
        [JsonPropertyName("content")]
        public ClaudeContent[]? content { get; set; }
    }

    private class ClaudeContent
    {
        [JsonPropertyName("text")]
        public string text { get; set; } = string.Empty;
    }

    private class SongSuggestion
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("artist")]
        public string Artist { get; set; } = string.Empty;
    }
}