namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Decides whether a field must exist, hold a value, or be absent. These run before value rules so a missing field reports that rather than
/// a later failure.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class PresenceValidationRule<TRequest, TProperty>(
    PresenceMode mode
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValidationRulePriority Priority => ValidationRulePriority.Required;

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        bool isValid = mode switch
        {
            PresenceMode.Missing => value is null,
            PresenceMode.Present => ValidationValue.IsPresent(value),
            PresenceMode.Prohibited => ValidationValue.IsEmpty(value),
            PresenceMode.Required => !ValidationValue.IsEmpty(value),
            PresenceMode.Filled => value is null || !ValidationValue.IsEmpty(value),
            _ => throw new InvalidOperationException($"Unsupported PresenceMode value '{mode}'.")
        };

        return ValueTask.FromResult(isValid ? ValidationRuleResult.Success() : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
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
        PresenceMode.Prohibited => "validation.prohibited",
        _ => throw new InvalidOperationException($"Unsupported PresenceMode value '{mode}'.")
    };
}
