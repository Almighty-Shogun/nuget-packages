namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Rejects text that contains, starts with, or ends with any of the configured values, sharing its mode set with the positive rule.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class DoesNotValidationRule<TRequest, TProperty>(
    StringMatchMode mode,
    IReadOnlyList<string> values
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationValue.TryGetText(value, out string text))
            return ValueTask.FromResult(mode == StringMatchMode.Contain && CollectionHoldsNoneOf(value)
                ? ValidationRuleResult.Success()
                : ValidationRuleResult.Failure(GetMessageKey(), ValidationValue.JoinValues(values)));

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
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool CollectionHoldsNoneOf(TProperty? value)
        => ValidationCollection.TryGetValues(value, out IReadOnlyList<object?> elements)
           && values.All(forbidden => elements.All(element => !string.Equals(element?.ToString(), forbidden, StringComparison.Ordinal)));

    private string GetMessageKey() => mode switch
    {
        StringMatchMode.Contain => "validation.does-not.contain",
        StringMatchMode.EndWith => "validation.does-not.end-with",
        _ => "validation.does-not.start-with"
    };
}
