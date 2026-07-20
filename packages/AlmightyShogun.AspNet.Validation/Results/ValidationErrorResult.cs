using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Creates MVC object results for validation error responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class ValidationErrorResult
{
    private const int _statusCode = StatusCodes.Status422UnprocessableEntity;

    /// <summary>
    /// Creates a validation error response for a single field.
    /// </summary>
    ///
    /// <param name="messageResolver">The message resolver used to resolve error descriptions.</param>
    /// <param name="field">The validation field.</param>
    /// <param name="key">The validation message key.</param>
    /// <param name="parameters">The validation message parameters.</param>
    ///
    /// <returns>The validation error object result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ObjectResult Create(IMessageResolver messageResolver, string field, string key, params object?[] parameters)
    {
        ValidationBag errors = new();
        errors.Add(field, key, parameters);

        return HttpErrorResult.Create(new ValidationErrorResponse
        {
            Code = _statusCode,
            Error = "validation_error",
            ErrorDescription = messageResolver.Resolve($"http-error.{_statusCode}"),
            Errors = errors.ToErrorDictionary(messageResolver)
        });
    }
}
