using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Wraps a validation failure in an MVC result, for the code paths that return a result rather than writing the response themselves.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class ValidationErrorResult
{
    private const int StatusCode = StatusCodes.Status422UnprocessableEntity;

    /// <summary>
    /// Builds a result reporting one field's failure, for the cases that fail before the rule pipeline is reached.
    /// </summary>
    ///
    /// <param name="messageResolver">The message resolver used to resolve error descriptions.</param>
    /// <param name="field">The validation field.</param>
    /// <param name="key">The message key the failure reports, resolved into a sentence only when the response is written.</param>
    /// <param name="parameters">The values substituted into the message template by position, empty when the message takes none.</param>
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
            Code = StatusCode,
            Error = "validation_error",
            ErrorDescription = messageResolver.Resolve($"http-error.{StatusCode}"),
            Errors = errors.ToErrorDictionary(messageResolver)
        });
    }
}
