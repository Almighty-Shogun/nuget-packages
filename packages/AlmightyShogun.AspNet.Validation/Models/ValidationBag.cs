using AlmightyShogun.AspNet.Utils;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Collects validation errors while a request is being validated.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationBag
{
    private readonly Dictionary<string, List<ValidationError>> _errors = new(StringComparer.OrdinalIgnoreCase);

    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Checks whether a field already has validation errors.
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
    /// Adds a validation error for a field.
    /// </summary>
    ///
    /// <param name="field">The field name.</param>
    /// <param name="key">The validation message key.</param>
    /// <param name="parameters">The validation message parameters.</param>
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
    /// Converts the error bag into the public validation error dictionary.
    /// </summary>
    ///
    /// <param name="messageResolver">The message resolver used to resolve error descriptions.</param>
    ///
    /// <returns>The public validation error dictionary.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyDictionary<string, ValidationRuleError> ToErrorDictionary(IMessageResolver messageResolver)
        => _errors.ToDictionary(
            error => error.Key,
            error => ToRuleError(error.Value[0], messageResolver),
            StringComparer.OrdinalIgnoreCase);

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
