using System.Reflection;
using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Stores and executes validation rules for a single request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class PropertyRule<TRequest, TProperty> : IRequestValidationRule<TRequest> where TRequest : class
{
    public string FieldName { get; }

    private readonly Func<TRequest, TProperty> _getter;

    private readonly List<IPropertyValidationRule<TRequest, TProperty>> _rules = [];

    internal IReadOnlyList<IPropertyValidationRule<TRequest, TProperty>> Rules => _rules;

    /// <summary>
    /// Creates a property rule from a request property expression.
    /// </summary>
    ///
    /// <param name="expression">The property expression to validate.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public PropertyRule(Expression<Func<TRequest, TProperty>> expression)
        : this(GetPropertyName(expression), expression.Compile()) { }

    /// <summary>
    /// Creates a property rule from an explicit field name and getter.
    /// </summary>
    ///
    /// <param name="fieldName">The public validation field name.</param>
    /// <param name="getter">The property value getter.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private PropertyRule(string fieldName, Func<TRequest, TProperty> getter)
    {
        _getter = getter;
        FieldName = fieldName;
    }

    /// <summary>
    /// Creates a property rule from reflected property metadata.
    /// </summary>
    ///
    /// <param name="property">The property metadata.</param>
    ///
    /// <returns>The property rule for the reflected property.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static PropertyRule<TRequest, TProperty> FromPropertyInfo(PropertyInfo property)
        => new(ToCamelCase(property.Name), request => (TProperty)property.GetValue(request)!);

    /// <summary>
    /// Creates a property rule used to collect an inline rule set.
    /// </summary>
    ///
    /// <param name="fieldName">The field name shared by the rule set.</param>
    ///
    /// <returns>The empty rule set property rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static PropertyRule<TRequest, TProperty> CreateRuleSet(string fieldName) => new(fieldName, _ => default!);

    /// <summary>
    /// Adds a validation rule to this property rule.
    /// </summary>
    ///
    /// <param name="rule">The validation rule to add.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public void AddRule(IPropertyValidationRule<TRequest, TProperty> rule) => _rules.Add(rule);

    /// <summary>
    /// Attempts to merge another request rule into this property rule.
    /// </summary>
    ///
    /// <param name="rule">The rule to merge.</param>
    ///
    /// <returns><c>true</c> when the rules target the same property; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool TryMerge(IRequestValidationRule<TRequest> rule)
    {
        if (rule is not PropertyRule<TRequest, TProperty> propertyRule)
            return false;

        if (!FieldName.Equals(propertyRule.FieldName, StringComparison.OrdinalIgnoreCase))
            return false;

        _rules.AddRange(propertyRule._rules);

        return true;
    }

    /// <summary>
    /// Removes duplicate rules and places required rules before normal rules.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public void DeduplicateRules()
    {
        HashSet<ValidationRuleIdentity> identities = [];
        List<IPropertyValidationRule<TRequest, TProperty>> rules = [];

        foreach (IPropertyValidationRule<TRequest, TProperty> rule in _rules)
        {
            if (ValidationRuleIdentity.TryCreate(rule, out ValidationRuleIdentity identity) && !identities.Add(identity))
                continue;

            rules.Add(rule);
        }

        _rules.Clear();
        AddRulesByPriority(rules, ValidationRulePriority.Required);
        AddRulesByPriority(rules, ValidationRulePriority.Normal);
    }

    /// <inheritdoc />
    public async ValueTask ValidateAsync(TRequest request, ValidationBag errors, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        TProperty value = _getter(request);

        foreach (IPropertyValidationRule<TRequest, TProperty> rule in _rules)
        {
            ValidationRuleResult result = await rule.ValidateAsync(request, value, FieldName, serviceProvider, cancellationToken);

            if (result.IsValid)
                continue;

            errors.Add(FieldName, result.Key, result.Parameters);

            return;
        }
    }

    /// <summary>
    /// Gets the public field name from a property expression.
    /// </summary>
    ///
    /// <param name="expression">The property expression.</param>
    ///
    /// <returns>The camel-cased property name.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string GetPropertyName(Expression<Func<TRequest, TProperty>> expression) => expression.Body switch
    {
        MemberExpression { Member: PropertyInfo propertyInfo } => ToCamelCase(propertyInfo.Name),
        UnaryExpression { Operand: MemberExpression { Member: PropertyInfo unaryPropertyInfo } } => ToCamelCase(unaryPropertyInfo.Name),
        _ => throw new InvalidOperationException("RuleFor only supports property access expressions.")
    };

    /// <summary>
    /// Converts a property name to a camel-cased validation field name.
    /// </summary>
    ///
    /// <param name="value">The property name.</param>
    ///
    /// <returns>The camel-cased value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// Adds rules with a specific priority back into the property rule.
    /// </summary>
    ///
    /// <param name="rules">The source rules.</param>
    /// <param name="priority">The priority to add.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void AddRulesByPriority(List<IPropertyValidationRule<TRequest, TProperty>> rules, ValidationRulePriority priority)
    {
        foreach (IPropertyValidationRule<TRequest, TProperty> rule in rules.Where(rule => rule.Priority == priority))
        {
            _rules.Add(rule);
        }
    }
}
