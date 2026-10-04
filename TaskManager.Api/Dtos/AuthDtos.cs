using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Dtos
{
    public record RegisterRequest(
        [Required, EmailAddress, MaxLength(255)] string Email,
        [Required, MinLength(8), MaxLength(100)] string Password,
        [Required, MaxLength(100)] string Name);
    public record LoginRequest(
        [Required] string Email,
        [Required] string Password);
    public record TokenResponse(string AccessToken);
    public record UserResponse(int Id, string Email, string Name);
}
