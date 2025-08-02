using Microsoft.AspNetCore.Http.HttpResults;


var builder = WebApplication.CreateSlimBuilder(args);

var services = builder.Services;

services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

services.Configure<ApiBehaviorOptions>(options => options.SuppressInferBindingSourcesForParameters = true);

var assembly = Assembly.GetExecutingAssembly();
var config = TypeAdapterConfig.GlobalSettings;
config.Scan(assembly);

services.AddCustomSwagger(new()
{
    Version = "v1",
    Title = assembly.GetName().Name,
    Description = "An ASP.NET Core Web API"
});

services.Configure<RouteOptions>(options => options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
services.AddHttpContextAccessor();
services.AddScoped<TaskService>();
services.AddScoped<IUserContext, UserContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var tasksApi = app.MapGroup("/tasks");

// Получить задачи пользователя
tasksApi.MapGet("/", async (TaskService taskService) =>
{
    var tasks = await taskService.GetTasksByUserAsync();
    return Results.Ok(tasks);
})
.Produces<List<TaskListItemDto>>(StatusCodes.Status200OK);

// Создать задачу
tasksApi.MapPost("/", async (CreateTaskRequest request, TaskService taskService) =>
{
    var result = await taskService.CreateTaskAsync(request);
    return Results.Created($"/tasks/{result.Id}", result);
})
.Produces<TaskDto>(StatusCodes.Status201Created);

// Обновить задачу
tasksApi.MapPut("/{id:int}", async (int id, UpdateTaskRequest request, TaskService taskService) =>
{
    var isSuccess = await taskService.UpdateTaskAsync(id, request);
    return isSuccess ? Results.NoContent() : Results.NotFound();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

// Получить задачу по id
tasksApi.MapGet("/{id:int}", async (int id, TaskService taskService) =>
{
    var task = await taskService.GetTaskAsync(id);
    return task == null ? Results.NotFound() : Results.Ok(task);
})
.Produces<TaskDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.Run();

public partial class Program { } // Делает приложение доступным для тестов

[JsonSerializable(typeof(TodoTask))]
[JsonSerializable(typeof(TaskDto))]
[JsonSerializable(typeof(TaskListItemDto))]
[JsonSerializable(typeof(CreateTaskRequest))]
[JsonSerializable(typeof(UpdateTaskRequest))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
