using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Email уникален: два пользователя с одним email невозможны
            b.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Первичный ключ связующей таблицы состоит из двух колонок:
            // одна и та же пара (проект, пользователь) не может повториться
            b.Entity<ProjectMember>().HasKey(m => new { m.ProjectId, m.UserId });

            // Запрещаем каскадное удаление пользователя вместе с его проектами и задачами
            b.Entity<Project>()
                .HasOne(p => p.Owner).WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<TaskItem>()
                .HasOne(t => t.Creator).WithMany()
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Если пользователя-исполнителя удалят, задача останется, а исполнитель станет null
            b.Entity<TaskItem>()
                .HasOne(t => t.Assignee).WithMany()
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Enum храним в базе строкой ("InProgress"), а не числом (1): так читабельнее
            b.Entity<TaskItem>().Property(t => t.Status).HasConversion<string>();
            b.Entity<ProjectMember>().Property(m => m.Role).HasConversion<string>();

            // Soft delete: запросы автоматически пропускают записи, у которых DeletedAt не null
            b.Entity<Project>().HasQueryFilter(p => p.DeletedAt == null);
            b.Entity<TaskItem>().HasQueryFilter(t => t.DeletedAt == null);

            // Индекс ускорит самый частый запрос: задачи проекта с фильтром по статусу
            b.Entity<TaskItem>().HasIndex(t => new { t.ProjectId, t.Status });
        }
    }
}
