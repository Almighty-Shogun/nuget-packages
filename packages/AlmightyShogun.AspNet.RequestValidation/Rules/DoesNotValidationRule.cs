namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Rejects text that contains, starts with, or ends with any of the configured values, sharing its mode set with the positive rule.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class DoesNotValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty>
    where TRequest : class
{
    /// <summary>
    /// Which part of the text is forbidden from matching, which also decides the message a failure reports.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly StringMatchMode _mode;

    /// <summary>
    /// The values the text must not match, held as declared so the failure message can list them.
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
    public DoesNotValidationRule(StringMatchMode mode, IReadOnlyList<string> values)
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
            return ValueTask.FromResult(_mode == StringMatchMode.Contain && CollectionHoldsNoneOf(value)
                ? ValidationRuleResult.Success()
                : ValidationRuleResult.Failure(GetMessageKey(), ValidationDisplay.JoinValues(_values)));

        bool isValid = _mode switch
        {
            StringMatchMode.Contain => _values.All(forbiddenValue => !text.Contains(forbiddenValue, StringComparison.Ordinal)),
            StringMatchMode.EndWith => _values.All(suffix => !text.EndsWith(suffix, StringComparison.Ordinal)),
            StringMatchMode.StartWith => _values.All(prefix => !text.StartsWith(prefix, StringComparison.Ordinal)),
            _ => throw new InvalidOperationException($"Unsupported StringMatchMode value '{_mode}'.")
        };

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), ValidationDisplay.JoinValues(_values)));
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
    /// <summary>
    /// Reports whether a collection value holds none of the configured values, which is how <c>DoesNotContain</c> reads a value that is
    /// not text.
    /// </summary>
    ///
    /// <param name="value">The property value, already known not to be a string.</param>
    ///
    /// <returns><c>true</c> when the value is a collection and no element equals a configured value; otherwise <c>false</c>.</returns>
    ///
    /// <remarks>
    /// Membership rather than substring matching, mirroring <c>Contains</c>: an element counts when it equals a configured value in full.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool CollectionHoldsNoneOf(TProperty? value)
        => ValidationCollection.TryGetValues(value, out IReadOnlyList<object?> elements)
           && _values.All(forbidden => elements.All(element => !string.Equals(element?.ToString(), forbidden, StringComparison.Ordinal)));

    /// <summary>
    /// Picks the message key for the spelling this rule was built with, so a rejected value names the check it broke.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => _mode switch
    {
        StringMatchMode.Contain => "validation.does-not.contain",
        StringMatchMode.EndWith => "validation.does-not.end-with",
        StringMatchMode.StartWith => "validation.does-not.start-with",
        _ => throw new InvalidOperationException($"Unsupported StringMatchMode value '{_mode}'.")
    };
}
