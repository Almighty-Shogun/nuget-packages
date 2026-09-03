using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the validation error body. Every path that reports a failure comes through here, whether the failure was a broken rule, a
/// binding error, or a payload that never parsed, so one envelope covers all of them and no caller assembles its own.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationErrorResponseFactory
{
    /// <summary>
    /// The status a validation failure is returned with, which is the unprocessable-content code rather than a plain bad request. A
    /// failure that names its own status, such as a body over the size limit, keeps that one instead.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const int StatusCode = StatusCodes.Status422UnprocessableEntity;

    /// <summary>
    /// The machine-readable identifier every validation failure is reported under, shared by the per-field response and the plain error
    /// body written for a request whose payload could not be read at all.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const string ErrorCode = "validation_error";

    /// <summary>
    /// Builds the body for a set of field failures, resolving each message key as it goes.
    /// </summary>
    ///
    /// <param name="messageResolver">The resolver used to produce the description and the per-field messages.</param>
    /// <param name="errors">The failures gathered while the rules ran, one entry per field that failed.</param>
    ///
    /// <returns>The validation error response, whose description names the status rather than any one field.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static ValidationErrorResponse Create(IMessageResolver messageResolver, ValidationBag errors) => new()
    {
        Code = StatusCode,
        Error = ErrorCode,
        ErrorDescription = messageResolver.Resolve($"http-error.{StatusCode}"),
        Errors = errors.ToErrorDictionary(messageResolver)
    };

    /// <summary>
    /// Builds the body for a failure with no per-field detail, such as a body that could not be read.
    /// </summary>
    ///
    /// <param name="messageResolver">The resolver used to produce the description.</param>
    /// <param name="messageKey">The message key describing the failure, resolved into the negotiated language here.</param>
    /// <param name="statusCode">
    /// The status repeated inside the body, defaulting to the validation status for the paths that have no other one to report.
    /// </param>
    ///
    /// <returns>The validation error response.</returns>
    ///
    /// <remarks>
    /// The <c>Errors</c> dictionary is present but empty, so a client reading it finds nothing to report rather than finding the field
    /// missing altogether. That is what lets one envelope cover both a rule failure and a body that never parsed.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static ValidationErrorResponse Create(
        IMessageResolver messageResolver,
        string messageKey,
        int statusCode = StatusCode
    ) => new()
    {
        Code = statusCode,
        Error = ErrorCode,
        ErrorDescription = messageResolver.Resolve(messageKey),
        Errors = new Dictionary<string, ValidationRuleError>()
    };

    /// <summary>
    /// Builds the body for a single field's failure, for the callers that fail one field before the rule pipeline is reached.
    /// </summary>
    ///
    /// <param name="messageResolver">The resolver used to produce the description and the field's message.</param>
    /// <param name="field">The field the failure is reported against, spelled as the client sees it.</param>
    /// <param name="key">The message key the failure reports.</param>
    /// <param name="parameters">The values substituted into the message template by position, empty when the message takes none.</param>
    ///
    /// <returns>The validation error response carrying that one field.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static ValidationErrorResponse Create(
        IMessageResolver messageResolver,
        string field,
        string key,
        object?[] parameters
    )
    {
        ValidationBag errors = new();
        errors.Add(field, key, parameters);

        return Create(messageResolver, errors);
    }
}
