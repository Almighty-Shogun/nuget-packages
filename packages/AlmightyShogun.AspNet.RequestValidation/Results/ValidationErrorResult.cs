using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the validation failure result for one field, for the code paths that fail before the rule pipeline is reached and return a
/// result rather than writing the response themselves.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class ValidationErrorResult
{
    /// <summary>
    /// Builds a result reporting one field's failure, in the same envelope and under the same status a broken rule reports.
    /// </summary>
    ///
    /// <param name="messageResolver">The message resolver used to resolve error descriptions.</param>
    /// <param name="field">
    /// The field the failure is reported against. Spell it as the client sees it, since nothing here converts it: this path has no
    /// property to read a serialization name from.
    /// </param>
    /// <param name="key">
    /// The message key the failure reports, resolved into a sentence here rather than deferred until the response is written.
    /// </param>
    /// <param name="parameters">The values substituted into the message template by position, empty when the message takes none.</param>
    ///
    /// <returns>The result carrying the validation body, whose status comes from the body rather than being set separately.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static HttpErrorResult Create(IMessageResolver messageResolver, string field, string key, params object?[] parameters)
        => new(ValidationErrorResponseFactory.Create(messageResolver, field, key, parameters));
}
