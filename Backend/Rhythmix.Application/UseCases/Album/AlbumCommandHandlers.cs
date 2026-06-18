using MediatR;
using Rhythmix.Application.DTOs.Album;
using Rhythmix.Domain.Interfaces;

namespace Rhythmix.Application.UseCases.Album;

public sealed class CreateAlbumCommandHandler : IRequestHandler<CreateAlbumCommand, AlbumDto>
{
    private readonly IAlbumRepository _albumRepository;

    public CreateAlbumCommandHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<AlbumDto> Handle(CreateAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = new Rhythmix.Domain.Entities.Album
        {
            AlbumId = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            Title = request.Title.Trim(),
            Description = request.Description,
            CoverImageUrl = request.CoverImageUrl,
            ReleaseDate = request.ReleaseDate,
            CreatedAt = DateTime.UtcNow
        };

        await _albumRepository.CreateAsync(album);
        return ToDto(album, 0);
    }

    private static AlbumDto ToDto(Rhythmix.Domain.Entities.Album album, int trackCount) => new()
    {
        AlbumId = album.AlbumId,
        OwnerId = album.OwnerId,
        Title = album.Title,
        Description = album.Description,
        CoverImageUrl = album.CoverImageUrl,
        ReleaseDate = album.ReleaseDate,
        CreatedAt = album.CreatedAt,
        TrackCount = trackCount
    };
}

public sealed class UpdateAlbumCommandHandler : IRequestHandler<UpdateAlbumCommand, AlbumDto?>
{
    private readonly IAlbumRepository _albumRepository;

    public UpdateAlbumCommandHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<AlbumDto?> Handle(UpdateAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await _albumRepository.GetByIdAsync(request.AlbumId);
        if (album is null)
        {
            return null;
        }

        if (album.OwnerId != request.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to update this album.");
        }

        album.Title = request.Title.Trim();
        album.Description = request.Description;
        album.CoverImageUrl = request.CoverImageUrl;
        album.ReleaseDate = request.ReleaseDate;

        await _albumRepository.UpdateAsync(album);
        var trackCount = await _albumRepository.GetTrackCountAsync(album.AlbumId);

        return new AlbumDto
        {
            AlbumId = album.AlbumId,
            OwnerId = album.OwnerId,
            Title = album.Title,
            Description = album.Description,
            CoverImageUrl = album.CoverImageUrl,
            ReleaseDate = album.ReleaseDate,
            CreatedAt = album.CreatedAt,
            TrackCount = trackCount
        };
    }
}

public sealed class DeleteAlbumCommandHandler : IRequestHandler<DeleteAlbumCommand, bool>
{
    private readonly IAlbumRepository _albumRepository;

    public DeleteAlbumCommandHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<bool> Handle(DeleteAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await _albumRepository.GetByIdAsync(request.AlbumId);
        if (album is null)
        {
            return false;
        }

        if (album.OwnerId != request.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this album.");
        }

        await _albumRepository.DeleteAsync(request.AlbumId);
        return true;
    }
}
