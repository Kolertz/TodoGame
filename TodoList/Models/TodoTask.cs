namespace TodoList.Models;

/// <summary>
/// Таска
/// </summary>
[AdaptTwoWays(nameof(TaskDto))]
[AdaptTo(nameof(TaskListItemDto))]
[GenerateMapper]
public class TodoTask
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = "";
    public TodoTaskStatus Status { get; set; } = TodoTaskStatus.ToDo;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompleteBefore { get; set; } = null;
    public Guid UserId { get; set; }
}
