using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Data;
using TaskManager.Api.Dtos;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize]
    public class UsersController(AppDbContext db) : ControllerBase
    {
        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> Me()
        {
            var id = User.GetUserId();
            var user = await db.Users.FindAsync(id);

            return user is null
                ? NotFound()
                : new UserResponse(user.Id, user.Email, user.Name);
        }
    }

}
