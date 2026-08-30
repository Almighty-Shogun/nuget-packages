namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires text to contain, start with, or end with one of the configured values.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class StringMatchValidationRule<TRequest, TProperty>(
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
            return ValueTask.FromResult(mode == StringMatchMode.Contain && CollectionHoldsOneOf(value)
                ? ValidationRuleResult.Success()
                : ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters()));

        bool isValid = mode switch
        {
            StringMatchMode.Contain => values.Any(requiredValue => text.Contains(requiredValue, StringComparison.Ordinal)),
            StringMatchMode.EndWith => values.Any(suffix => text.EndsWith(suffix, StringComparison.Ordinal)),
            StringMatchMode.StartWith => values.Any(prefix => text.StartsWith(prefix, StringComparison.Ordinal)),
            _ => false
        };

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters()));
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
        StringMatchMode.Contain => "validation.contains",
        StringMatchMode.EndWith => "validation.ends-with",
        _ => "validation.starts-with"
    };

    /// <summary>
    /// Maps the configured mode onto the values a message template substitutes, so the bounds a rule was built with appear in the sentence
    /// the client reads.
    /// </summary>
    ///
    /// <returns>The validation message parameters.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private object?[] GetMessageParameters() => [ValidationValue.JoinValues(values)];

    /// <summary>
    /// Reports whether a collection value holds one of the configured values, which is how <c>Contains</c> reads a value that is not text.
    /// </summary>
    ///
    /// <param name="value">The property value, already known not to be a string.</param>
    ///
    /// <returns>
    /// <c>true</c> when the value is a collection and one of its elements equals a configured value; otherwise <c>false</c>.
    /// </returns>
    ///
    /// <remarks>
    /// Membership rather than substring matching: an element counts when it equals a configured value in full, since a collection holds
    /// values rather than text to search within.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool CollectionHoldsOneOf(TProperty? value)
        => ValidationCollection.TryGetValues(value, out IReadOnlyList<object?> elements)
           && values.Any(required => elements.Any(element => string.Equals(element?.ToString(), required, StringComparison.Ordinal)));
}
