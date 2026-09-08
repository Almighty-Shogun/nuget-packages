using System.Reflection;
using System.Collections.Concurrent;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Describes the rules a request type declares through attributes, by reading the attribute metadata the compiler recorded at the call
/// site.
/// </summary>
///
/// <remarks>
/// Attribute rules only. A rule declared in a <see cref="Validator{TRequest}"/> is enforced but not described, because a built rule carries
/// no record of the name it was declared under: one rule class serves several spellings, told apart only by the mode it holds. A request
/// using both therefore describes as less than it enforces, and a client generating its own checks from this must not treat the result as
/// the complete set.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class ValidationRuleDescriber : IValidationRuleDescriber
{
    /// <summary>
    /// The descriptions built so far, keyed by request type, so a request already described is answered without reflecting over it again.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, IReadOnlyList<ValidationRuleDescription>>> _descriptions =
        new();

    /// <inheritdoc />
    public IReadOnlyDictionary<string, IReadOnlyList<ValidationRuleDescription>> Describe<TRequest>() where TRequest : class
        => _descriptions.GetOrAdd(typeof(TRequest), BuildDescription);

    /// <summary>
    /// Walks the public properties and describes those that declare rules, omitting the ones that declare none and the base declarations a
    /// <c>new</c> property redeclares.
    /// </summary>
    ///
    /// <param name="requestType">The request type to inspect.</param>
    ///
    /// <returns>The described rules keyed by the field name a client sees rather than by the declared property name.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// Two rule-carrying properties resolve to one field name through <see cref="ValidationFieldName.FromProperty"/> without one
    /// redeclaring the other, which a <c>[JsonPropertyName]</c> spelling the name another property already has does.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static IReadOnlyDictionary<string, IReadOnlyList<ValidationRuleDescription>> BuildDescription(Type requestType)
    {
        Dictionary<string, IReadOnlyList<ValidationRuleDescription>> descriptions = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, PropertyInfo> claimants = new(StringComparer.OrdinalIgnoreCase);

        foreach (PropertyInfo property in GetNearestDeclarations(requestType))
        {
            IReadOnlyList<ValidationRuleDescription> rules = DescribeProperty(property);

            if (rules.Count == 0) continue;

            string field = ValidationFieldName.FromProperty(property);

            if (claimants.TryGetValue(field, out PropertyInfo? claimant))
                throw new InvalidOperationException(
                    $"'{requestType.Name}' declares rules on both '{ToQualifiedName(claimant)}' and '{ToQualifiedName(property)}', "
                    + $"which describe as the one field '{field}'. Give one of them a distinct name."
                );

            claimants.Add(field, property);
            descriptions.Add(field, rules);
        }

        return descriptions;
    }

    /// <summary>
    /// Reduces the public properties to the declaration each name resolves to, dropping a base declaration that a <c>new</c> property on a
    /// more derived type redeclares.
    /// </summary>
    ///
    /// <param name="requestType">The request type whose properties are wanted.</param>
    ///
    /// <returns>
    /// The properties in the order reflection reported them, less the ones a more derived type redeclares. A <c>new</c> property matching
    /// the base declaration's signature is reported once to begin with, since reflection hides the declaration it redeclares.
    /// </returns>
    ///
    /// <remarks>
    /// A dropped declaration takes none of its rules with it: <see cref="GetRuleAttributeData"/> walks by name from the surviving
    /// property's declaring type upward, so a rule written on the redeclared property is still described under the field.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IEnumerable<PropertyInfo> GetNearestDeclarations(Type requestType)
    {
        PropertyInfo[] properties = requestType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        return properties.Where(property => !properties.Any(other =>
            other.Name == property.Name
            && other.DeclaringType != property.DeclaringType
            && property.DeclaringType?.IsAssignableFrom(other.DeclaringType) is true
        ));
    }

    /// <summary>
    /// Names a property by its declaring type as well as its own name, so a colliding pair declared on two types of one hierarchy is told
    /// apart in the message that reports them.
    /// </summary>
    ///
    /// <param name="property">The property to name, read for the type it was declared on rather than the type being described.</param>
    ///
    /// <returns>The property name behind the name of the type declaring it.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ToQualifiedName(PropertyInfo property) => $"{property.DeclaringType?.Name}.{property.Name}";

    /// <summary>
    /// Describes one property's rules, including those inherited from a base declaration.
    /// </summary>
    ///
    /// <param name="property">The property to inspect.</param>
    ///
    /// <returns>The described rules for the property.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static IReadOnlyList<ValidationRuleDescription> DescribeProperty(PropertyInfo property) =>
    [
        .. GetRuleAttributeData(property)
            .Select(attributeData => new ValidationRuleDescription
            {
                Rule = GetRuleName(attributeData.AttributeType),
                Arguments = GetArguments(attributeData)
            })
    ];

    /// <summary>
    /// Collects the validation attributes declared on a property as metadata rather than as constructed instances, walking the base
    /// declarations the way <see cref="MemberInfo.GetCustomAttributes(bool)"/> would so an inherited rule is described too.
    /// </summary>
    ///
    /// <param name="property">The property whose declared rules are wanted.</param>
    ///
    /// <returns>
    /// One entry per attribute type, nearest declaration first, so an override hides the base declaration of the same attribute rather
    /// than being reported twice. The dedupe is by type across the whole walk, so a repeatable attribute written twice on one property
    /// is described once while a rule is still built from each of the two instances.
    /// </returns>
    ///
    /// <remarks>
    /// Metadata rather than instances, because no validation attribute exposes the arguments it was written with: each takes them as
    /// primary-constructor parameters and reads them only from inside its own <c>CreateRule</c>.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
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
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
    private static object? ToArgumentValue(CustomAttributeTypedArgument argument)
    {
        if (argument.Value is IReadOnlyCollection<CustomAttributeTypedArgument> elements)
            return elements.Select(ToArgumentValue).ToArray();

        return argument.ArgumentType.IsEnum && argument.Value is not null
            ? Enum.ToObject(argument.ArgumentType, argument.Value)
            : argument.Value;
    }
}
