using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Gathers failures as rules run. It is per request rather than shared, and a field stops at its first failure, so a client sees one reason
/// per field rather than every rule that field broke.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationBag
{
    private readonly Dictionary<string, List<ValidationError>> _errors = new(StringComparer.OrdinalIgnoreCase);

    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Reports whether a field has already failed, which is what stops the remaining rules for that field from running.
    /// </summary>
    ///
    /// <param name="field">The field name.</param>
    ///
    /// <returns><c>true</c> when the field has errors; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool HasError(string field) => _errors.ContainsKey(field);

    /// <summary>
    /// Records a failure against a field, keyed by the public name the client sent.
    /// </summary>
    ///
    /// <param name="field">The field name.</param>
    /// <param name="key">The message key the failure reports, resolved into a sentence only when the response is written.</param>
    /// <param name="parameters">The values substituted into the message template by position, empty when the message takes none.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public void Add(string field, string key, params object?[] parameters)
    {
        if (!_errors.TryGetValue(field, out List<ValidationError>? fieldErrors))
        {
            fieldErrors = [];

            _errors[field] = fieldErrors;
        }

        fieldErrors.Add(ValidationError.From(key, parameters));
    }

    /// <summary>
    /// Renders the gathered failures as the response shape, resolving each message key into the negotiated language as it goes.
    /// </summary>
    ///
    /// <param name="messageResolver">The message resolver used to resolve error descriptions.</param>
    ///
    /// <returns>The public validation error dictionary.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyDictionary<string, ValidationRuleError> ToErrorDictionary(IMessageResolver messageResolver) => _errors.ToDictionary(
        error => error.Key,
        error => ToRuleError(error.Value[0], messageResolver),
        StringComparer.OrdinalIgnoreCase
    );

    /// <summary>
    /// Converts an internal validation error into the public validation rule error model.
    /// </summary>
    ///
    /// <param name="validationError">The internal validation error.</param>
    /// <param name="messageResolver">The message resolver used to resolve the error description.</param>
    ///
    /// <returns>The public validation rule error model.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static ValidationRuleError ToRuleError(ValidationError validationError, IMessageResolver messageResolver) => new()
    {
        Code = validationError.Code,
        Error = validationError.Error,
        ErrorDescription = messageResolver.Resolve(validationError.Key, validationError.Parameters)
    };
}
