namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires text to name a time zone the host system knows, which differs between Windows and everything else.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class TimezoneValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
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
