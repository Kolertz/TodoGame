
//namespace ApiGateway;

using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddAuthentication()
    .AddJwtBearer("Keycloak", options =>
    {
        options.Authority = "http://localhost:8080/realms/Default";
        options.Audience = "ocelot-gateway";
        options.RequireHttpsMetadata = false; // Только для разработки
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8080/realms/Default",
            NameClaimType = "preferred_username",
            RoleClaimType = "roles"
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireClaim("roles", "app-admin"));

// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseOcelot().Wait();
app.Run();
