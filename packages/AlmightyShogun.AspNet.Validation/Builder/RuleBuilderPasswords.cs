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
    /// Adds the selected password requirement rules to the property.
    /// </summary>
    ///
    /// <param name="letters">Whether a letter requirement should be added.</param>
    /// <param name="mixed">Whether a mixed-case requirement should be added.</param>
    /// <param name="numbers">Whether a number requirement should be added.</param>
    /// <param name="symbols">Whether a symbol requirement should be added.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// Adds a rule that validates the password contains at least one letter.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PasswordLetters()
    {
        _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Letters));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the password contains both lowercase and uppercase letters.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PasswordMixed()
    {
        _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Mixed));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the password contains at least one number.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PasswordNumbers()
    {
        _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Numbers));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the password contains at least one symbol.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PasswordSymbols()
    {
        _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Symbols));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates all password security requirements at once.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PasswordSecure()
    {
        _propertyRule.AddRule(new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Secure));

        return this;
    }
}
