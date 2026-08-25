using System.Reflection;
using System.Collections.Concurrent;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Describes validation rules by reading the same attributes the rule factory builds from, so a description cannot drift from what is
/// actually enforced.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationRuleDescriber : IValidationRuleDescriber
{
    /// <summary>
    /// The descriptions built so far, keyed by request type, so publishing the same request's rules repeatedly reflects once.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, IReadOnlyList<ValidationRuleDescription>>> _descriptions =
        new();

    /// <inheritdoc />
    public IReadOnlyDictionary<string, IReadOnlyList<ValidationRuleDescription>> Describe<TRequest>() where TRequest : class
        => _descriptions.GetOrAdd(typeof(TRequest), BuildDescription);

    /// <summary>
    /// Walks the public properties and describes those that declare rules, omitting the ones that declare none.
    /// </summary>
    ///
    /// <param name="requestType">The request type to inspect.</param>
    ///
    /// <returns>The described rules keyed by property name.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IReadOnlyDictionary<string, IReadOnlyList<ValidationRuleDescription>> BuildDescription(Type requestType)
        => requestType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(property => (property.Name, Rules: DescribeProperty(property)))
            .Where(entry => entry.Rules.Count > 0)
            .ToDictionary(entry => entry.Name, entry => entry.Rules, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Describes one property's rules, including those inherited from a base declaration.
    /// </summary>
    ///
    /// <param name="property">The property to inspect.</param>
    ///
    /// <returns>The described rules for the property.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IReadOnlyList<ValidationRuleDescription> DescribeProperty(PropertyInfo property) =>
        [
            .. property
                .GetCustomAttributes<ValidationRuleAttribute>(true)
                .Select(attribute => new ValidationRuleDescription(GetRuleName(attribute), GetArguments(attribute)))
        ];

    /// <summary>
    /// Derives the rule name a client sees from the attribute's type name, which is the same name the rule catalogue documents.
    /// </summary>
    ///
    /// <param name="attribute">The attribute to describe, read for its type name and the arguments it was declared with.</param>
    ///
    /// <returns>The rule name.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string GetRuleName(ValidationRuleAttribute attribute)
    {
        const string suffix = "Attribute";
        string name = attribute.GetType().Name;

        return name.EndsWith(suffix, StringComparison.Ordinal) ? name[..^suffix.Length] : name;
    }

    /// <summary>
    /// Recovers the arguments an attribute was written with, by reading the properties its longest constructor names, since an attribute
    /// keeps no record of its own call site.
    /// </summary>
    ///
    /// <param name="attribute">The attribute to describe, read for its type name and the arguments it was declared with.</param>
    ///
    /// <returns>The declared argument values.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IReadOnlyList<object?> GetArguments(ValidationRuleAttribute attribute)
    {
        ConstructorInfo? constructor = attribute.GetType()
            .GetConstructors()
            .OrderByDescending(candidate => candidate.GetParameters().Length)
            .FirstOrDefault();

        if (constructor is null)
            return [];

        List<object?> arguments =
        [
            .. constructor.GetParameters()
                .Select(parameter => attribute.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .FirstOrDefault(candidate =>
                        candidate.Name.Contains($"<{parameter.Name}>", StringComparison.Ordinal)
                        || candidate.Name.Equals(parameter.Name, StringComparison.OrdinalIgnoreCase))
                )
                .OfType<FieldInfo>()
                .Select(field => field.GetValue(attribute))

        ];

        return arguments;
    }
}
