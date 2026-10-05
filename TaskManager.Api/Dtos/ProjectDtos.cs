using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Dtos
{
    public record CreateProjectRequest(
        [Required, MaxLength(100)] string Name,
        [MaxLength(1000)] string? Description);

    public record UpdateProjectRequest(
        [MaxLength(100)] string? Name,
        [MaxLength(1000)] string? Description);

    public record ProjectResponse(
        int Id,
        string Name,
        string? Description,
        int OwnerId,
        DateTime CreatedAt);
}
