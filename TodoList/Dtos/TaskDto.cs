namespace TodoList.Dtos;

public record TaskDto(
    int Id,
    string Title,
    string Description = "",
    TodoTaskStatus Status = TodoTaskStatus.ToDo,
    DateTime CreatedAt = default,
    DateTime? CompleteBefore = null
);
