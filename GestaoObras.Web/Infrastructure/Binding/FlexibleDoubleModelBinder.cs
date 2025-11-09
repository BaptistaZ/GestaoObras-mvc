using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace GestaoObras.Web.Infrastructure.Binding
{
    // Binder que aceita ponto ou vírgula para doubles
    public sealed class FlexibleDoubleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext ctx)
        {
            var raw = ctx.ValueProvider.GetValue(ctx.ModelName).FirstValue;
            if (string.IsNullOrWhiteSpace(raw)) return Task.CompletedTask;

            var s = raw.Replace(',', '.'); // normaliza
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                ctx.Result = ModelBindingResult.Success(d);
            else
                ctx.ModelState.TryAddModelError(ctx.ModelName, $"Valor inválido para {ctx.ModelMetadata.DisplayName ?? ctx.ModelName}.");

            return Task.CompletedTask;
        }
    }

    public sealed class FlexibleDoubleModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
            => context.Metadata.ModelType == typeof(double) ? new FlexibleDoubleModelBinder() : null;
    }
}