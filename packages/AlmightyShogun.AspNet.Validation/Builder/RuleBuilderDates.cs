using System.Linq.Expressions;

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
    /// Adds a rule that validates the property as a date value.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Date()
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>());

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as a date matching a specific format.
    /// </summary>
    ///
    /// <param name="format">The expected date format.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DateFormat(string format)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(format));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is after a target date.
    /// </summary>
    ///
    /// <param name="date">The target date.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> After(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.After, date));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is after a target date.
    /// </summary>
    ///
    /// <param name="date">The target date.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> After(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return After(dateTimeOffset);
    }

    /// <summary>
    /// Adds a rule that validates the property is after another date property.
    /// </summary>
    ///
    /// <param name="compareExpression">The target date property.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> After<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.After, ValidationField<TRequest>.From(compareExpression)));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is after or equal to a target date.
    /// </summary>
    ///
    /// <param name="date">The target date.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AfterOrEqual(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.AfterOrEqual, date));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is after or equal to a target date.
    /// </summary>
    ///
    /// <param name="date">The target date.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AfterOrEqual(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return AfterOrEqual(dateTimeOffset);
    }

    /// <summary>
    /// Adds a rule that validates the property is after or equal to another date property.
    /// </summary>
    ///
    /// <param name="compareExpression">The target date property.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AfterOrEqual<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.AfterOrEqual, ValidationField<TRequest>.From(compareExpression)));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is before a target date.
    /// </summary>
    ///
    /// <param name="date">The target date.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Before(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.Before, date));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is before a target date.
    /// </summary>
    ///
    /// <param name="date">The target date.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Before(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return Before(dateTimeOffset);
    }

    /// <summary>
    /// Adds a rule that validates the property is before another date property.
    /// </summary>
    ///
    /// <param name="compareExpression">The target date property.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Before<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.Before, ValidationField<TRequest>.From(compareExpression)));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is before or equal to a target date.
    /// </summary>
    ///
    /// <param name="date">The target date.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> BeforeOrEqual(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.BeforeOrEqual, date));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is before or equal to a target date.
    /// </summary>
    ///
    /// <param name="date">The target date.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> BeforeOrEqual(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return BeforeOrEqual(dateTimeOffset);
    }

    /// <summary>
    /// Adds a rule that validates the property is before or equal to another date property.
    /// </summary>
    ///
    /// <param name="compareExpression">The target date property.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> BeforeOrEqual<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.BeforeOrEqual, ValidationField<TRequest>.From(compareExpression)));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property equals a target date.
    /// </summary>
    ///
    /// <param name="date">The target date.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DateEquals(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.Equals, date));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property equals a target date.
    /// </summary>
    ///
    /// <param name="date">The target date.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DateEquals(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return DateEquals(dateTimeOffset);
    }

    /// <summary>
    /// Adds a rule that validates the property equals another date property.
    /// </summary>
    ///
    /// <param name="compareExpression">The target date property.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DateEquals<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.Equals, ValidationField<TRequest>.From(compareExpression)));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as a known time zone.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Timezone()
    {
        _propertyRule.AddRule(new TimezoneValidationRule<TRequest, TProperty>());

        return this;
    }
}
