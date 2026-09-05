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
    /// Adds several password requirements at once. By default it requires letters, mixed casing, numbers, and symbols; pass <c>false</c>
    /// for a requirement to skip that part.
    /// </summary>
    ///
    /// <param name="letters">Whether a letter requirement should be added.</param>
    /// <param name="mixed">Whether a mixed-case requirement should be added.</param>
    /// <param name="numbers">Whether a number requirement should be added.</param>
    /// <param name="symbols">Whether a symbol requirement should be added.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Password(bool letters = true, bool mixed = true, bool numbers = true, bool symbols = true)
    {
        if (letters)
            _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Letters));

        if (mixed)
            _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Mixed));

        if (numbers)
            _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Numbers));

        if (symbols)
            _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Symbols));

        return this;
    }

    /// <summary>
    /// Requires the password to contain at least one letter.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> PasswordLetters()
    {
        _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Letters));

        return this;
    }

    /// <summary>
    /// Requires the password to contain at least one lowercase and one uppercase letter.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> PasswordMixed()
    {
        _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Mixed));

        return this;
    }

    /// <summary>
    /// Requires the password to contain at least one number.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> PasswordNumbers()
    {
        _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Numbers));

        return this;
    }

    /// <summary>
    /// Requires the password to contain at least one punctuation or symbol character.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> PasswordSymbols()
    {
        _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Symbols));

        return this;
    }

    /// <summary>
    /// Requires the password to contain mixed-case letters, at least one number, and at least one symbol.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> PasswordSecure()
    {
        _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Secure));

        return this;
    }
}
