namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Describes the attribute rules declared on a request type, so an application can publish its own rules endpoint or generate client-side
/// checks from the same declarations the server enforces.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IValidationRuleDescriber
{
    /// <summary>
    /// Describes the validation rules declared on a request type. Results are cached per type, so a request whose rules have already been
    /// described is answered without reflecting over it again.
    /// </summary>
    ///
    /// <typeparam name="TRequest">
    /// The request type to describe. Taken as a type argument rather than a <see cref="Type"/> so the compiler resolves it instead of a
    /// runtime lookup. Nothing constrains it to a type that carries rules: one that declares none simply describes as empty.
    /// </typeparam>
    ///
    /// <returns>
    /// The rules for each property that declares at least one, keyed by the field name a client sees. A property with no rules is absent
    /// rather than present and empty, so the result reads as the rules that exist.
    /// </returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// Two of the request type's rule-carrying properties resolve to one field name, so one property's rules would be reported under a
    /// key the other claims as well. The message names both properties. A <c>new</c> property redeclaring a base one is not such a pair:
    /// the derived declaration is described, carrying the rules the base declared as well. Nothing is cached for a type that fails this
    /// way, so every later call for it fails the same way.
    /// </exception>
    ///
    /// <remarks>
    /// Only attribute rules are described. A rule declared in a <see cref="Validator{TRequest}"/> is enforced but cannot be described,
    /// since a built rule carries no record of the name it was declared under, so a request using both describes as less than it enforces.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    IReadOnlyDictionary<string, IReadOnlyList<ValidationRuleDescription>> Describe<TRequest>() where TRequest : class;
}
