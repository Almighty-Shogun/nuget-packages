using System.Reflection;
using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Holds one property's rules and runs them in order. It is where a field's rules are merged, deduplicated, and sorted so the pipeline
/// above deals in fields rather than in individual rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class PropertyRule<TRequest, TProperty> : IRequestValidationRule<TRequest> where TRequest : class
{
    /// <summary>
    /// Gets the name failures are reported under, resolved through <see cref="ValidationFieldName"/> , so it is the name the client sent
    /// rather than the name the property was declared with.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string FieldName { get; }

    /// <summary>
    /// The property's name as the type declares it, which the confirmation convention needs so it can look for a sibling property by
    /// name. Kept apart from <see cref="FieldName"/> , since that one may have been renamed for the client.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal string DeclaredName { get; }

    /// <summary>
    /// Reads the property's value, compiled from the expression or built from reflection so the two paths are the same afterwards.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Func<TRequest, TProperty> _getter;

    /// <summary>
    /// The rules for this field, in the order they will run once merging and the priority sort have finished with them.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly List<IPropertyValidationRule<TRequest, TProperty>> _rules = [];

    /// <summary>
    /// Exposes the rules for the grouped composition rule, which gathers a nested set rather than running them itself.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal IReadOnlyList<IPropertyValidationRule<TRequest, TProperty>> Rules => _rules;

    /// <summary>
    /// Builds a rule for a property named by an expression, which is the fluent spelling and the only one the compiler checks.
    /// </summary>
    ///
    /// <param name="expression">The property expression to validate.</param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="expression"/> is not a property read directly off the request, such as a nested read or a method call. Thrown as
    /// the rule is built rather than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public PropertyRule(Expression<Func<TRequest, TProperty>> expression)
        : this(ValidationExpression.GetProperty(expression), expression.Compile()) { }

    /// <summary>
    /// Builds a rule for a property already resolved to metadata, which is where both the field name and the declared name come from.
    /// </summary>
    ///
    /// <param name="property">The property metadata.</param>
    /// <param name="getter">The property value getter.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private PropertyRule(PropertyInfo property, Func<TRequest, TProperty> getter)
        : this(ValidationFieldName.FromProperty(property), property.Name, getter) { }

    /// <summary>
    /// Builds a rule for a field addressed by name and reader, for the cases where no expression exists to point at it.
    /// </summary>
    ///
    /// <param name="fieldName">The public validation field name.</param>
    /// <param name="declaredName">The property name as declared, which the confirmation convention searches siblings by.</param>
    /// <param name="getter">The property value getter.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private PropertyRule(string fieldName, string declaredName, Func<TRequest, TProperty> getter)
    {
        _getter = getter;
        FieldName = fieldName;
        DeclaredName = declaredName;
    }

    /// <summary>
    /// Builds a rule for a property discovered by reflection, which is the attribute path where the property is only known at runtime.
    /// </summary>
    ///
    /// <param name="property">The property metadata.</param>
    ///
    /// <returns>The property rule for the reflected property.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static PropertyRule<TRequest, TProperty> FromPropertyInfo(PropertyInfo property)
        => new(property, request => (TProperty)property.GetValue(request)!);

    /// <summary>
    /// Builds a detached rule used to gather a nested set, as the grouped composition rule needs, so the collected rules never join the
    /// request's own field rules.
    /// </summary>
    ///
    /// <param name="fieldName">The field name shared by the rule set.</param>
    ///
    /// <returns>The empty rule set property rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static PropertyRule<TRequest, TProperty> CreateRuleSet(string fieldName) => new(fieldName, fieldName, _ => default!);

    /// <summary>
    /// Appends a rule, keeping declaration order, which is what decides evaluation order within a band.
    /// </summary>
    ///
    /// <param name="rule">The validation rule to add.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public void AddRule(IPropertyValidationRule<TRequest, TProperty> rule) => _rules.Add(rule);

    /// <summary>
    /// Absorbs another rule for the same field, so the field ends with one rule holding both sets rather than two rules reporting apart.
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
    /// Drops rules identical to one already held and reorders so the presence band runs first. Both happen while a request type's rules
    /// are being built, rather than on every request.
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
            if (ValidationRuleIdentity.TryCreate(rule, out ValidationRuleIdentity identity) && !identities.Add(identity)) continue;

            rules.Add(rule);
        }

        _rules.Clear();

        AddRulesByPriority(rules, ValidationRulePriority.Required);
        AddRulesByPriority(rules, ValidationRulePriority.Normal);
    }

    /// <inheritdoc />
    public async ValueTask ValidateAsync(
        TRequest request,
        ValidationBag errors,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        TProperty value = _getter(request);

        foreach (IPropertyValidationRule<TRequest, TProperty> rule in _rules)
        {
            ValidationRuleResult result = await rule.ValidateAsync(request, value, FieldName, serviceProvider, cancellationToken);

            if (result.IsValid) continue;

            errors.Add(FieldName, result.Key, result.Parameters);

            return;
        }
    }

    /// <summary>
    /// Re-adds one band's rules in order, which is how the reordering above is performed without disturbing declaration order inside a
    /// band.
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
            _rules.Add(rule);
    }
}
