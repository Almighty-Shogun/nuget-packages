using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Turns the validation attributes declared on a request type into rules, once per type on its first validation, so only the first request
/// of each type pays for the reflection.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class AttributeRuleFactory
{
    /// <summary>
    /// Reports whether a type declares any validation attributes on its public instance properties. A fluent validator counts for
    /// nothing here, so <see cref="ValidationRuleCache.HasRules"/> also asks the registry before deciding a request has no rules.
    /// </summary>
    ///
    /// <param name="requestType">The request type to inspect.</param>
    ///
    /// <returns><c>true</c> when at least one validation attribute exists; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool HasRules(Type requestType) => requestType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
        .Any(property => property.GetCustomAttributes<ValidationRuleAttribute>(true).Any());

    /// <summary>
    /// Builds one rule per attributed property, skipping properties that declare nothing.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type whose declared attributes are read.</typeparam>
    ///
    /// <returns>One rule per property carrying at least one validation attribute, empty when the type carries none.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlyList<IRequestValidationRule<TRequest>> CreateRules<TRequest>() where TRequest : class =>
    [
        .. typeof(TRequest)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(property => (Property: property, Attributes: property.GetCustomAttributes<ValidationRuleAttribute>(true).ToArray()))
            .Where(rule => rule.Attributes.Length > 0)
            .Select(rule => CreatePropertyRule<TRequest>(rule.Property, rule.Attributes))
    ];

    /// <summary>
    /// Bridges from reflection to generics: the property's type is only known at runtime, so the typed builder is invoked through a
    /// constructed generic method rather than called directly.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type the built rule reads the property from.</typeparam>
    /// <param name="property">The attributed property.</param>
    /// <param name="attributes">The attributes declared on the property, including any inherited from a base declaration.</param>
    ///
    /// <returns>The property's rule, already holding one rule per declared attribute.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IRequestValidationRule<TRequest> CreatePropertyRule<TRequest>(
        PropertyInfo property,
        IReadOnlyList<ValidationRuleAttribute> attributes
    ) where TRequest : class => (IRequestValidationRule<TRequest>)typeof(AttributeRuleFactory)
        .GetMethod(nameof(CreateTypedPropertyRule), BindingFlags.Static | BindingFlags.NonPublic)!
        .MakeGenericMethod(typeof(TRequest), property.PropertyType)
        .Invoke(null, [property, attributes])!;

    /// <summary>
    /// Builds the typed rule once the property's type is known, which is the generic method the reflective step above invokes.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type the built rule reads the property from.</typeparam>
    /// <typeparam name="TProperty">The property's own type, supplied at runtime by the reflective step above.</typeparam>
    /// <param name="property">The attributed property.</param>
    /// <param name="attributes">The attributes declared on the property, including any inherited from a base declaration.</param>
    ///
    /// <returns>The typed property rule populated with attribute rules.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static PropertyRule<TRequest, TProperty> CreateTypedPropertyRule<TRequest, TProperty>(
        PropertyInfo property,
        IReadOnlyList<ValidationRuleAttribute> attributes
    ) where TRequest : class
    {
        PropertyRule<TRequest, TProperty> rule = PropertyRule<TRequest, TProperty>.FromPropertyInfo(property);

        foreach (ValidationRuleAttribute attribute in attributes)
            rule.AddRule(attribute.CreateRule<TRequest, TProperty>(property));

        return rule;
    }
}
