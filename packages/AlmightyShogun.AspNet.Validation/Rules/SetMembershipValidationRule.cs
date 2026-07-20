namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates set membership constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class SetMembershipValidationRule<TRequest, TProperty>(IReadOnlyList<TProperty?> values, bool shouldContain) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        bool isValid = values.Contains(value) == shouldContain;

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(shouldContain ? "validation.in" : "validation.not.in", field));
    }
}
