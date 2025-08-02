using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using System.Text.Json;

namespace Shared.Utils;

public class UniversalModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var modelType = bindingContext.ModelType;
        var modelInstance = Activator.CreateInstance(modelType)!;
        var httpContext = bindingContext.HttpContext;
        var request = httpContext.Request;

        // Чтение тела, если есть хотя бы одно свойство с [FromBody]
        string? bodyContent = null;
        Dictionary<string, JsonElement>? bodyJson = null;

        bool hasFromBody = modelType.GetProperties()
            .Any(p => p.GetCustomAttribute<FromBodyAttribute>() != null);

        if (hasFromBody && request.ContentLength > 0)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            bodyContent = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            if (!string.IsNullOrEmpty(bodyContent))
            {
                try
                {
                    bodyJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(bodyContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch { /* Игнорировать ошибки, допустим частичное тело */ }
            }
        }

        foreach (var prop in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            object? value = null;

            // [FromRoute]
            if (prop.GetCustomAttribute<FromRouteAttribute>() != null)
            {
                if (httpContext.Request.RouteValues.TryGetValue(prop.Name.ToLower(), out var routeVal))
                {
                    value = ConvertToType(routeVal?.ToString(), prop.PropertyType);
                }
            }
            // [FromQuery]
            else if (prop.GetCustomAttribute<FromQueryAttribute>() != null)
            {
                if (request.Query.TryGetValue(prop.Name, out var queryVal))
                {
                    value = ConvertToType(queryVal.ToString(), prop.PropertyType);
                }
            }
            // [FromBody]
            else if (prop.GetCustomAttribute<FromBodyAttribute>() != null)
            {
                if (bodyJson != null && bodyJson.TryGetValue(prop.Name, out var jsonElement))
                {
                    value = jsonElement.Deserialize(prop.PropertyType, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else if (bodyContent != null && bodyJson == null)
                {
                    // Если всё тело — это объект, попробуем десериализовать напрямую
                    try
                    {
                        value = JsonSerializer.Deserialize(bodyContent, prop.PropertyType, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    catch { }
                }
            }

            if (value != null)
            {
                prop.SetValue(modelInstance, value);
            }
        }

        bindingContext.Result = ModelBindingResult.Success(modelInstance);
    }

    private object? ConvertToType(string? value, Type targetType)
    {
        if (value == null)
            return null;

        try
        {
            if (targetType == typeof(string))
                return value;
            if (targetType.IsEnum)
                return Enum.Parse(targetType, value, true);
            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            return null;
        }
    }
}

