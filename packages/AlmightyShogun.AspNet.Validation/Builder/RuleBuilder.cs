namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Builds validation rules for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed partial class RuleBuilder<TRequest, TProperty> where TRequest : class
{
    private readonly PropertyRule<TRequest, TProperty> _propertyRule;

    /// <summary>
    /// Creates a rule builder for a property rule.
    /// </summary>
    ///
    /// <param name="propertyRule">The property rule that receives configured validation rules.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal RuleBuilder(PropertyRule<TRequest, TProperty> propertyRule) => _propertyRule = propertyRule;

    /// <summary>
    /// Adds grouped rule sets where at least one rule set must pass.
    /// </summary>
    ///
    /// <param name="ruleSets">The rule sets that can validate the property.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> AnyOf(params Action<RuleBuilder<TRequest, TProperty>>[] ruleSets)
    {
        IReadOnlyList<IReadOnlyList<IPropertyValidationRule<TRequest, TProperty>>> configuredRuleSets = ruleSets
            .Select(CreateRuleSet)
            .ToArray();

        _propertyRule.AddRule(new AnyOfValidationRule<TRequest, TProperty>(configuredRuleSets));

        return this;
    }

    /// <summary>
    /// Builds a standalone rule set from a rule builder callback.
    /// </summary>
    ///
    /// <param name="configure">The callback that configures the rule set.</param>
    ///
    /// <returns>The validation rules configured by the callback.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IReadOnlyList<IPropertyValidationRule<TRequest, TProperty>> CreateRuleSet(Action<RuleBuilder<TRequest, TProperty>> configure)
    {
        var ruleSet = PropertyRule<TRequest, TProperty>.CreateRuleSet(_propertyRule.FieldName);

        configure(new RuleBuilder<TRequest, TProperty>(ruleSet));

        return ruleSet.Rules;
    }
}
