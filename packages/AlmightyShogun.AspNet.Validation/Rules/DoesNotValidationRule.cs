namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates does not constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class DoesNotValidationRule<TRequest, TProperty>(StringMatchMode mode, IReadOnlyList<string> values) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationValue.TryGetText(value, out string text))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey(), ValidationValue.JoinValues(values)));

        bool isValid = mode switch
        {
            StringMatchMode.Contain => values.All(forbiddenValue => !text.Contains(forbiddenValue, StringComparison.Ordinal)),
            StringMatchMode.EndWith => values.All(suffix => !text.EndsWith(suffix, StringComparison.Ordinal)),
            StringMatchMode.StartWith => values.All(prefix => !text.StartsWith(prefix, StringComparison.Ordinal)),
            _ => false
        };

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), ValidationValue.JoinValues(values)));
    }

    /// <summary>
    /// Gets the validation message key for the configured inverse string matching mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => mode switch
    {
        StringMatchMode.Contain => "validation.does-not.contain",
        StringMatchMode.EndWith => "validation.does-not.end-with",
        _ => "validation.does-not.start-with"
    };
}
