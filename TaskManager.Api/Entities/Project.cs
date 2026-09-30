namespace TaskManager.Api.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null!;

        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime DeletedAt { get; set; }

        public List<ProjectMember> Members { get; set; } = new();
        public List<TaskItem> Tasks { get; set; } = new();

    }
}
 