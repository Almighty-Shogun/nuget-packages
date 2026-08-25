namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires a keyed value to carry the named keys, either any of them or all of them.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ArrayKeysValidationRule<TRequest, TProperty>(
    ArrayKeyMode mode,
    IReadOnlyList<string> requiredKeys
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

        if (!ValidationCollection.TryGetKeys(value, out IReadOnlySet<string> keys))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey(), ValidationValue.JoinValues(requiredKeys)));

        bool isValid = mode == ArrayKeyMode.AnyRequiredKey ? requiredKeys.Any(keys.Contains) : requiredKeys.All(keys.Contains);

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), ValidationValue.JoinValues(requiredKeys)));
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
    private string GetMessageKey() => mode == ArrayKeyMode.AnyRequiredKey ? "validation.in.array-keys" : "validation.required.array-keys";
}
