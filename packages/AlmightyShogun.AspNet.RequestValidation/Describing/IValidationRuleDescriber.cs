namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Describes the validation rules declared on a request type, so an application can publish its own rules endpoint or generate client-side
/// validation from the same declarations the server enforces.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IValidationRuleDescriber
{
    /// <summary>
    /// Describes the validation rules declared on a request type. Results are cached per type, so publishing the rules for the same request
    /// on every call costs one reflection pass for the life of the process.
    /// </summary>
    ///
    /// <typeparam name="TRequest">
    /// The request type to describe. Taken as a type argument rather than a <see cref="Type"/> so the compiler resolves it instead of a
    /// runtime lookup. Nothing constrains it to a type that carries rules: one that declares none simply describes as empty.
    /// </typeparam>
    ///
    /// <returns>
    /// The rules for each property that declares at least one, keyed by property name. A property with no rules is absent rather than
    /// present and empty, so the result reads as the rules that exist.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IReadOnlyDictionary<string, IReadOnlyList<ValidationRuleDescription>> Describe<TRequest>() where TRequest : class;
}
