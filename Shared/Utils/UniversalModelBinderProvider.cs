using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Reflection;

namespace Shared.Utils;
public class AttributeBasedModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        // Применяем ко всем типам, у которых хотя бы одно свойство помечено FromRoute, FromQuery, FromBody
        var props = context.Metadata.ModelType.GetProperties();
        var hasBindingAttributes = props.Any(p =>
            p.GetCustomAttribute<FromRouteAttribute>() != null ||
            p.GetCustomAttribute<FromQueryAttribute>() != null ||
            p.GetCustomAttribute<FromBodyAttribute>() != null);

        if (hasBindingAttributes)
            return new BinderTypeModelBinder(typeof(UniversalModelBinder));

        return null;
    }
}

