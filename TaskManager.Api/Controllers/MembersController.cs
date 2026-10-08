using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Dtos;
using TaskManager.Api.Entities;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("projects/{projectId:int}/members")]
    [Authorize]
    public class MembersController(AppDbContext db, ProjectAccesService access) : ControllerBase
    {
        private int CurrentUserId => User.GetUserId();
        [HttpGet]
        public async Task<ActionResult<List<MemberResponse>>> List(int projectId)
        {
            var project = await access.FindForUser(projectId, CurrentUserId);
            if (project is null)
                return NotFound();

            return await db.ProjectMembers
                .Where(m => m.ProjectId == projectId)
                .OrderBy(m => m.JoinedAt)
                .Select(m => new MemberResponse(
                    m.UserId, m.User.Email, m.User.Name, m.Role.ToString(), m.JoinedAt))
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<MemberResponse>> Add(int projectId, AddMemberRequest req)
        {
            var project = await access.FindForUser(projectId, CurrentUserId);
            if (project is null)
                return NotFound();

            if (!access.IsOwner(project, CurrentUserId))
                return Forbid();

            var email = req.Email.Trim().ToLowerInvariant();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
                return NotFound(new { error = "Пользователь с таким email не найден" });
            if (project.Members.Any(m => m.UserId == user.Id))
                return Conflict(new { error = "Пользователь уже в проекте" });

            var member = new ProjectMember
            {
                ProjectId = projectId,
                UserId = user.Id,
                Role = ProkectRole.Member
            };
            db.ProjectMembers.Add(member);
            await db.SaveChangesAsync();
            return StatusCode(201, new MemberResponse(
                user.Id, user.Email, user.Name, member.Role.ToString(), member.JoinedAt));
        }

        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> Remove(int projectId, int userId)
        {
            var project = await access.FindForUser(projectId, CurrentUserId);
            if (project is null)
                return NotFound();
            var isSelf = userId == CurrentUserId;
            if (!isSelf && !access.IsOwner(project, CurrentUserId))
                return Forbid();
            var member = project.Members.FirstOrDefault(m => m.UserId == userId);
            if (member is null)
                return NotFound();
            if (member.Role == ProkectRole.Owner)
                return Conflict(new { error = "Владельца нельзя удалить из проекта" });
            db.ProjectMembers.Remove(member);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
