namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates timezone constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class TimezoneValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationValue.TryGetText(value, out string text))
            return ValueTask.FromResult(ValidationRuleResult.Failure("validation.timezone"));

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(text);
            return ValueTask.FromResult(ValidationRuleResult.Success());
        }
        catch (TimeZoneNotFoundException)
        {
            return ValueTask.FromResult(ValidationRuleResult.Failure("validation.timezone"));
        }
        catch (InvalidTimeZoneException)
        {
            return ValueTask.FromResult(ValidationRuleResult.Failure("validation.timezone"));
        }
    }
}
