using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Controllers
{
    public class ProjectAccesService(AppDbContext db)
    {
        public Task<Project?> FindForUser(int projectId, int userId) =>
            db.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p =>
                    p.Id == projectId && p.Members.Any(m => m.UserId == userId));
        public bool IsOwner(Project project, int userId) =>
            project.Members.Any(m => m.UserId == userId && m.Role == ProkectRole.Owner);
    }
}
