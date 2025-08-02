using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Shared.Extensions;

public static class SwaggerServiceCollectionExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, OpenApiInfo info)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(info.Version, info);

            var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
            var xmlFilename = $"{assemblyName}.xml";
            var path = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(path))
                options.IncludeXmlComments(path);
        });

        return services;
    }
}