namespace TodoList.Dtos;

public record TaskListItemDto(
    int Id,
    string Title,
    TodoTaskStatus Status = TodoTaskStatus.ToDo,
    DateTime? CompleteBefore = null
);
