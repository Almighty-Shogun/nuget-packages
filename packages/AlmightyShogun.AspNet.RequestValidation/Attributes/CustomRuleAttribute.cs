using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Base for an attribute that names a rule the application implements itself. Deriving from it, rather than using the generic form, is for
/// an attribute that wants a name of its own at the call site.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class CustomRuleAttribute : ValidationRuleAttribute
{
    /// <summary>
    /// Creates the attribute. Protected because only a derived attribute names a rule; there is nothing to construct directly.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    protected CustomRuleAttribute() { }

    /// <inheritdoc />
    internal sealed override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new CustomValidationRuleAdapter<TRequest, TProperty>(CreateCustomRule());

    /// <summary>
    /// Names the rule type to run. A derived attribute implements this rather than being handed the type, so the type is fixed by the
    /// attribute rather than by whoever applies it.
    /// </summary>
    ///
    /// <returns>The custom validation rule type.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    protected abstract Type CreateCustomRule();

    /// <summary>
    /// Hands back a rule type named as a type argument, so a derived attribute writes the rule name once inside its
    /// <see cref="CreateCustomRule"/> override.
    /// </summary>
    ///
    /// <typeparam name="TRule">
    /// The rule to run, taken from the container when it is registered there and activated from it when it is not.
    /// </typeparam>
    ///
    /// <returns>The rule's type, which is all an attribute may carry since it cannot hold an instance.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    protected static Type CustomRule<TRule>() where TRule : class => typeof(TRule);
}

/// <summary>
/// Applies a rule the application implements, named directly as a type argument. Repeatable, so a property may carry several custom rules
/// alongside the built-in ones.
/// </summary>
///
/// <typeparam name="TRule">The custom validation rule type.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class CustomRuleAttribute<TRule> : CustomRuleAttribute where TRule : class
{
    /// <inheritdoc />
    protected override Type CreateCustomRule() => CustomRule<TRule>();
}
