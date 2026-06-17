using System.ComponentModel.DataAnnotations;

namespace Rhythmix.Application.DTOs.Auth;

public sealed class RegisterRequest
{
    [Required]
    [RegularExpression(@"^[^@\s]+@gmail\.com$", ErrorMessage = "Email must be a gmail.com address.")]
    public string Email { get; set; } = string.Empty;


    [Required]
    [MinLength(4)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be at least 8 characters and contain at least one uppercase letter and one number.")]

    public string Password { get; set; } = string.Empty;



    public string? DisplayName { get; set; }

    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
}
