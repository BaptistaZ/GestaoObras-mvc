using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace GestaoObras.Web.Infrastructure.Binding;

public sealed class FlexibleNumberModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext ctx)
    {
        var raw = ctx.ValueProvider.GetValue(ctx.ModelName).FirstValue;

        if (string.IsNullOrWhiteSpace(raw))
            return Task.CompletedTask;

        // normaliza vírgula -> ponto para aceitar "41,693" e "41.693"
        var norm = raw.Replace(',', '.');

        try
        {
            object? value = null;
            var t = Nullable.GetUnderlyingType(ctx.ModelType) ?? ctx.ModelType;

            if (t == typeof(double))
            {
                if (double.TryParse(norm, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                    value = d;
            }
            else if (t == typeof(decimal))
            {
                if (decimal.TryParse(norm, NumberStyles.Float, CultureInfo.InvariantCulture, out var m))
                    value = m;
            }

            if (value is not null)
            {
                ctx.Result = ModelBindingResult.Success(value);
            }
            else
            {
                ctx.ModelState.TryAddModelError(
                    ctx.ModelName,
                    $"Valor inválido para {ctx.ModelMetadata.DisplayName ?? ctx.ModelName}.");
            }
        }
        catch
        {
            ctx.ModelState.TryAddModelError(
                ctx.ModelName,
                $"Valor inválido para {ctx.ModelMetadata.DisplayName ?? ctx.ModelName}.");
        }

        return Task.CompletedTask;
    }
}

public sealed class FlexibleNumberModelBinderProvider : IModelBinderProvider
{
    private static readonly IModelBinder _binder = new FlexibleNumberModelBinder();

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var t = Nullable.GetUnderlyingType(context.Metadata.ModelType) ?? context.Metadata.ModelType;
        return (t == typeof(double) || t == typeof(decimal)) ? _binder : null;
    }
}