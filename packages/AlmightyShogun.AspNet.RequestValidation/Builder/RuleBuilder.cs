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
    /// The rule the chain appends to. Every builder method adds to this one instance, which is what lets a chain read as one statement.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly PropertyRule<TRequest, TProperty> _propertyRule;

    /// <summary>
    /// Wraps a property rule so the fluent methods have something to append to.
    /// </summary>
    ///
    /// <param name="propertyRule">The property rule that receives configured validation rules.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal RuleBuilder(PropertyRule<TRequest, TProperty> propertyRule) => _propertyRule = propertyRule;

    /// <summary>
    /// Requires at least one configured rule set to pass. Each callback receives an isolated rule builder for the same field, and the field
    /// is valid when one of those builders completes without a rule failure. This rule is fluent-only because attributes cannot express
    /// nested rule groups cleanly. An absent or empty value passes, so pair it with <see cref="RuleBuilder{TRequest,TProperty}.Required"/>
    /// when the field is mandatory.
    /// </summary>
    ///
    /// <param name="ruleSets">The rule sets that can validate the property.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AnyOf(params Action<RuleBuilder<TRequest, TProperty>>[] ruleSets)
    {
        IReadOnlyList<IReadOnlyList<IPropertyValidationRule<TRequest, TProperty>>> configuredRuleSets =
        [
            .. ruleSets.Select(CreateRuleSet)
        ];

        _propertyRule.AddRule(new AnyOfValidationRule<TRequest, TProperty>(configuredRuleSets));

        return this;
    }

    /// <summary>
    /// Collects a nested rule set from a callback, for the grouped rule that needs several alternatives built independently of the field.
    /// </summary>
    ///
    /// <param name="configure">The callback that configures the rule set.</param>
    ///
    /// <returns>The validation rules configured by the callback.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IReadOnlyList<IPropertyValidationRule<TRequest, TProperty>> CreateRuleSet(
        Action<RuleBuilder<TRequest, TProperty>> configure
    )
    {
        PropertyRule<TRequest, TProperty> ruleSet = PropertyRule<TRequest, TProperty>.CreateRuleSet(_propertyRule.FieldName);

        configure(new RuleBuilder<TRequest, TProperty>(ruleSet));

        return ruleSet.Rules;
    }
}
