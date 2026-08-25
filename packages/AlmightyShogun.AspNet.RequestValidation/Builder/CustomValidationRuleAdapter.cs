using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Wraps an application's own rule so the pipeline can run it like any built-in one. The rule type is resolved from the container per
/// request, which is what lets a custom rule depend on services a built-in rule never needs.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal class CustomValidationRuleAdapter<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    private readonly Type _ruleType;

    /// <summary>
    /// Adapts a rule type known only at runtime, which is the case for an attribute that names its rule as a <see cref="Type"/> .
    /// </summary>
    ///
    /// <param name="ruleType">The custom validation rule type.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public CustomValidationRuleAdapter(Type ruleType) => _ruleType = ruleType;

    /// <inheritdoc />
    public async ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        object rule = serviceProvider.GetService(_ruleType) ?? ActivatorUtilities.CreateInstance(serviceProvider, _ruleType);

        if (rule is not ICustomValidationRule<TRequest, TProperty> validationRule)
            throw new InvalidOperationException(
                $"The custom validation rule '{_ruleType.Name}' is not valid for '{typeof(TRequest).Name}.{field}'."
            );

        return await validationRule.ValidateAsync(request, value, cancellationToken);
    }
}

/// <summary>
/// Adapts dependency-injected custom validation rules to property validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class CustomValidationRuleAdapter<TRequest, TProperty, TRule> : CustomValidationRuleAdapter<TRequest, TProperty>
    where TRequest : class where TRule : class
{
    /// <summary>
    /// Adapts a rule type known at compile time, which is the case for the fluent and generic-attribute spellings.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public CustomValidationRuleAdapter() : base(typeof(TRule)) { }
}
