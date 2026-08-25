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
    /// Requires the value to be numeric. An absent or empty value passes, so pair it with
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
    public RuleBuilder<TRequest, TProperty> Numeric()
    {
        _propertyRule.AddRule(new NumberValidationRule<TRequest, TProperty>(NumberMode.Numeric));

        return this;
    }

    /// <summary>
    /// Requires the value to be an integer. An absent or empty value passes, so pair it with
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
    public RuleBuilder<TRequest, TProperty> Integer()
    {
        _propertyRule.AddRule(new NumberValidationRule<TRequest, TProperty>(NumberMode.Integer));

        return this;
    }

    /// <summary>
    /// Requires the value to be at least the provided number. For strings, collections, and files, the number is interpreted as length,
    /// count, or kilobytes. An absent or empty value passes, so pair it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when
    /// the field is mandatory.
    /// </summary>
    ///
    /// <param name="min">The minimum size or value.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Min(decimal min)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Min, min));

        return this;
    }

    /// <summary>
    /// Requires the value to be no greater than the provided number. For strings, collections, and files, the number is interpreted as
    /// length, count, or kilobytes. An absent or empty value passes, so pair it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/>
    /// when the field is mandatory.
    /// </summary>
    ///
    /// <param name="max">The maximum size or value.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Max(decimal max)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Max, max));

        return this;
    }

    /// <summary>
    /// Requires the value to be between the inclusive minimum and maximum. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="min">The minimum size or value.</param>
    /// <param name="max">The maximum size or value.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Between(decimal min, decimal max)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Between, min, max));

        return this;
    }

    /// <summary>
    /// Requires the value to match the exact numeric value, string length, collection count, or file size. An absent or empty value passes,
    /// so pair it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="size">The expected size or value.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Size(decimal size)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Size, size));

        return this;
    }

    /// <summary>
    /// Requires the numeric value to have the configured number of decimal places. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="places">The expected decimal place count.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Decimal(int places)
    {
        _propertyRule.AddRule(new NumberValidationRule<TRequest, TProperty>(NumberMode.DecimalPlaces, places));

        return this;
    }

    /// <summary>
    /// Requires the value to contain exactly the provided number of digits. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="digits">How many digits the value must be written with, counting leading zeroes.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Digits(int digits)
    {
        _propertyRule.AddRule(new DigitCountValidationRule<TRequest, TProperty>(DigitMode.Exact, digits));

        return this;
    }

    /// <summary>
    /// Requires the digit count to be between the inclusive minimum and maximum. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="min">The minimum digit count.</param>
    /// <param name="max">The maximum digit count.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DigitsBetween(int min, int max)
    {
        _propertyRule.AddRule(new DigitCountValidationRule<TRequest, TProperty>(DigitMode.Between, min, max));

        return this;
    }

    /// <summary>
    /// Requires the value to contain at least the provided number of digits. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="min">The minimum digit count.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MinDigits(int min)
    {
        _propertyRule.AddRule(new DigitCountValidationRule<TRequest, TProperty>(DigitMode.Min, min));

        return this;
    }

    /// <summary>
    /// Allows the value to contain at most the provided number of digits. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="max">The maximum digit count.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MaxDigits(int max)
    {
        _propertyRule.AddRule(new DigitCountValidationRule<TRequest, TProperty>(DigitMode.Max, max));

        return this;
    }

    /// <summary>
    /// Requires the numeric value to be a multiple of the provided number. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="multipleOf">The value the property must be a multiple of.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MultipleOf(decimal multipleOf)
    {
        _propertyRule.AddRule(new NumberValidationRule<TRequest, TProperty>(NumberMode.MultipleOf, multipleOf));

        return this;
    }

    /// <summary>
    /// Requires the value to be greater than the provided number. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound the value is compared against, widened to a decimal so the comparison does not depend on the declared type.
    /// </param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> GreaterThan(decimal value)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.GreaterThan, value));

        return this;
    }

    /// <summary>
    /// Requires the value to be greater than or equal to the provided number. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound the value is compared against, widened to a decimal so the comparison does not depend on the declared type.
    /// </param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> GreaterThanOrEqual(decimal value)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.GreaterThanOrEqual, value));

        return this;
    }

    /// <summary>
    /// Requires the value to be less than the provided number. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound the value is compared against, widened to a decimal so the comparison does not depend on the declared type.
    /// </param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> LessThan(decimal value)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.LessThan, value));

        return this;
    }

    /// <summary>
    /// Requires the value to be less than or equal to the provided number. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound the value is compared against, widened to a decimal so the comparison does not depend on the declared type.
    /// </param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> LessThanOrEqual(decimal value)
    {
        _propertyRule.AddRule(new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.LessThanOrEqual, value));

        return this;
    }
}
