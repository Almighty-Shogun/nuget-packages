using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Base for every validation attribute. It carries no rule data of its own: each attribute holds the arguments it was written with and
/// builds its own rule, so adding a rule family means adding an attribute rather than editing this class.
/// </summary>
///
/// <remarks>
/// The constructor is <c>private protected</c> , so the set of validation attributes is closed to this assembly. An application extends
/// the surface through <see cref="CustomRuleAttribute"/> and <see cref="ICustomValidationRule{TRequest, TProperty}"/> instead, which is
/// the supported way to run logic the built-in rules cannot express.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class ValidationRuleAttribute : Attribute
{
    /// <summary>
    /// Creates the attribute. Constrained to this assembly, since the rule an attribute builds has to be one this package implements.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute() { }

    /// <summary>
    /// Builds the rule this attribute stands for, once for the request type rather than per request.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type the rule reads from.</typeparam>
    /// <typeparam name="TProperty">The decorated property's type, which decides what the rule is able to measure.</typeparam>
    /// <param name="property">
    /// The decorated property. Supplied because a few rules need more than their own arguments: the confirmation rule reads the declared
    /// name to find its sibling, and the enum rule reads the property's type to know which enum to check against.
    /// </param>
    ///
    /// <returns>The rule to run for the decorated property.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal abstract IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        where TRequest : class;
}
