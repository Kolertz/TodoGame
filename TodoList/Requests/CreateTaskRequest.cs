namespace TodoList.Requests;

public record CreateTaskRequest(string Title, string Description, TodoTaskStatus Status, DateTime? CompleteBefore);
