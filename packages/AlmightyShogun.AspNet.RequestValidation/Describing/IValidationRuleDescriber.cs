namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Describes the attribute rules declared on a request type, so an application can publish its own rules endpoint or generate client-side
/// checks from the same declarations the server enforces.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
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
    /// <remarks>
    /// Only attribute rules are described. A rule declared in a <see cref="Validator{TRequest}"/> is enforced but cannot be described,
    /// since a built rule carries no record of the name it was declared under, so a request using both describes as less than it enforces.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IReadOnlyDictionary<string, IReadOnlyList<ValidationRuleDescription>> Describe<TRequest>() where TRequest : class;
}
