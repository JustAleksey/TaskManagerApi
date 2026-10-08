using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Dtos
{
    public record AddMemberRequest(
        [Required, EmailAddress] string Email);

    public record MemberResponse(
        int UserId,
        string Email,
        string Name,
        string Role,
        DateTime JoinedAt);
}
