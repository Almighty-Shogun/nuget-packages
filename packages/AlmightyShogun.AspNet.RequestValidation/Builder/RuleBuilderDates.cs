using System.Linq.Expressions;

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
    /// Requires the value to be a date or date/time value that can be parsed. An absent or empty value passes, so pair it with
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
    public RuleBuilder<TRequest, TProperty> Date()
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>());

        return this;
    }

    /// <summary>
    /// Requires the value to be a date string matching the exact configured format. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="format">
    /// The one format the value must match, so a date written any other way fails even when it names a real instant.
    /// </param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DateFormat(string format)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(format));

        return this;
    }

    /// <summary>
    /// Requires the value to be after a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> After(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.After, date));

        return this;
    }

    /// <summary>
    /// Requires the value to be after a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> After(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return After(dateTimeOffset);
    }

    /// <summary>
    /// Requires the value to be after a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field holding the date to compare against, so the bound varies per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> After<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new DateValidationRule<TRequest, TProperty>(DateMode.After, ValidationField<TRequest>.From(compareExpression))
        );

        return this;
    }

    /// <summary>
    /// Requires the value to be after or equal to a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AfterOrEqual(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.AfterOrEqual, date));

        return this;
    }

    /// <summary>
    /// Requires the value to be after or equal to a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AfterOrEqual(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return AfterOrEqual(dateTimeOffset);
    }

    /// <summary>
    /// Requires the value to be after or equal to a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field holding the date to compare against, so the bound varies per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AfterOrEqual<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new DateValidationRule<TRequest, TProperty>(DateMode.AfterOrEqual, ValidationField<TRequest>.From(compareExpression))
        );

        return this;
    }

    /// <summary>
    /// Requires the value to be before a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Before(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.Before, date));

        return this;
    }

    /// <summary>
    /// Requires the value to be before a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Before(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return Before(dateTimeOffset);
    }

    /// <summary>
    /// Requires the value to be before a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field holding the date to compare against, so the bound varies per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Before<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new DateValidationRule<TRequest, TProperty>(DateMode.Before, ValidationField<TRequest>.From(compareExpression))
        );

        return this;
    }

    /// <summary>
    /// Requires the value to be before or equal to a literal date or another request field. An absent or empty value passes, so pair it
    /// with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> BeforeOrEqual(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.BeforeOrEqual, date));

        return this;
    }

    /// <summary>
    /// Requires the value to be before or equal to a literal date or another request field. An absent or empty value passes, so pair it
    /// with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> BeforeOrEqual(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return BeforeOrEqual(dateTimeOffset);
    }

    /// <summary>
    /// Requires the value to be before or equal to a literal date or another request field. An absent or empty value passes, so pair it
    /// with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field holding the date to compare against, so the bound varies per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> BeforeOrEqual<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new DateValidationRule<TRequest, TProperty>(DateMode.BeforeOrEqual, ValidationField<TRequest>.From(compareExpression))
        );

        return this;
    }

    /// <summary>
    /// Requires the value to equal a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DateEquals(DateTimeOffset date)
    {
        _propertyRule.AddRule(new DateValidationRule<TRequest, TProperty>(DateMode.Equals, date));

        return this;
    }

    /// <summary>
    /// Requires the value to equal a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="date">The fixed date to compare against, decided when the rule was written rather than per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DateEquals(DateTime date)
    {
        ValidationDate.TryGetDate(date, out DateTimeOffset dateTimeOffset);

        return DateEquals(dateTimeOffset);
    }

    /// <summary>
    /// Requires the value to equal a literal date or another request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field holding the date to compare against, so the bound varies per request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DateEquals<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new DateValidationRule<TRequest, TProperty>(DateMode.Equals, ValidationField<TRequest>.From(compareExpression))
        );

        return this;
    }

    /// <summary>
    /// Requires the value to be a valid time zone identifier. An absent or empty value passes, so pair it with
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
    public RuleBuilder<TRequest, TProperty> Timezone()
    {
        _propertyRule.AddRule(new TimezoneValidationRule<TRequest, TProperty>());

        return this;
    }
}
