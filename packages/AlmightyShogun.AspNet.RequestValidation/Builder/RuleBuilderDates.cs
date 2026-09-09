using System.Linq.Expressions;

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
    /// Requires the value to be a date or date/time value that can be parsed.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Date()
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>());

        return this;
    }

    /// <summary>
    /// Requires the value to be a date string matching the exact configured format.
    /// </summary>
    ///
    /// <param name="format">
    /// The one format the value must match, so a date written any other way fails even when it names a real instant.
    /// </param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentException">
    /// <paramref name="format"/> is empty or whitespace. Thrown as the rule is built rather than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> DateFormat(string format)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(format));

        return this;
    }

    /// <summary>
    /// Requires the value to be after the provided date.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> After(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.After, date));

        return this;
    }

    /// <summary>
    /// Requires the value to be after the provided date.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> After(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return After(dateTimeOffset);
    }

    /// <summary>
    /// Requires the value to be after the date another request field holds.
    /// </summary>
    ///
    /// <typeparam name="TCompare">
    /// The compared field's type. Its value is read as a date rather than cast, so text parses too, and a value that reads as no
    /// date at all fails the rule.
    /// </typeparam>
    /// <param name="compareExpression">Points at the field holding the date to compare against, so the bound varies per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="compareExpression"/> is not a property read directly off the request, such as a nested read or a method call.
    /// Thrown as the rule is built rather than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> After<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(
            DateMode.After,
            ValidationField<TRequest>.From(compareExpression)
        ));

        return this;
    }

    /// <summary>
    /// Requires the value to be after or equal to the provided date.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> AfterOrEqual(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.AfterOrEqual, date));

        return this;
    }

    /// <summary>
    /// Requires the value to be after or equal to the provided date.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> AfterOrEqual(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return AfterOrEqual(dateTimeOffset);
    }

    /// <summary>
    /// Requires the value to be after or equal to the date another request field holds.
    /// </summary>
    ///
    /// <typeparam name="TCompare">
    /// The compared field's type. Its value is read as a date rather than cast, so text parses too, and a value that reads as no
    /// date at all fails the rule.
    /// </typeparam>
    /// <param name="compareExpression">Points at the field holding the date to compare against, so the bound varies per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="compareExpression"/> is not a property read directly off the request, such as a nested read or a method call.
    /// Thrown as the rule is built rather than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> AfterOrEqual<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(
            DateMode.AfterOrEqual,
            ValidationField<TRequest>.From(compareExpression)
        ));

        return this;
    }

    /// <summary>
    /// Requires the value to be before the provided date.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Before(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.Before, date));

        return this;
    }

    /// <summary>
    /// Requires the value to be before the provided date.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Before(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return Before(dateTimeOffset);
    }

    /// <summary>
    /// Requires the value to be before the date another request field holds.
    /// </summary>
    ///
    /// <typeparam name="TCompare">
    /// The compared field's type. Its value is read as a date rather than cast, so text parses too, and a value that reads as no
    /// date at all fails the rule.
    /// </typeparam>
    /// <param name="compareExpression">Points at the field holding the date to compare against, so the bound varies per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="compareExpression"/> is not a property read directly off the request, such as a nested read or a method call.
    /// Thrown as the rule is built rather than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Before<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(
            DateMode.Before,
            ValidationField<TRequest>.From(compareExpression)
        ));

        return this;
    }

    /// <summary>
    /// Requires the value to be before or equal to the provided date.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> BeforeOrEqual(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.BeforeOrEqual, date));

        return this;
    }

    /// <summary>
    /// Requires the value to be before or equal to the provided date.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> BeforeOrEqual(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return BeforeOrEqual(dateTimeOffset);
    }

    /// <summary>
    /// Requires the value to be before or equal to the date another request field holds.
    /// </summary>
    ///
    /// <typeparam name="TCompare">
    /// The compared field's type. Its value is read as a date rather than cast, so text parses too, and a value that reads as no
    /// date at all fails the rule.
    /// </typeparam>
    /// <param name="compareExpression">Points at the field holding the date to compare against, so the bound varies per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="compareExpression"/> is not a property read directly off the request, such as a nested read or a method call.
    /// Thrown as the rule is built rather than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> BeforeOrEqual<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(
            DateMode.BeforeOrEqual,
            ValidationField<TRequest>.From(compareExpression)
        ));

        return this;
    }

    /// <summary>
    /// Requires the value to equal the provided date.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> DateEquals(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.Equals, date));

        return this;
    }

    /// <summary>
    /// Requires the value to equal the provided date.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> DateEquals(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return DateEquals(dateTimeOffset);
    }

    /// <summary>
    /// Requires the value to equal the date another request field holds.
    /// </summary>
    ///
    /// <typeparam name="TCompare">
    /// The compared field's type. Its value is read as a date rather than cast, so text parses too, and a value that reads as no
    /// date at all fails the rule.
    /// </typeparam>
    /// <param name="compareExpression">Points at the field holding the date to compare against, so the bound varies per request.</param>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="compareExpression"/> is not a property read directly off the request, such as a nested read or a method call.
    /// Thrown as the rule is built rather than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> DateEquals<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(
            DateMode.Equals,
            ValidationField<TRequest>.From(compareExpression)
        ));

        return this;
    }

    /// <summary>
    /// Requires the value to be a valid time zone identifier.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RuleBuilder<TRequest, TProperty> Timezone()
    {
        _propertyRule.AddRule(new TimezoneValidationRule<TRequest, TProperty>());

        return this;
    }
}
