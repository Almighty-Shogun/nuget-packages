using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares a custom validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class CustomRuleAttribute : ValidationRuleAttribute
{
    /// <summary>
    /// Creates a custom rule attribute.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected CustomRuleAttribute() { }

    /// <inheritdoc />
    internal sealed override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new CustomValidationRuleAdapter<TRequest, TProperty>(CreateCustomRule());

    /// <summary>
    /// Creates the custom validation rule type used by this attribute.
    /// </summary>
    ///
    /// <returns>The custom validation rule type.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected abstract Type CreateCustomRule();

    /// <summary>
    /// Returns the custom validation rule type.
    /// </summary>
    ///
    /// <typeparam name="TRule">The custom validation rule type.</typeparam>
    ///
    /// <returns>The custom validation rule type.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected static Type CustomRule<TRule>() where TRule : class => typeof(TRule);
}

/// <summary>
/// Declares a custom validation rule for a request property.
/// </summary>
///
/// <typeparam name="TRule">The custom validation rule type.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class CustomRuleAttribute<TRule> : CustomRuleAttribute where TRule : class
{
    /// <inheritdoc />
    protected override Type CreateCustomRule() => CustomRule<TRule>();
}
