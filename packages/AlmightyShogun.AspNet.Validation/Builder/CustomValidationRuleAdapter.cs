using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Adapts a runtime custom validation rule type to a property validation rule.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal class CustomValidationRuleAdapter<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    private readonly Type _ruleType;

    /// <summary>
    /// Creates a custom validation rule adapter for a runtime rule type.
    /// </summary>
    ///
    /// <param name="ruleType">The custom validation rule type.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public CustomValidationRuleAdapter(Type ruleType) => _ruleType = ruleType;

    /// <inheritdoc />
    public async ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        object rule = serviceProvider.GetService(_ruleType) ?? ActivatorUtilities.CreateInstance(serviceProvider, _ruleType);

        if (rule is not ICustomValidationRule<TRequest, TProperty> validationRule)
            throw new InvalidOperationException($"The custom validation rule '{_ruleType.Name}' is not valid for '{typeof(TRequest).Name}.{field}'.");

        return await validationRule.ValidateAsync(request, value, cancellationToken);
    }
}

/// <summary>
/// Adapts dependency-injected custom validation rules to property validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class CustomValidationRuleAdapter<TRequest, TProperty, TRule> : CustomValidationRuleAdapter<TRequest, TProperty> where TRequest : class where TRule : class
{
    /// <summary>
    /// Creates a custom validation rule adapter for a generic rule type.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public CustomValidationRuleAdapter() : base(typeof(TRule)) { }
}
