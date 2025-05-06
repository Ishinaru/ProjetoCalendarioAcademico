using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace CalendarioAcademico.WebAPI.Helpers
{
    public class DateOnlyModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

            if (DateOnly.TryParseExact(
                value,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
            {
                bindingContext.Result = ModelBindingResult.Success(date);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    "Formato de data inválido. Use dd/MM/yyyy."
                );
            }
            return Task.CompletedTask;
        }
    }
}
