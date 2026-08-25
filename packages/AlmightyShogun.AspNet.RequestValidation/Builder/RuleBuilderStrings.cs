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
    /// Requires the value to contain only letters. An absent or empty value passes, so pair it with
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
    public RuleBuilder<TRequest, TProperty> Alpha()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Alpha));

        return this;
    }

    /// <summary>
    /// Requires the value to contain only letters and numbers. An absent or empty value passes, so pair it with
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
    public RuleBuilder<TRequest, TProperty> AlphaNumeric()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.AlphaNumeric));

        return this;
    }

    /// <summary>
    /// Requires the value to contain only letters, numbers, dashes, and underscores. Use it for slugs, handles, and similar
    /// identifier-style text. An absent or empty value passes, so pair it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when
    /// the field is mandatory.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AlphaDash()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.AlphaDash));

        return this;
    }

    /// <summary>
    /// Requires the value to contain only single-byte ASCII characters. An absent or empty value passes, so pair it with
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
    public RuleBuilder<TRequest, TProperty> Ascii()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Ascii));

        return this;
    }

    /// <summary>
    /// Requires the text value to already be lowercase. An absent or empty value passes, so pair it with
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
    public RuleBuilder<TRequest, TProperty> Lowercase()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Lowercase));

        return this;
    }

    /// <summary>
    /// Requires the text value to already be uppercase. An absent or empty value passes, so pair it with
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
    public RuleBuilder<TRequest, TProperty> Uppercase()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Uppercase));

        return this;
    }

    /// <summary>
    /// Requires the text value to start with one of the provided prefixes. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="prefixes">The allowed prefixes.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> StartsWith(params string[] prefixes)
    {
        _propertyRule.AddRule(new StringMatchValidationRule<TRequest, TProperty>(StringMatchMode.StartWith, prefixes));

        return this;
    }

    /// <summary>
    /// Requires the text value to end with one of the provided suffixes. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="suffixes">The allowed suffixes.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> EndsWith(params string[] suffixes)
    {
        _propertyRule.AddRule(new StringMatchValidationRule<TRequest, TProperty>(StringMatchMode.EndWith, suffixes));

        return this;
    }

    /// <summary>
    /// Rejects text that starts with one of the provided prefixes. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="prefixes">The forbidden prefixes.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DoesNotStartWith(params string[] prefixes)
    {
        _propertyRule.AddRule(new DoesNotValidationRule<TRequest, TProperty>(StringMatchMode.StartWith, prefixes));

        return this;
    }

    /// <summary>
    /// Rejects text that ends with one of the provided suffixes. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="suffixes">The forbidden suffixes.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DoesNotEndWith(params string[] suffixes)
    {
        _propertyRule.AddRule(new DoesNotValidationRule<TRequest, TProperty>(StringMatchMode.EndWith, suffixes));

        return this;
    }

    /// <summary>
    /// Requires the text value or collection to contain one of the provided values. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="requiredValues">The required contained values.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Contains(params string[] requiredValues)
    {
        _propertyRule.AddRule(new StringMatchValidationRule<TRequest, TProperty>(StringMatchMode.Contain, requiredValues));

        return this;
    }

    /// <summary>
    /// Rejects text or collections containing one of the provided values. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="forbiddenValues">The forbidden contained values.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DoesNotContain(params string[] forbiddenValues)
    {
        _propertyRule.AddRule(new DoesNotValidationRule<TRequest, TProperty>(StringMatchMode.Contain, forbiddenValues));

        return this;
    }
}
