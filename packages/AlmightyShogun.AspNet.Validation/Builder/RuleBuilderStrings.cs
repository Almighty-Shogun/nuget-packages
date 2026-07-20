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
    /// Adds a rule that validates the property contains only letters.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Alpha()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Alpha));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property contains only letters and numbers.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AlphaNumeric()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.AlphaNumeric));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property contains only letters, numbers, dashes, and underscores.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AlphaDash()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.AlphaDash));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property contains only ASCII characters.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Ascii()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Ascii));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property contains only lowercase characters.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Lowercase()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Lowercase));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property contains only uppercase characters.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Uppercase()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Uppercase));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property starts with one of the configured prefixes.
    /// </summary>
    ///
    /// <param name="prefixes">The allowed prefixes.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> StartsWith(params string[] prefixes)
    {
        _propertyRule.AddRule(new StringMatchValidationRule<TRequest, TProperty>(StringMatchMode.StartWith, prefixes));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property ends with one of the configured suffixes.
    /// </summary>
    ///
    /// <param name="suffixes">The allowed suffixes.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> EndsWith(params string[] suffixes)
    {
        _propertyRule.AddRule(new StringMatchValidationRule<TRequest, TProperty>(StringMatchMode.EndWith, suffixes));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property does not start with any configured prefix.
    /// </summary>
    ///
    /// <param name="prefixes">The forbidden prefixes.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DoesNotStartWith(params string[] prefixes)
    {
        _propertyRule.AddRule(new DoesNotValidationRule<TRequest, TProperty>(StringMatchMode.StartWith, prefixes));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property does not end with any configured suffix.
    /// </summary>
    ///
    /// <param name="suffixes">The forbidden suffixes.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DoesNotEndWith(params string[] suffixes)
    {
        _propertyRule.AddRule(new DoesNotValidationRule<TRequest, TProperty>(StringMatchMode.EndWith, suffixes));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property contains one of the required values.
    /// </summary>
    ///
    /// <param name="requiredValues">The required contained values.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Contains(params string[] requiredValues)
    {
        _propertyRule.AddRule(new StringMatchValidationRule<TRequest, TProperty>(StringMatchMode.Contain, requiredValues));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property does not contain any forbidden value.
    /// </summary>
    ///
    /// <param name="forbiddenValues">The forbidden contained values.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DoesNotContain(params string[] forbiddenValues)
    {
        _propertyRule.AddRule(new DoesNotValidationRule<TRequest, TProperty>(StringMatchMode.Contain, forbiddenValues));

        return this;
    }
}
