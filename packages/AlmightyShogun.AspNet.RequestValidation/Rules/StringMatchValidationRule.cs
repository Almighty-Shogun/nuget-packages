namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires text to contain, start with, or end with one of the configured values.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class StringMatchValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty>
    where TRequest : class
{
    /// <summary>
    /// Which part of the text must match, which also decides the message a failure reports.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly StringMatchMode _mode;

    /// <summary>
    /// The values the text is matched against, held as declared so the failure message can list them.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlyList<string> _values;

    /// <summary>
    /// Builds the rule, refusing an empty set of values rather than accepting one the rule could not act on.
    /// </summary>
    ///
    /// <param name="mode">Which comparison to perform, which also decides the message a failure reports.</param>
    /// <param name="values">The values compared against, of which there must be at least one.</param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="values"/> is empty, which would make the rule pass or fail every value regardless of what it holds.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public StringMatchValidationRule(StringMatchMode mode, IReadOnlyList<string> values)
    {
        _mode = mode;
        _values = ValidationRuleArguments.RequireAny(values, nameof(values));
    }

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
            return ValueTask.FromResult(_mode == StringMatchMode.Contain && CollectionHoldsOneOf(value)
                ? ValidationRuleResult.Success()
                : ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters()));

        bool isValid = _mode switch
        {
            StringMatchMode.Contain => _values.Any(requiredValue => text.Contains(requiredValue, StringComparison.Ordinal)),
            StringMatchMode.EndWith => _values.Any(suffix => text.EndsWith(suffix, StringComparison.Ordinal)),
            StringMatchMode.StartWith => _values.Any(prefix => text.StartsWith(prefix, StringComparison.Ordinal)),
            _ => throw new InvalidOperationException($"Unsupported StringMatchMode value '{_mode}'.")
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
    private string GetMessageKey() => _mode switch
    {
        StringMatchMode.Contain => "validation.contains",
        StringMatchMode.EndWith => "validation.ends-with",
        StringMatchMode.StartWith => "validation.starts-with",
        _ => throw new InvalidOperationException($"Unsupported StringMatchMode value '{_mode}'.")
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
    private object?[] GetMessageParameters() => [ValidationDisplay.JoinValues(_values)];

    /// <summary>
    /// Reports whether a collection value holds one of the configured values, which is how <c>Contains</c> reads a value that is not text.
    /// </summary>
    ///
    /// <param name="value">The property value, already known not to be a string.</param>
    ///
    /// <returns>
    /// <c>true</c> when the value is a collection and one of its elements renders as a configured value in full; otherwise <c>false</c>.
    /// </returns>
    ///
    /// <remarks>
    /// Membership rather than substring matching, since a collection holds values rather than text to search within. The comparison is
    /// between an element's <see cref="object.ToString"/> and the configured text, not between the values themselves, so the number
    /// <c>123</c> matches <c>"123"</c> . An element whose rendering depends on the ambient culture, such as a decimal or a date, is
    /// therefore compared as that culture writes it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool CollectionHoldsOneOf(TProperty? value)
        => ValidationCollection.TryGetValues(value, out IReadOnlyList<object?> elements)
           && _values.Any(required => elements.Any(element => string.Equals(element?.ToString(), required, StringComparison.Ordinal)));
}
