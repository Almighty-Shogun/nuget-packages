namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates string match constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class StringMatchValidationRule<TRequest, TProperty>(StringMatchMode mode, IReadOnlyList<string> values) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationValue.TryGetText(value, out string text))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters()));

        bool isValid = mode switch
        {
            StringMatchMode.Contain => values.All(requiredValue => text.Contains(requiredValue, StringComparison.Ordinal)),
            StringMatchMode.EndWith => values.Any(suffix => text.EndsWith(suffix, StringComparison.Ordinal)),
            StringMatchMode.StartWith => values.Any(prefix => text.StartsWith(prefix, StringComparison.Ordinal)),
            _ => false
        };

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters()));
    }

    /// <summary>
    /// Gets the validation message key for the configured string matching mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => mode switch
    {
        StringMatchMode.Contain => "validation.contains",
        StringMatchMode.EndWith => "validation.ends-with",
        _ => "validation.starts-with"
    };

    /// <summary>
    /// Gets the validation message parameters for the configured string matching mode.
    /// </summary>
    ///
    /// <returns>The validation message parameters.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private object?[] GetMessageParameters()
        => mode == StringMatchMode.Contain ? [] : [ValidationValue.JoinValues(values)];
}
