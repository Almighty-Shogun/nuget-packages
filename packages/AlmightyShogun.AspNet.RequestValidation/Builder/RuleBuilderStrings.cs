namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the rule set for one request property. Split across several partial files by rule family, so the fluent surface stays one type
/// while each family's methods sit together.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed partial class RuleBuilder<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// Requires the value to contain only letters.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Alpha()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Alpha));

        return this;
    }

    /// <summary>
    /// Requires the value to contain only letters and numbers.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> AlphaNumeric()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.AlphaNumeric));

        return this;
    }

    /// <summary>
    /// Requires the value to contain only letters, numbers, dashes, and underscores. Use it for slugs, handles, and similar
    /// identifier-style text.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> AlphaDash()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.AlphaDash));

        return this;
    }

    /// <summary>
    /// Requires the value to contain only single-byte ASCII characters.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Ascii()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Ascii));

        return this;
    }

    /// <summary>
    /// Requires the text value to already be lowercase, which text with nothing cased in it does not satisfy.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Lowercase()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Lowercase));

        return this;
    }

    /// <summary>
    /// Requires the text value to already be uppercase, which text with nothing cased in it does not satisfy.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Uppercase()
    {
        _propertyRule.AddRule(new StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode.Uppercase));

        return this;
    }

    /// <summary>
    /// Requires the text value to start with one of the provided prefixes.
    /// </summary>
    ///
    /// <param name="prefixes">The allowed prefixes.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="prefixes"/> is empty, which would leave the rule with nothing to compare against. Thrown as the rule is built rather
    /// than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> StartsWith(params string[] prefixes)
    {
        _propertyRule.AddRule(new StringMatchValidationRule<TRequest, TProperty>(StringMatchMode.StartWith, prefixes));

        return this;
    }

    /// <summary>
    /// Requires the text value to end with one of the provided suffixes.
    /// </summary>
    ///
    /// <param name="suffixes">The allowed suffixes.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="suffixes"/> is empty, which would leave the rule with nothing to compare against. Thrown as the rule is built rather
    /// than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> EndsWith(params string[] suffixes)
    {
        _propertyRule.AddRule(new StringMatchValidationRule<TRequest, TProperty>(StringMatchMode.EndWith, suffixes));

        return this;
    }

    /// <summary>
    /// Requires the text value not to start with any of the provided prefixes.
    /// </summary>
    ///
    /// <param name="prefixes">The forbidden prefixes.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="prefixes"/> is empty, which would leave the rule with nothing to compare against. Thrown as the rule is built rather
    /// than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> DoesNotStartWith(params string[] prefixes)
    {
        _propertyRule.AddRule(new DoesNotValidationRule<TRequest, TProperty>(StringMatchMode.StartWith, prefixes));

        return this;
    }

    /// <summary>
    /// Requires the text value not to end with any of the provided suffixes.
    /// </summary>
    ///
    /// <param name="suffixes">The forbidden suffixes.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="suffixes"/> is empty, which would leave the rule with nothing to compare against. Thrown as the rule is built rather
    /// than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> DoesNotEndWith(params string[] suffixes)
    {
        _propertyRule.AddRule(new DoesNotValidationRule<TRequest, TProperty>(StringMatchMode.EndWith, suffixes));

        return this;
    }

    /// <summary>
    /// Requires the text value or collection to contain one of the provided values.
    /// </summary>
    ///
    /// <param name="requiredValues">The required contained values.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="requiredValues"/> is empty, which would leave the rule with nothing to compare against. Thrown as the rule is
    /// built rather than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Contains(params string[] requiredValues)
    {
        _propertyRule.AddRule(new StringMatchValidationRule<TRequest, TProperty>(StringMatchMode.Contain, requiredValues));

        return this;
    }

    /// <summary>
    /// Requires the text value or collection not to contain any of the provided values.
    /// </summary>
    ///
    /// <param name="forbiddenValues">The forbidden contained values.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="forbiddenValues"/> is empty, which would leave the rule with nothing to compare against. Thrown as the rule is
    /// built rather than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> DoesNotContain(params string[] forbiddenValues)
    {
        _propertyRule.AddRule(new DoesNotValidationRule<TRequest, TProperty>(StringMatchMode.Contain, forbiddenValues));

        return this;
    }
}
