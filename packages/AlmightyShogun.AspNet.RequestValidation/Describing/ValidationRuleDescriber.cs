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
        .. GetRuleAttributeData(property)
            .Select(attributeData => new ValidationRuleDescription(
                GetRuleName(attributeData.AttributeType),
                GetArguments(attributeData)
            ))
    ];

    /// <summary>
    /// Collects the validation attributes declared on a property as metadata rather than as constructed instances, walking the base
    /// declarations the way <see cref="MemberInfo.GetCustomAttributes(bool)"/> would so an inherited rule is described too.
    /// </summary>
    ///
    /// <param name="property">The property whose declared rules are wanted.</param>
    ///
    /// <returns>
    /// One entry per validation attribute, nearest declaration first, so an override hides the base declaration of the same attribute
    /// rather than being reported twice.
    /// </returns>
    ///
    /// <remarks>
    /// Metadata rather than instances, because an attribute keeps no record of the arguments it was written with: a primary-constructor
    /// parameter forwarded straight to the base constructor leaves no field behind to read it back from.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IEnumerable<CustomAttributeData> GetRuleAttributeData(PropertyInfo property)
    {
        HashSet<Type> declared = [];

        for (Type? type = property.DeclaringType; type is not null; type = type.BaseType)
        {
            PropertyInfo? candidate = type.GetProperty(
                property.Name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly
            );

            if (candidate is null) continue;

            foreach (CustomAttributeData attributeData in candidate.GetCustomAttributesData())
                if (typeof(ValidationRuleAttribute).IsAssignableFrom(attributeData.AttributeType)
                    && declared.Add(attributeData.AttributeType))
                    yield return attributeData;
        }
    }

    /// <summary>
    /// Derives the rule name a client sees from the attribute's type name, which is the same name the rule catalogue documents.
    /// </summary>
    ///
    /// <param name="attributeType">The attribute type to name, read for its type name alone.</param>
    ///
    /// <returns>The rule name.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string GetRuleName(Type attributeType)
    {
        const string suffix = "Attribute";
        string name = attributeType.Name;

        return name.EndsWith(suffix, StringComparison.Ordinal) ? name[..^suffix.Length] : name;
    }

    /// <summary>
    /// Reads the arguments an attribute was written with, straight from the metadata the compiler recorded at the call site.
    /// </summary>
    ///
    /// <param name="attributeData">The attribute's metadata, carrying its constructor arguments in declaration order.</param>
    ///
    /// <returns>
    /// The declared argument values, in constructor order. An array argument comes back as an <see cref="object"/> array rather than as
    /// the metadata wrapper, so a caller reads the values themselves.
    /// </returns>
    ///
    /// <remarks>
    /// Metadata is the only place these survive. Reading them back off the constructed attribute does not work, because a
    /// primary-constructor parameter forwarded straight to the base constructor is never captured in a field of the derived type.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IReadOnlyList<object?> GetArguments(CustomAttributeData attributeData) 
        => [.. attributeData.ConstructorArguments.Select(ToArgumentValue)];

    /// <summary>
    /// Unwraps one metadata argument into the value it stands for, flattening an array argument into its elements.
    /// </summary>
    ///
    /// <param name="argument">One constructor argument as the metadata records it.</param>
    ///
    /// <returns>
    /// The value, or an <see cref="object"/> array when the argument was itself an array. An enum argument is returned as its enum type
    /// rather than as the underlying integer metadata stores it in, so a client sees the name it was written with.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static object? ToArgumentValue(CustomAttributeTypedArgument argument)
    {
        if (argument.Value is IReadOnlyCollection<CustomAttributeTypedArgument> elements)
            return elements.Select(ToArgumentValue).ToArray();

        return argument.ArgumentType.IsEnum && argument.Value is not null
            ? Enum.ToObject(argument.ArgumentType, argument.Value)
            : argument.Value;
    }
}
