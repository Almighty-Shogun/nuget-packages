using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Wraps an application's own rule so the pipeline can run it like any built-in one. The rule is taken from the container on each
/// invocation and activated from it when it is not registered there, so registration is optional and the rule may still take dependencies.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal class CustomValidationRuleAdapter<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// The rule type to resolve per invocation, held as a <see cref="Type"/> because the generic-attribute spelling only knows it
    /// at runtime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Type _ruleType;

    /// <summary>
    /// Adapts a rule type known only at runtime, which is the case for an attribute that names its rule as a <see cref="Type"/> .
    /// </summary>
    ///
    /// <param name="ruleType">The custom validation rule type.</param>
    ///
    /// <exception cref="InvalidOperationException">
    /// <paramref name="ruleType"/> does not implement <see cref="ICustomValidationRule{TRequest, TProperty}"/> for this request and
    /// property type. Checked as the rule is built rather than when it first runs, which is on the first request of the type declaring
    /// it, since that is when its rules are built.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public CustomValidationRuleAdapter(Type ruleType)
    {
        if (!typeof(ICustomValidationRule<TRequest, TProperty>).IsAssignableFrom(ruleType))
            throw new InvalidOperationException(
                $"The custom validation rule '{ruleType.Name}' does not implement "
                + $"ICustomValidationRule<{typeof(TRequest).Name}, {typeof(TProperty).Name}>."
            );

        _ruleType = ruleType;
    }

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

        return await ((ICustomValidationRule<TRequest, TProperty>)rule).ValidateAsync(request, value, cancellationToken);
    }
}

/// <summary>
/// Adapts a custom rule whose type is a compile-time argument, which is what the fluent builder produces.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class CustomValidationRuleAdapter<TRequest, TProperty, TRule>
    : CustomValidationRuleAdapter<TRequest, TProperty> where TRequest : class where TRule : class
{
    /// <summary>
    /// Adapts a rule type known at compile time, which is the fluent spelling. The generic attribute reaches the runtime-type constructor
    /// instead, passing its own type argument through as a <see cref="Type"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public CustomValidationRuleAdapter() : base(typeof(TRule)) { }
}
