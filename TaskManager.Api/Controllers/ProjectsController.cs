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
    }
}
