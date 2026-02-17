using FluentValidation.Results;

namespace GanaPay.API.Extensions;

public static class ValidationExtensions
{
    public static object ToErrorResponse(this ValidationResult validationResult)
    {
        return new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "One or more validation errors occurred.",
            status = 400,
            errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                )
        };
    }
}