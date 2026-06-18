using MediatR;
using Rhythmix.Application.DTOs.Playlist;

namespace Rhythmix.Application.UseCases.Playlist;

public sealed class GetPublicPlaylistsQuery : IRequest<IEnumerable<PlaylistSummaryDto>>
{
}
