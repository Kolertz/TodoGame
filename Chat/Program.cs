var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.Run();
