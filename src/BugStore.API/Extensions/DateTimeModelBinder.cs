using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugStore.API.Extensions;

public class DateTimeModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        
        if (value == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

        var stringValue = value.FirstValue;

        if (string.IsNullOrEmpty(stringValue))
        {
            return Task.CompletedTask;
        }

        // Tentar diferentes formatos de data
        var formats = new[]
        {
            "dd/MM/yyyy",
            "dd-MM-yyyy",
            "yyyy-MM-dd",
            "MM/dd/yyyy",
            "MM-dd-yyyy"
        };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(stringValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                // Garantir que o DateTime seja UTC para PostgreSQL
                var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                bindingContext.Result = ModelBindingResult.Success(utcDate);
                return Task.CompletedTask;
            }
        }

        // Se não conseguiu parsear com nenhum formato, tentar o parser padrão
        if (DateTime.TryParse(stringValue, out var defaultDate))
        {
            var utcDate = DateTime.SpecifyKind(defaultDate, DateTimeKind.Utc);
            bindingContext.Result = ModelBindingResult.Success(utcDate);
            return Task.CompletedTask;
        }

        bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid date format");
        bindingContext.Result = ModelBindingResult.Failed();
        
        return Task.CompletedTask;
    }
}