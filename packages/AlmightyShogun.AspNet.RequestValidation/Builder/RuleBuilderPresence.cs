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
    /// Requires the field to be present and contain a non-empty value. It fails for missing values, <c>null</c> , empty text, empty
    /// collections, and empty uploaded files. Presence rules run before value rules, so a field this rejects reports that rather than a
    /// later format or size failure.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Required()
    {
        _propertyRule.AddRule(new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Required));

        return this;
    }

    /// <summary>
    /// Requires the field when another field equals one of the provided values. Presence rules run before value rules, so a field this
    /// rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose value decides whether this rule applies at all.</param>
    /// <param name="values">The values that trigger the requirement.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredIf<TCompare>(
        Expression<Func<TRequest, TCompare>> compareExpression,
        params TCompare?[] values
    )
    {
        _propertyRule.AddRule(
            new ConditionalValidationRule<TRequest, TProperty, TCompare>(
                ConditionalTargetMode.Required,
                ConditionMode.If,
                compareExpression,
                values
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field unless another field equals one of the provided values. Presence rules run before value rules, so a field this
    /// rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose value decides whether this rule applies at all.</param>
    /// <param name="values">The values that skip the requirement.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredUnless<TCompare>(
        Expression<Func<TRequest, TCompare>> compareExpression,
        params TCompare?[] values
    )
    {
        _propertyRule.AddRule(
            new ConditionalValidationRule<TRequest, TProperty, TCompare>(
                ConditionalTargetMode.Required,
                ConditionMode.Unless,
                compareExpression,
                values
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field when another field contains an accepted value such as <c>true</c> , <c>yes</c> , <c>on</c> , or <c>1</c> .
    /// Presence rules run before value rules, so a field this rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose truthiness decides whether this rule applies.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredIfAccepted<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new ConditionalStateValidationRule<TRequest, TProperty, TCompare>(
                ConditionalStateTargetMode.Required,
                ConditionalStateMode.Accepted,
                compareExpression
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field when another field contains a declined value such as <c>false</c> , <c>no</c> , <c>off</c> , or <c>0</c> .
    /// Presence rules run before value rules, so a field this rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose falseness decides whether this rule applies.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredIfDeclined<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new ConditionalStateValidationRule<TRequest, TProperty, TCompare>(
                ConditionalStateTargetMode.Required,
                ConditionalStateMode.Declined,
                compareExpression
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field when any listed field is present. Presence rules run before value rules, so a field this rejects reports that
    /// rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpressions">Point at the fields watched for presence, all of which the trigger is evaluated against.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredWith(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(
            new MultiFieldPresenceValidationRule<TRequest, TProperty>(
                MultiFieldPresenceTargetMode.Required,
                MultiFieldPresenceTriggerMode.WithAny,
                compareExpressions
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field when all listed fields are present. Presence rules run before value rules, so a field this rejects reports that
    /// rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpressions">Point at the fields watched for presence, all of which the trigger is evaluated against.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredWithAll(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(
            new MultiFieldPresenceValidationRule<TRequest, TProperty>(
                MultiFieldPresenceTargetMode.Required,
                MultiFieldPresenceTriggerMode.WithAll,
                compareExpressions
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field when any listed field is missing. Presence rules run before value rules, so a field this rejects reports that
    /// rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpressions">Point at the fields watched for presence, all of which the trigger is evaluated against.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredWithout(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(
            new MultiFieldPresenceValidationRule<TRequest, TProperty>(
                MultiFieldPresenceTargetMode.Required,
                MultiFieldPresenceTriggerMode.WithoutAny,
                compareExpressions
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field when all listed fields are missing. Presence rules run before value rules, so a field this rejects reports that
    /// rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpressions">Point at the fields watched for presence, all of which the trigger is evaluated against.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> RequiredWithoutAll(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(
            new MultiFieldPresenceValidationRule<TRequest, TProperty>(
                MultiFieldPresenceTargetMode.Required,
                MultiFieldPresenceTriggerMode.WithoutAll,
                compareExpressions
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field to contain a value when the client sends it. Missing values are allowed, but present empty values fail
    /// validation. Presence rules run before value rules, so a field this rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Filled()
    {
        _propertyRule.AddRule(new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Filled));

        return this;
    }

    /// <summary>
    /// Requires the field to exist in the request even when the value is allowed to be empty. Use it when the API must distinguish omitted
    /// fields from explicitly provided empty values. Presence rules run before value rules, so a field this rejects reports that rather
    /// than a later format or size failure.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Present()
    {
        _propertyRule.AddRule(new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Present));

        return this;
    }

    /// <summary>
    /// Requires the field to be present when another field equals one of the provided values. The field may still be empty unless another
    /// rule rejects empty values. Presence rules run before value rules, so a field this rejects reports that rather than a later format or
    /// size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose value decides whether this rule applies at all.</param>
    /// <param name="values">The values that trigger presence.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PresentIf<TCompare>(
        Expression<Func<TRequest, TCompare>> compareExpression,
        params TCompare?[] values
    )
    {
        _propertyRule.AddRule(
            new ConditionalValidationRule<TRequest, TProperty, TCompare>(
                ConditionalTargetMode.Present,
                ConditionMode.If,
                compareExpression,
                values
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field to be present unless another field equals one of the provided values. Presence rules run before value rules, so a
    /// field this rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose value decides whether this rule applies at all.</param>
    /// <param name="values">The values that skip presence.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PresentUnless<TCompare>(
        Expression<Func<TRequest, TCompare>> compareExpression,
        params TCompare?[] values
    )
    {
        _propertyRule.AddRule(
            new ConditionalValidationRule<TRequest, TProperty, TCompare>(
                ConditionalTargetMode.Present,
                ConditionMode.Unless,
                compareExpression,
                values
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field to be present when any listed field is present. Presence rules run before value rules, so a field this rejects
    /// reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpressions">Point at the fields watched for presence, all of which the trigger is evaluated against.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PresentWith(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(
            new MultiFieldPresenceValidationRule<TRequest, TProperty>(
                MultiFieldPresenceTargetMode.Present,
                MultiFieldPresenceTriggerMode.WithAny,
                compareExpressions
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field to be present when all listed fields are present. Presence rules run before value rules, so a field this rejects
    /// reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpressions">Point at the fields watched for presence, all of which the trigger is evaluated against.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> PresentWithAll(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(
            new MultiFieldPresenceValidationRule<TRequest, TProperty>(
                MultiFieldPresenceTargetMode.Present,
                MultiFieldPresenceTriggerMode.WithAll,
                compareExpressions
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field to be absent from the request. Use it for server-controlled values that clients must never send. Presence rules
    /// run before value rules, so a field this rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Missing()
    {
        _propertyRule.AddRule(new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Missing));

        return this;
    }

    /// <summary>
    /// Requires the field to be missing when another field equals one of the provided values. Presence rules run before value rules, so a
    /// field this rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose value decides whether this rule applies at all.</param>
    /// <param name="values">The values that trigger missing validation.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MissingIf<TCompare>(
        Expression<Func<TRequest, TCompare>> compareExpression,
        params TCompare?[] values
    )
    {
        _propertyRule.AddRule(
            new ConditionalValidationRule<TRequest, TProperty, TCompare>(
                ConditionalTargetMode.Missing,
                ConditionMode.If,
                compareExpression,
                values
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field to be missing unless another field equals one of the provided values. Presence rules run before value rules, so a
    /// field this rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose value decides whether this rule applies at all.</param>
    /// <param name="values">The values that skip missing validation.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MissingUnless<TCompare>(
        Expression<Func<TRequest, TCompare>> compareExpression,
        params TCompare?[] values
    )
    {
        _propertyRule.AddRule(
            new ConditionalValidationRule<TRequest, TProperty, TCompare>(
                ConditionalTargetMode.Missing,
                ConditionMode.Unless,
                compareExpression,
                values
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field to be missing when any listed field is present. Presence rules run before value rules, so a field this rejects
    /// reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpressions">Point at the fields watched for presence, all of which the trigger is evaluated against.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MissingWith(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(
            new MultiFieldPresenceValidationRule<TRequest, TProperty>(
                MultiFieldPresenceTargetMode.Missing,
                MultiFieldPresenceTriggerMode.WithAny,
                compareExpressions
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the field to be missing when all listed fields are present. Presence rules run before value rules, so a field this rejects
    /// reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpressions">Point at the fields watched for presence, all of which the trigger is evaluated against.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MissingWithAll(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(
            new MultiFieldPresenceValidationRule<TRequest, TProperty>(
                MultiFieldPresenceTargetMode.Missing,
                MultiFieldPresenceTriggerMode.WithAll,
                compareExpressions
            )
        );

        return this;
    }

    /// <summary>
    /// Rejects the field when the client sends it with a non-empty value. Empty or missing values are allowed. Presence rules run before
    /// value rules, so a field this rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Prohibited()
    {
        _propertyRule.AddRule(new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Prohibited));

        return this;
    }

    /// <summary>
    /// Prohibits the field when another field equals one of the provided values. Presence rules run before value rules, so a field this
    /// rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose value decides whether this rule applies at all.</param>
    /// <param name="values">The values that trigger prohibition.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> ProhibitedIf<TCompare>(
        Expression<Func<TRequest, TCompare>> compareExpression,
        params TCompare?[] values
    )
    {
        _propertyRule.AddRule(
            new ConditionalValidationRule<TRequest, TProperty, TCompare>(
                ConditionalTargetMode.Prohibited,
                ConditionMode.If,
                compareExpression,
                values
            )
        );

        return this;
    }

    /// <summary>
    /// Prohibits the field unless another field equals one of the provided values. Presence rules run before value rules, so a field this
    /// rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose value decides whether this rule applies at all.</param>
    /// <param name="values">The values that skip prohibition.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> ProhibitedUnless<TCompare>(
        Expression<Func<TRequest, TCompare>> compareExpression,
        params TCompare?[] values
    )
    {
        _propertyRule.AddRule(
            new ConditionalValidationRule<TRequest, TProperty, TCompare>(
                ConditionalTargetMode.Prohibited,
                ConditionMode.Unless,
                compareExpression,
                values
            )
        );

        return this;
    }

    /// <summary>
    /// Prohibits the field when another field contains an accepted value. Presence rules run before value rules, so a field this rejects
    /// reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose truthiness decides whether this rule applies.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> ProhibitedIfAccepted<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new ConditionalStateValidationRule<TRequest, TProperty, TCompare>(
                ConditionalStateTargetMode.Prohibited,
                ConditionalStateMode.Accepted,
                compareExpression
            )
        );

        return this;
    }

    /// <summary>
    /// Prohibits the field when another field contains a declined value. Presence rules run before value rules, so a field this rejects
    /// reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpression">Points at the field whose falseness decides whether this rule applies.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> ProhibitedIfDeclined<TCompare>(Expression<Func<TRequest, TCompare>> compareExpression)
    {
        _propertyRule.AddRule(
            new ConditionalStateValidationRule<TRequest, TProperty, TCompare>(
                ConditionalStateTargetMode.Prohibited,
                ConditionalStateMode.Declined,
                compareExpression
            )
        );

        return this;
    }

    /// <summary>
    /// Makes this field prohibit the listed fields from being present with a value. Use it for mutually exclusive request options. Presence
    /// rules run before value rules, so a field this rejects reports that rather than a later format or size failure.
    /// </summary>
    ///
    /// <param name="compareExpressions">Point at the fields watched for presence, all of which the trigger is evaluated against.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Prohibits(params Expression<Func<TRequest, object?>>[] compareExpressions)
    {
        _propertyRule.AddRule(
            new MultiFieldPresenceValidationRule<TRequest, TProperty>(
                MultiFieldPresenceTargetMode.Prohibits,
                MultiFieldPresenceTriggerMode.Prohibits,
                compareExpressions
            )
        );

        return this;
    }
}
