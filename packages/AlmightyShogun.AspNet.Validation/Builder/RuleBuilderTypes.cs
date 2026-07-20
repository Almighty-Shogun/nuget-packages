namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Builds validation rules for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed partial class RuleBuilder<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// Adds a rule that validates the property as a string value.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> String()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.String));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as an array-like value.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Array()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.Array));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as a list-like value.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> List()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.List));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as a boolean value.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Boolean()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.Boolean));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property against its enum type.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Enum()
    {
        _propertyRule.AddRule(new EnumValidationRule<TRequest, TProperty>(typeof(TProperty)));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property against the provided enum type.
    /// </summary>
    ///
    /// <typeparam name="TEnum">The enum type to validate against.</typeparam>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Enum<TEnum>() where TEnum : struct, Enum
    {
        _propertyRule.AddRule(new EnumValidationRule<TRequest, TProperty>(typeof(TEnum)));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property against the provided enum type.
    /// </summary>
    ///
    /// <param name="enumType">The enum type to validate against.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Enum(Type enumType)
    {
        _propertyRule.AddRule(new EnumValidationRule<TRequest, TProperty>(enumType));

        return this;
    }
}
