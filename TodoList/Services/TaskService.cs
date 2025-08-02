namespace TodoList.Services;

internal class TaskService(AppDbContext db, IUserContext userContext)
{
    private readonly AppDbContext _db = db;
    private readonly IUserContext _userContext = userContext;

    internal async Task<List<TaskListItemDto>> GetTasksByUserAsync() => await _db.Tasks
        .Where(a => a.UserId == _userContext.UserId)
        .ProjectToType<TaskListItemDto>()
        .ToListAsync();

    internal async Task<TaskDto?> GetTaskAsync(int id) => (await _db.Tasks.FindAsync(id)).Adapt<TaskDto?>();

    internal async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request) 
    {
        var task = request.Adapt<TodoTask>();
        task.UserId = _userContext.UserId;
        await _db.Tasks.AddAsync(task);
        await _db.SaveChangesAsync();
        return task.Adapt<TaskDto>();
    }

    internal async Task<bool> UpdateTaskAsync(int id, UpdateTaskRequest request)
    {
        var task = request.Adapt<TodoTask>();
        task.Id = id;
        task.UserId = _userContext.UserId;
        _db.Tasks.Update(task);
        try
        {
            var changedCount = await _db.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            return false;
        }

        return true;
    }
}
