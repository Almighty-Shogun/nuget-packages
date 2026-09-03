namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be inside, or outside, a fixed set of values.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class SetMembershipValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty>
    where TRequest : class
{
    /// <summary>
    /// The permitted or forbidden values, held as declared so the failure message can list them.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlyList<TProperty?> _values;

    /// <summary>
    /// Whether membership is what passes or what fails, which is the difference between the in and not-in spellings.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly bool _shouldContain;

    /// <summary>
    /// Builds the rule, refusing an empty set of values rather than accepting one the rule could not act on.
    /// </summary>
    ///
    /// <param name="values">The values compared against, of which there must be at least one.</param>
    /// <param name="shouldContain">Whether the value must be one of them, or must not be.</param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="values"/> is empty, which would reject every value under the in spelling and accept every value under the not-in
    /// spelling.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public SetMembershipValidationRule(IReadOnlyList<TProperty?> values, bool shouldContain)
    {
        _shouldContain = shouldContain;
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

        bool isValid = _values.Contains(value) == _shouldContain;

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(_shouldContain ? "validation.in" : "validation.not.in", field));
    }
}
