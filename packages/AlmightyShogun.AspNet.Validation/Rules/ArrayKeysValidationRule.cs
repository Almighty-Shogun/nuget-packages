namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates array keys constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ArrayKeysValidationRule<TRequest, TProperty>(ArrayKeyMode mode, IReadOnlyList<string> requiredKeys) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationCollection.TryGetKeys(value, out IReadOnlySet<string> keys))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey(), ValidationValue.JoinValues(requiredKeys)));

        bool isValid = mode == ArrayKeyMode.AnyRequiredKey
            ? requiredKeys.Any(keys.Contains)
            : requiredKeys.All(keys.Contains);

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), ValidationValue.JoinValues(requiredKeys)));
    }

    /// <summary>
    /// Gets the validation message key for the configured array key mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey()
        => mode == ArrayKeyMode.AnyRequiredKey ? "validation.in.array-keys" : "validation.required.array-keys";
}
