using System.Linq.Expressions;
using System.Text.RegularExpressions;

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
    /// Requires the field to contain an accepted value such as <c>true</c> , <c>yes</c> , <c>on</c> , or <c>1</c> .
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Accepted()
    {
        _propertyRule.AddRule(new AcceptedValidationRule<TRequest, TProperty>());

        return this;
    }

    /// <summary>
    /// Requires the field to contain an accepted value when another field equals one of the provided values. An absent or empty value
    /// passes, so pair it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose value decides whether this rule applies at all.</param>
    /// <param name="values">The values that trigger accepted validation.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AcceptedIf<TCompare>(
        Expression<Func<TRequest, TCompare>> compareExpression,
        params TCompare?[] values
    )
    {
        _propertyRule.AddRule(
            new ConditionalValidationRule<TRequest, TProperty, TCompare>(
                ConditionalTargetMode.Accepted,
                ConditionMode.If,
                compareExpression,
                values
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field to contain a declined value such as <c>false</c> , <c>no</c> , <c>off</c> , or <c>0</c> .
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Declined()
    {
        _propertyRule.AddRule(new DeclinedValidationRule<TRequest, TProperty>());

        return this;
    }

    /// <summary>
    /// Requires the field to contain a declined value when another field equals one of the provided values. An absent or empty value
    /// passes, so pair it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose value decides whether this rule applies at all.</param>
    /// <param name="values">The values that trigger declined validation.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DeclinedIf<TCompare>(
        Expression<Func<TRequest, TCompare>> compareExpression,
        params TCompare?[] values
    )
    {
        _propertyRule.AddRule(
            new ConditionalValidationRule<TRequest, TProperty, TCompare>(
                ConditionalTargetMode.Declined,
                ConditionMode.If,
                compareExpression,
                values
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field value to match another request field.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field this one is compared against, so both are read from the same request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> SameAs<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new FieldComparisonValidationRule<TRequest, TProperty, TCompare>(FieldComparisonMode.Same, compareExpression)
        );

        return this;
    }

    /// <summary>
    /// Requires the field value to be different from another request field.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field this one is compared against, so both are read from the same request.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Different<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new FieldComparisonValidationRule<TRequest, TProperty, TCompare>(FieldComparisonMode.Different, compareExpression)
        );

        return this;
    }

    /// <summary>
    /// Requires the field to match a confirmation field. Without an explicit target, the validator looks for the property name with
    /// <c>Confirmation</c> appended, then for <c>Confirm</c> prefixed to it.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Confirmed()
    {
        _propertyRule.AddRule(new FieldComparisonValidationRule<TRequest, TProperty, TProperty>());

        return this;
    }

    /// <summary>
    /// Requires the field to match a confirmation field. Without an explicit target, the validator looks for the property name with
    /// <c>Confirmation</c> appended, then for <c>Confirm</c> prefixed to it.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the confirmation field, replacing the name this rule would otherwise assume.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Confirmed<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new FieldComparisonValidationRule<TRequest, TProperty, TCompare>(FieldComparisonMode.Confirmed, compareExpression)
        );

        return this;
    }

    /// <summary>
    /// Requires the field value to be inside a set of allowed values. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="values">The allowed values.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> In(params TProperty?[] values)
    {
        _propertyRule.AddRule(new SetMembershipValidationRule<TRequest, TProperty>(values, true));

        return this;
    }

    /// <summary>
    /// Requires the field value to be outside a set of forbidden values. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="values">The forbidden values.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> NotIn(params TProperty?[] values)
    {
        _propertyRule.AddRule(new SetMembershipValidationRule<TRequest, TProperty>(values, false));

        return this;
    }

    /// <summary>
    /// Requires the field value to exist in another array-like request field. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the collection field this value must appear in.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> InArray<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new InArrayValidationRule<TRequest, TProperty>(ValidationField<TRequest>.From(compareExpression)));

        return this;
    }

    /// <summary>
    /// Requires an object or dictionary-like field to contain at least one of the provided keys. An absent or empty value passes, so pair
    /// it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="keys">The required keys.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> InArrayKeys(params string[] keys)
    {
        _propertyRule.AddRule(new ArrayKeysValidationRule<TRequest, TProperty>(ArrayKeyMode.AnyRequiredKey, keys));

        return this;
    }

    /// <summary>
    /// Requires an object or dictionary-like field to contain all provided keys. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="keys">The required keys.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredArrayKeys(params string[] keys)
    {
        _propertyRule.AddRule(new ArrayKeysValidationRule<TRequest, TProperty>(ArrayKeyMode.AllRequiredKeys, keys));

        return this;
    }

    /// <summary>
    /// Requires all values in an array-like field to be unique. An absent or empty value passes, so pair it with
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
    public RuleBuilder<TRequest, TProperty> Distinct()
    {
        _propertyRule.AddRule(new DistinctValidationRule<TRequest, TProperty>());

        return this;
    }

    /// <summary>
    /// Requires the value to match the regular expression pattern. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="options">The options to build with, passed through untouched so the caller decides whether to compile.</param>
    /// <param name="description">An optional description of the expected shape, passed to the message template as <c>{0}</c>.</param>
    /// <param name="matchTimeout">The maximum time a single match may take before it is abandoned and the rule fails.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Regex(
        string pattern,
        RegexOptions options = RegexOptions.None,
        string? description = null,
        TimeSpan? matchTimeout = null
    )
    {
        _propertyRule.AddRule(new RegexValidationRule<TRequest, TProperty>(pattern, options, true, description, matchTimeout));

        return this;
    }

    /// <summary>
    /// Requires the value to not match the regular expression pattern. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="options">The options to build with, passed through untouched so the caller decides whether to compile.</param>
    /// <param name="description">An optional description of the expected shape, passed to the message template as <c>{0}</c>.</param>
    /// <param name="matchTimeout">The maximum time a single match may take before it is abandoned and the rule fails.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> NotRegex(
        string pattern,
        RegexOptions options = RegexOptions.None,
        string? description = null,
        TimeSpan? matchTimeout = null
    )
    {
        _propertyRule.AddRule(new RegexValidationRule<TRequest, TProperty>(pattern, options, false, description, matchTimeout));

        return this;
    }

    /// <summary>
    /// Adds a custom validation rule resolved from the dependency injection container.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> CustomRule<TRule>() where TRule : class, ICustomValidationRule<TRequest, TProperty>
    {
        _propertyRule.AddRule(new CustomValidationRuleAdapter<TRequest, TProperty, TRule>());

        return this;
    }
}
