namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the rule set for one request property. Split across several partial files by rule family, so the fluent surface stays one type
/// while each family's methods sit together.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed partial class RuleBuilder<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// Requires the value to be a string. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> String()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.String));

        return this;
    }

    /// <summary>
    /// Requires the value to be an enumerable value that is not a string. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Array()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.Array));

        return this;
    }

    /// <summary>
    /// Requires the value to be an array or list-like value. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> List()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.List));

        return this;
    }

    /// <summary>
    /// Requires the value to be a boolean value. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Boolean()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.Boolean));

        return this;
    }

    /// <summary>
    /// Requires the value to be parseable as the provided enum type. When the enum type is omitted, the validator uses the request property
    /// type. An absent or empty value passes, so pair it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is
    /// mandatory.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Enum()
    {
        _propertyRule.AddRule(new EnumValidationRule<TRequest, TProperty>(typeof(TProperty)));

        return this;
    }

    /// <summary>
    /// Requires the value to be parseable as the provided enum type. When the enum type is omitted, the validator uses the request property
    /// type. An absent or empty value passes, so pair it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is
    /// mandatory.
    /// </summary>
    ///
    /// <typeparam name="TEnum">The enum type to validate against.</typeparam>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Enum<TEnum>() where TEnum : struct, Enum
    {
        _propertyRule.AddRule(new EnumValidationRule<TRequest, TProperty>(typeof(TEnum)));

        return this;
    }

    /// <summary>
    /// Requires the value to be parseable as the provided enum type. When the enum type is omitted, the validator uses the request property
    /// type. An absent or empty value passes, so pair it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is
    /// mandatory.
    /// </summary>
    ///
    /// <param name="enumType">
    /// The enum whose defined values the input must be one of, named at runtime rather than as a type argument.
    /// </param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Enum(Type enumType)
    {
        _propertyRule.AddRule(new EnumValidationRule<TRequest, TProperty>(enumType));

        return this;
    }
}
