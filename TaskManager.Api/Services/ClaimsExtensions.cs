using System.Security.Claims;

namespace TaskManager.Api.Services
{
    public static class ClaimsExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue("sub")!);
    }
}
