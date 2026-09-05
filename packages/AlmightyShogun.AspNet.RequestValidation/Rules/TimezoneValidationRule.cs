namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires text to name a time zone the host system resolves, so what passes is decided by the machine the application runs on rather
/// than by a list this package holds. An absent or empty value passes without being checked, so the rule never implies the field is
/// required.
/// </summary>
///
/// <typeparam name="TRequest">The request type the rule is declared on, though only the bound value decides the outcome.</typeparam>
/// <typeparam name="TProperty">The bound property's type; a non-empty value that cannot be read as text fails.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
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
