using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Base for every validation attribute. It carries no rule data of its own: each attribute holds the arguments it was written with and
/// builds its own rule, so adding a rule family means adding an attribute rather than editing this class. Presence attributes run ahead
/// of every other attribute on the same property whatever order they are written in, so a field one of them rejects reports that rather
/// than a later format or size failure.
/// </summary>
///
/// <remarks>
/// The constructor is <c>private protected</c> , so deriving from this class directly is closed to this assembly. An application extends
/// the surface through <see cref="CustomRuleAttribute"/> and <see cref="ICustomValidationRule{TRequest, TProperty}"/> instead, which is
/// public and carries a <c>protected</c> constructor, and is the supported way to run logic the built-in rules cannot express.
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
    /// <exception cref="ArgumentException">
    /// An argument cannot be turned into the rule it configures: a blank date format, or a regular expression pattern the runtime refuses
    /// to parse.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// An attribute taking a set of values was written with none, a confirmation attribute found neither sibling property on
    /// <typeparamref name="TRequest"/>, or a regular expression timeout lies outside the range the runtime permits.
    /// </exception>
    /// <exception cref="FormatException">
    /// A date attribute's literal target is not a date the invariant culture parses.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A set-membership attribute was written with an argument that is not assignable to <typeparamref name="TProperty"/>, which
    /// <c>[In(1, 2)]</c> on a <c>long</c> property produces.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// An attribute names a field <typeparamref name="TRequest"/> does not declare, or a custom-rule attribute names a type that does not
    /// implement <see cref="ICustomValidationRule{TRequest, TProperty}"/> for these two type arguments.
    /// </exception>
    /// <exception cref="OverflowException">
    /// A size attribute's argument lies outside the range <c>decimal</c> represents, which every one of them converts to.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal abstract IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        where TRequest : class;
}
