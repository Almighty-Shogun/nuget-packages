namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates presence constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class PresenceValidationRule<TRequest, TProperty>(PresenceMode mode) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    public ValidationRulePriority Priority => mode == PresenceMode.Required
        ? ValidationRulePriority.Required : ValidationRulePriority.Normal;

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        bool isValid = mode switch
        {
            PresenceMode.Missing => value is null,
            PresenceMode.Present => ValidationValue.IsPresent(value),
            PresenceMode.Prohibited => ValidationValue.IsEmpty(value),
            PresenceMode.Required or PresenceMode.Filled => !ValidationValue.IsEmpty(value),
            _ => false
        };

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Gets the validation message key for the configured presence mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => mode switch
    {
        PresenceMode.Required => "validation.required",
        PresenceMode.Filled => "validation.filled",
        PresenceMode.Present => "validation.present",
        PresenceMode.Missing => "validation.missing",
        _ => "validation.prohibited"
    };
}
