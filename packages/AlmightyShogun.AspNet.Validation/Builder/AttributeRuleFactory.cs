using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Builds validation rules from validation attributes.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class AttributeRuleFactory
{
    /// <summary>
    /// Checks whether a request type has validation attributes on any public property.
    /// </summary>
    ///
    /// <param name="requestType">The request type to inspect.</param>
    ///
    /// <returns><c>true</c> when at least one validation attribute exists; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool HasRules(Type requestType) => requestType
        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
        .Any(property => property.GetCustomAttributes<ValidationRuleAttribute>(true).Any());

    /// <summary>
    /// Creates validation rules from attributes declared on a request type.
    /// </summary>
    ///
    /// <returns>The validation rules created from request attributes.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlyList<IRequestValidationRule<TRequest>> CreateRules<TRequest>() where TRequest : class
        => typeof(TRequest)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(property => (Property: property, Attributes: property.GetCustomAttributes<ValidationRuleAttribute>(true).ToArray()))
            .Where(rule => rule.Attributes.Length > 0)
            .Select(rule => CreatePropertyRule<TRequest>(rule.Property, rule.Attributes))
            .ToArray();

    /// <summary>
    /// Creates a typed property rule through reflection for an attributed property.
    /// </summary>
    ///
    /// <param name="property">The attributed property.</param>
    /// <param name="attributes">The validation attributes on the property.</param>
    ///
    /// <returns>The request validation rule for the property.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IRequestValidationRule<TRequest> CreatePropertyRule<TRequest>(PropertyInfo property, IReadOnlyList<ValidationRuleAttribute> attributes)
        where TRequest : class => (IRequestValidationRule<TRequest>)typeof(AttributeRuleFactory)
        .GetMethod(nameof(CreateTypedPropertyRule), BindingFlags.Static | BindingFlags.NonPublic)!
        .MakeGenericMethod(typeof(TRequest), property.PropertyType)
        .Invoke(null, [property, attributes])!;

    /// <summary>
    /// Creates a property rule for a specific request and property type.
    /// </summary>
    ///
    /// <param name="property">The attributed property.</param>
    /// <param name="attributes">The validation attributes on the property.</param>
    ///
    /// <returns>The typed property rule populated with attribute rules.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static PropertyRule<TRequest, TProperty> CreateTypedPropertyRule<TRequest, TProperty>(PropertyInfo property, IReadOnlyList<ValidationRuleAttribute> attributes) where TRequest : class
    {
        var rule = PropertyRule<TRequest, TProperty>.FromPropertyInfo(property);

        foreach (ValidationRuleAttribute attribute in attributes)
            rule.AddRule(attribute.CreateRule<TRequest, TProperty>(property));

        return rule;
    }
}
