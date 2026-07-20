using System.Linq.Expressions;
using System.Text.RegularExpressions;

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
    /// Adds a rule that validates the property as an accepted value.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Accepted()
    {
        _propertyRule.AddRule(new AcceptedValidationRule<TRequest, TProperty>());

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is accepted when another property matches any configured value.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the condition.</param>
    /// <param name="values">The values that trigger accepted validation.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AcceptedIf<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression, params TCompare?[] values)
    {
        _propertyRule.AddRule(new ConditionalValidationRule<TRequest, TProperty, TCompare>(ConditionalTargetMode.Accepted, ConditionMode.If, compareExpression, values));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as a declined value.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Declined()
    {
        _propertyRule.AddRule(new DeclinedValidationRule<TRequest, TProperty>());

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is declined when another property matches any configured value.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the condition.</param>
    /// <param name="values">The values that trigger declined validation.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> DeclinedIf<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression, params TCompare?[] values)
    {
        _propertyRule.AddRule(new ConditionalValidationRule<TRequest, TProperty, TCompare>(ConditionalTargetMode.Declined, ConditionMode.If, compareExpression, values));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property has the same value as another property.
    /// </summary>
    ///
    /// <param name="compareExpression">The property to compare against.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> SameAs<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new FieldComparisonValidationRule<TRequest, TProperty, TCompare>(FieldComparisonMode.Same, compareExpression));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property has a different value than another property.
    /// </summary>
    ///
    /// <param name="compareExpression">The property to compare against.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Different<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new FieldComparisonValidationRule<TRequest, TProperty, TCompare>(FieldComparisonMode.Different, compareExpression));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property against the conventional confirmation property.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Confirmed()
    {
        _propertyRule.AddRule(new FieldComparisonValidationRule<TRequest, TProperty, TProperty>());

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property against a configured confirmation property.
    /// </summary>
    ///
    /// <param name="compareExpression">The confirmation property.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Confirmed<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new FieldComparisonValidationRule<TRequest, TProperty, TCompare>(FieldComparisonMode.Confirmed, compareExpression));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is inside a set of allowed values.
    /// </summary>
    ///
    /// <param name="values">The allowed values.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> In(params TProperty?[] values)
    {
        _propertyRule.AddRule(new SetMembershipValidationRule<TRequest, TProperty>(values, true));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property is outside a set of forbidden values.
    /// </summary>
    ///
    /// <param name="values">The forbidden values.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> NotIn(params TProperty?[] values)
    {
        _propertyRule.AddRule(new SetMembershipValidationRule<TRequest, TProperty>(values, false));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property exists in another array-like property.
    /// </summary>
    ///
    /// <param name="compareExpression">The array-like property to search.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> InArray<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new InArrayValidationRule<TRequest, TProperty>(ValidationField<TRequest>.From(compareExpression)));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates an array-like property contains at least one required key.
    /// </summary>
    ///
    /// <param name="keys">The required keys.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> InArrayKeys(params string[] keys)
    {
        _propertyRule.AddRule(new ArrayKeysValidationRule<TRequest, TProperty>(ArrayKeyMode.AnyRequiredKey, keys));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates an array-like property contains all required keys.
    /// </summary>
    ///
    /// <param name="keys">The required keys.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredArrayKeys(params string[] keys)
    {
        _propertyRule.AddRule(new ArrayKeysValidationRule<TRequest, TProperty>(ArrayKeyMode.AllRequiredKeys, keys));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates all array-like values are distinct.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Distinct()
    {
        _propertyRule.AddRule(new DistinctValidationRule<TRequest, TProperty>());

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property matches a regular expression pattern.
    /// </summary>
    ///
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="options">The regular expression options.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Regex(string pattern, RegexOptions options = RegexOptions.None)
    {
        _propertyRule.AddRule(new RegexValidationRule<TRequest, TProperty>(pattern, options, true));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property does not match a regular expression pattern.
    /// </summary>
    ///
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="options">The regular expression options.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> NotRegex(string pattern, RegexOptions options = RegexOptions.None)
    {
        _propertyRule.AddRule(new RegexValidationRule<TRequest, TProperty>(pattern, options, false));

        return this;
    }

    /// <summary>
    /// Adds a custom validation rule resolved from the dependency injection container.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> CustomRule<TRule>() where TRule : class, ICustomValidationRule<TRequest, TProperty>
    {
        _propertyRule.AddRule(new CustomValidationRuleAdapter<TRequest, TProperty, TRule>());

        return this;
    }
}
