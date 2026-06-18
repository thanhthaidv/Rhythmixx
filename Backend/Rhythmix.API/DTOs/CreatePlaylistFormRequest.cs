using System.ComponentModel.DataAnnotations;

namespace Rhythmix.API.DTOs;

public sealed class CreatePlaylistFormRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsPublic { get; set; } = true;

    public IFormFile? CoverImage { get; set; }
}
