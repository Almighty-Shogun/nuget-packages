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
    /// Adds a rule that requires the property to be present and non-empty.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Required()
    {
        _propertyRule.AddRule(new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Required));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property when another property matches any configured value.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the condition.</param>
    /// <param name="values">The values that trigger the requirement.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredIf<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression, params TCompare?[] values)
    {
        _propertyRule.AddRule(new ConditionalValidationRule<TRequest, TProperty, TCompare>(ConditionalTargetMode.Required, ConditionMode.If, compareExpression, values));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property unless another property matches any configured value.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the condition.</param>
    /// <param name="values">The values that skip the requirement.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredUnless<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression, params TCompare?[] values)
    {
        _propertyRule.AddRule(new ConditionalValidationRule<TRequest, TProperty, TCompare>(ConditionalTargetMode.Required, ConditionMode.Unless, compareExpression, values));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property when another property is accepted.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the accepted-state condition.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredIfAccepted<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new ConditionalStateValidationRule<TRequest, TProperty, TCompare>(ConditionalStateTargetMode.Required, ConditionalStateMode.Accepted, compareExpression));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property when another property is declined.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the declined-state condition.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredIfDeclined<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new ConditionalStateValidationRule<TRequest, TProperty, TCompare>(ConditionalStateTargetMode.Required, ConditionalStateMode.Declined, compareExpression));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property when any compared property is present.
    /// </summary>
    ///
    /// <param name="compareExpressions">The compared properties.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredWith(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(new MultiFieldPresenceValidationRule<TRequest, TProperty>(MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithAny, compareExpressions));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property when all compared properties are present.
    /// </summary>
    ///
    /// <param name="compareExpressions">The compared properties.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredWithAll(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(new MultiFieldPresenceValidationRule<TRequest, TProperty>(MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithAll, compareExpressions));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property when any compared property is missing.
    /// </summary>
    ///
    /// <param name="compareExpressions">The compared properties.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredWithout(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(new MultiFieldPresenceValidationRule<TRequest, TProperty>(MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithoutAny, compareExpressions));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property when all compared properties are missing.
    /// </summary>
    ///
    /// <param name="compareExpressions">The compared properties.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredWithoutAll(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(new MultiFieldPresenceValidationRule<TRequest, TProperty>(MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithoutAll, compareExpressions));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be present and not empty when supplied.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Filled()
    {
        _propertyRule.AddRule(new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Filled));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be present, while allowing empty values.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Present()
    {
        _propertyRule.AddRule(new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Present));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be present when another property matches any configured value.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the condition.</param>
    /// <param name="values">The values that trigger presence.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PresentIf<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression, params TCompare?[] values)
    {
        _propertyRule.AddRule(new ConditionalValidationRule<TRequest, TProperty, TCompare>(ConditionalTargetMode.Present, ConditionMode.If, compareExpression, values));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be present unless another property matches any configured value.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the condition.</param>
    /// <param name="values">The values that skip presence.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PresentUnless<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression, params TCompare?[] values)
    {
        _propertyRule.AddRule(new ConditionalValidationRule<TRequest, TProperty, TCompare>(ConditionalTargetMode.Present, ConditionMode.Unless, compareExpression, values));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be present when any compared property is present.
    /// </summary>
    ///
    /// <param name="compareExpressions">The compared properties.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PresentWith(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(new MultiFieldPresenceValidationRule<TRequest, TProperty>(MultiFieldPresenceTargetMode.Present, MultiFieldPresenceTriggerMode.WithAny, compareExpressions));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be present when all compared properties are present.
    /// </summary>
    ///
    /// <param name="compareExpressions">The compared properties.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PresentWithAll(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(new MultiFieldPresenceValidationRule<TRequest, TProperty>(MultiFieldPresenceTargetMode.Present, MultiFieldPresenceTriggerMode.WithAll, compareExpressions));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be missing.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Missing()
    {
        _propertyRule.AddRule(new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Missing));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be missing when another property matches any configured value.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the condition.</param>
    /// <param name="values">The values that trigger missing validation.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MissingIf<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression, params TCompare?[] values)
    {
        _propertyRule.AddRule(new ConditionalValidationRule<TRequest, TProperty, TCompare>(ConditionalTargetMode.Missing, ConditionMode.If, compareExpression, values));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be missing unless another property matches any configured value.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the condition.</param>
    /// <param name="values">The values that skip missing validation.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MissingUnless<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression, params TCompare?[] values)
    {
        _propertyRule.AddRule(new ConditionalValidationRule<TRequest, TProperty, TCompare>(ConditionalTargetMode.Missing, ConditionMode.Unless, compareExpression, values));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be missing when any compared property is present.
    /// </summary>
    ///
    /// <param name="compareExpressions">The compared properties.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MissingWith(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(new MultiFieldPresenceValidationRule<TRequest, TProperty>(MultiFieldPresenceTargetMode.Missing, MultiFieldPresenceTriggerMode.WithAny, compareExpressions));

        return this;
    }

    /// <summary>
    /// Adds a rule that requires the property to be missing when all compared properties are present.
    /// </summary>
    ///
    /// <param name="compareExpressions">The compared properties.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MissingWithAll(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(new MultiFieldPresenceValidationRule<TRequest, TProperty>(MultiFieldPresenceTargetMode.Missing, MultiFieldPresenceTriggerMode.WithAll, compareExpressions));

        return this;
    }

    /// <summary>
    /// Adds a rule that prohibits the property from being present.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Prohibited()
    {
        _propertyRule.AddRule(new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Prohibited));

        return this;
    }

    /// <summary>
    /// Adds a rule that prohibits the property when another property matches any configured value.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the condition.</param>
    /// <param name="values">The values that trigger prohibition.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> ProhibitedIf<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression, params TCompare?[] values)
    {
        _propertyRule.AddRule(new ConditionalValidationRule<TRequest, TProperty, TCompare>(ConditionalTargetMode.Prohibited, ConditionMode.If, compareExpression, values));

        return this;
    }

    /// <summary>
    /// Adds a rule that prohibits the property unless another property matches any configured value.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the condition.</param>
    /// <param name="values">The values that skip prohibition.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> ProhibitedUnless<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression, params TCompare?[] values)
    {
        _propertyRule.AddRule(new ConditionalValidationRule<TRequest, TProperty, TCompare>(ConditionalTargetMode.Prohibited, ConditionMode.Unless, compareExpression, values));

        return this;
    }

    /// <summary>
    /// Adds a rule that prohibits the property when another property is accepted.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the accepted-state condition.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> ProhibitedIfAccepted<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new ConditionalStateValidationRule<TRequest, TProperty, TCompare>(ConditionalStateTargetMode.Prohibited, ConditionalStateMode.Accepted, compareExpression));

        return this;
    }

    /// <summary>
    /// Adds a rule that prohibits the property when another property is declined.
    /// </summary>
    ///
    /// <param name="compareExpression">The property used as the declined-state condition.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> ProhibitedIfDeclined<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(new ConditionalStateValidationRule<TRequest, TProperty, TCompare>(ConditionalStateTargetMode.Prohibited, ConditionalStateMode.Declined, compareExpression));

        return this;
    }

    /// <summary>
    /// Adds a rule that prohibits the compared properties when this property is present.
    /// </summary>
    ///
    /// <param name="compareExpressions">The compared properties.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Prohibits(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(new MultiFieldPresenceValidationRule<TRequest, TProperty>(MultiFieldPresenceTargetMode.Prohibits, MultiFieldPresenceTriggerMode.Prohibits, compareExpressions));

        return this;
    }
}
