namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires a keyed value to carry the named keys, either any of them or all of them.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ArrayKeysValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// Whether every named key is required or only one of them, which also decides the message a failure reports.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ArrayKeyMode _mode;

    /// <summary>
    /// The keys the value must carry, held as declared so the failure message can list them.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlyList<string> _requiredKeys;

    /// <summary>
    /// Builds the rule, refusing an empty set of values rather than accepting one the rule could not act on.
    /// </summary>
    ///
    /// <param name="mode">Which comparison to perform, which also decides the message a failure reports.</param>
    /// <param name="requiredKeys">The values compared against, of which there must be at least one.</param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="requiredKeys"/> is empty, which would make the rule pass or fail every value regardless of what it holds.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public ArrayKeysValidationRule(ArrayKeyMode mode, IReadOnlyList<string> requiredKeys)
    {
        _mode = mode;
        _requiredKeys = ValidationRuleArguments.RequireAny(requiredKeys, nameof(requiredKeys));
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

        if (!ValidationCollection.TryGetKeys(value, out IReadOnlySet<string> keys))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey(), ValidationDisplay.JoinValues(_requiredKeys)));

        bool isValid = _mode == ArrayKeyMode.AnyRequiredKey ? _requiredKeys.Any(keys.Contains) : _requiredKeys.All(keys.Contains);

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), ValidationDisplay.JoinValues(_requiredKeys)));
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
    private string GetMessageKey() => _mode == ArrayKeyMode.AnyRequiredKey ? "validation.in.array-keys" : "validation.required.array-keys";
}
