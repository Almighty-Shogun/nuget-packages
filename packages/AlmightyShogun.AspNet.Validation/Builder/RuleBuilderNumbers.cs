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
    /// Adds a rule that validates the property as a numeric value.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Numeric()
    {
        _propertyRule.AddRule(new NumberValidationRule<TRequest, TProperty>(NumberMode.Numeric));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as an integer value.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Integer()
    {
        _propertyRule.AddRule(new NumberValidationRule<TRequest, TProperty>(NumberMode.Integer));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property has at least the configured size or value.
    /// </summary>
    ///
    /// <param name="min">The minimum size or value.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Min(decimal min)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Min, min));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property has at most the configured size or value.
    /// </summary>
    ///
    /// <param name="max">The maximum size or value.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Max(decimal max)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Max, max));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property size or value falls between the configured bounds.
    /// </summary>
    ///
    /// <param name="min">The minimum size or value.</param>
    /// <param name="max">The maximum size or value.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Between(decimal min, decimal max)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Between, min, max));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property has the exact configured size or value.
    /// </summary>
    ///
    /// <param name="size">The expected size or value.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Size(decimal size)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Size, size));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property has the configured number of decimal places.
    /// </summary>
    ///
    /// <param name="places">The expected decimal place count.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Decimal(int places)
    {
        _propertyRule.AddRule(new NumberValidationRule<TRequest, TProperty>(NumberMode.DecimalPlaces, places));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property has the exact configured digit count.
    /// </summary>
    ///
    /// <param name="digits">The expected digit count.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Digits(int digits)
    {
        _propertyRule.AddRule(new DigitCountValidationRule<TRequest, TProperty>(DigitMode.Exact, digits));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property digit count falls between the configured bounds.
    /// </summary>
    ///
    /// <param name="min">The minimum digit count.</param>
    /// <param name="max">The maximum digit count.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DigitsBetween(int min, int max)
    {
        _propertyRule.AddRule(new DigitCountValidationRule<TRequest, TProperty>(DigitMode.Between, min, max));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property has at least the configured digit count.
    /// </summary>
    ///
    /// <param name="min">The minimum digit count.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MinDigits(int min)
    {
        _propertyRule.AddRule(new DigitCountValidationRule<TRequest, TProperty>(DigitMode.Min, min));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property has at most the configured digit count.
    /// </summary>
    ///
    /// <param name="max">The maximum digit count.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MaxDigits(int max)
    {
        _propertyRule.AddRule(new DigitCountValidationRule<TRequest, TProperty>(DigitMode.Max, max));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is a multiple of the configured value.
    /// </summary>
    ///
    /// <param name="multipleOf">The value the property must be a multiple of.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MultipleOf(decimal multipleOf)
    {
        _propertyRule.AddRule(new NumberValidationRule<TRequest, TProperty>(NumberMode.MultipleOf, multipleOf));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is greater than the configured value.
    /// </summary>
    ///
    /// <param name="value">The comparison value.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> GreaterThan(decimal value)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.GreaterThan, value));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is greater than or equal to the configured value.
    /// </summary>
    ///
    /// <param name="value">The comparison value.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> GreaterThanOrEqual(decimal value)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.GreaterThanOrEqual, value));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is less than the configured value.
    /// </summary>
    ///
    /// <param name="value">The comparison value.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> LessThan(decimal value)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.LessThan, value));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is less than or equal to the configured value.
    /// </summary>
    ///
    /// <param name="value">The comparison value.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> LessThanOrEqual(decimal value)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.LessThanOrEqual, value));

        return this;
    }
}
