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
    [Route("projects")]
    [Authorize]
    public class ProjectsController(AppDbContext db) : ControllerBase
    {
        private int CurrentUserId => User.GetUserId();
        [HttpPost]
        public async Task<ActionResult<ProjectResponse>> Create(CreateProjectRequest req)
        {

            var project = new Project
            {
                Name = req.Name.Trim(),
                Description = req.Description?.Trim(),
                OwnerId = CurrentUserId
            };

            project.Members.Add(new ProjectMember
            {
                UserId = CurrentUserId,
                Role = ProkectRole.Owner
            });

            db.Projects.Add(project);

            await db.SaveChangesAsync();

            var response = new ProjectResponse(
                project.Id, project.Name, project.Description,
                project.OwnerId, project.CreateAt);

            return CreatedAtAction(nameof(Get), new { id = project.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectResponse>>> List()
        {
            return await db.Projects
                .Where(p => p.Members.Any(m => m.UserId == CurrentUserId))
                .OrderBy(p => p.Id)
                .Select(p => new ProjectResponse(
                    p.Id, p.Name, p.Description, p.OwnerId, p.CreateAt))
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectResponse>> Get(int id)
        {
            var project = await db.Projects
                .Where(p => p.Id == id && p.Members.Any(m => m.UserId == CurrentUserId))
                .Select(p => new ProjectResponse(
                    p.Id, p.Name, p.Description, p.OwnerId, p.CreateAt))
                .FirstOrDefaultAsync();

            return project is null ? NotFound() : project;
        }

        private Task<Project?> FindMyProject(int id) =>
    db.Projects
        .Include(p => p.Members)
        .FirstOrDefaultAsync(p => p.Id == id && p.Members.Any(m => m.UserId == CurrentUserId));

        private bool IsOwner(Project project) =>
    project.Members.Any(m => m.UserId == CurrentUserId && m.Role == ProkectRole.Owner);

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<ProjectResponse>> Update(int id, UpdateProjectRequest req)
        {
            var project = await FindMyProject(id);
            if (project is null)
            {
                return NotFound();
            }

            if (!IsOwner(project)) return Forbid();

            if (req.Name is not null)
                project.Name = req.Name.Trim();

            if (req.Description is not null)
                project.Description = req.Description.Trim();

            await db.SaveChangesAsync();

            var response = new ProjectResponse(
                project.Id, project.Name, project.Description,
                project.OwnerId, project.CreateAt);

            return response;
        }

        [HttpDelete("{id:int}")]

        public async Task<IActionResult> Delete(int id)
        {
            var project = await FindMyProject(id);
            if (project is null)
                return NotFound();
            if (!IsOwner(project)) 
                return Forbid();
            
            project.DeletedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
