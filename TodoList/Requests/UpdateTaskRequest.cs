namespace TodoList.Requests;

public record UpdateTaskRequest(string Title, string Description, TodoTaskStatus Status, DateTime? CompleteBefore);