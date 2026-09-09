namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the rule set for one request property. Split across several partial files by rule family,
/// so the fluent surface stays one type while each family's methods sit together.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed partial class RuleBuilder<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// Requires the value to be a string. Only text and an absent value pass, so unlike most rules here an empty collection or a
    /// zero-length upload fails rather than counting as empty.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> String()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.String));

        return this;
    }

    /// <summary>
    /// Requires the value to be an enumerable value that is not a string.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Array()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.Array));

        return this;
    }

    /// <summary>
    /// Requires the value to be an array or list-like value.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> List()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.List));

        return this;
    }

    /// <summary>
    /// Requires the value to be a boolean, or text that parses as one. An absent value, blank text and an empty collection pass, but unlike
    /// most rules here a zero-length upload fails rather than counting as empty.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Boolean()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.Boolean));

        return this;
    }

    /// <summary>
    /// Requires the value to be a member <typeparamref name="TProperty"/> defines, so the validated property's own type is what supplies
    /// the member set.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Enum()
    {
        _propertyRule.AddRule(new EnumValidationRule<TRequest, TProperty>(typeof(TProperty)));

        return this;
    }

    /// <summary>
    /// Requires the value to be a member <typeparamref name="TEnum"/> defines, rather than any value its underlying type can hold.
    /// </summary>
    ///
    /// <typeparam name="TEnum">The enum type to validate against.</typeparam>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Enum<TEnum>() where TEnum : struct, Enum
    {
        _propertyRule.AddRule(new EnumValidationRule<TRequest, TProperty>(typeof(TEnum)));

        return this;
    }

    /// <summary>
    /// Requires the value to be a member the provided enum type defines, rather than any value its underlying type can hold.
    /// </summary>
    ///
    /// <param name="enumType">
    /// The enum whose defined values the input must be one of, named at runtime rather than as a type argument.
    /// </param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Enum(Type enumType)
    {
        _propertyRule.AddRule(new EnumValidationRule<TRequest, TProperty>(enumType));

        return this;
    }
}
