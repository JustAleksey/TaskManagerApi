namespace TaskManager.Api.Entities
{
    public enum ProkectRole
    {
        Owner,
        Member
    }

    public class ProjectMember
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ProkectRole Role { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
    }
}
