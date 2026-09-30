namespace TaskManager.Api.Entities
{
    public enum TaskItemStatus
    {
        Todo,
        InProgress,
        Done,
        Cancelled
    }
    public class TaskItem
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
        public DateTime? DueDate { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;

        public int? AssigneeId { get; set; }
        public User? Assignee { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}
