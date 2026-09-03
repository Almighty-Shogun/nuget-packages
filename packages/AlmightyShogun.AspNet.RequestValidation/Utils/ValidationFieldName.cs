using System.Text.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Resolves the name a failure is reported under, which is the name the client sent rather than the name the property was declared with.
/// Every path that names a field goes through here, so the response key, the message parameters, and the described rules cannot disagree
/// about what one field is called.
/// </summary>
///
/// <remarks>
/// The naming policy is assumed to be the framework default rather than read from the serializer. Rules are built once per request type by
/// static factories that no service provider reaches, and the policy is only reachable through <c>JsonOptions</c> in the container. An
/// application serving another policy, such as snake case, must spell the name out with <see cref="JsonPropertyNameAttribute"/> for this to
/// agree with its payloads.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationFieldName
{
    /// <summary>
    /// Resolves a property's field name, preferring the name it was explicitly given over the one the policy would derive.
    /// </summary>
    ///
    /// <param name="property">The property to name, read for its serialization attribute before its own name is considered.</param>
    ///
    /// <returns>
    /// The name from <see cref="JsonPropertyNameAttribute"/> when the property carries one, and the camel-cased property name otherwise.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string FromProperty(PropertyInfo property)
        => property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? FromDeclaredName(property.Name);

    /// <summary>
    /// Converts a declared name to the form a client sees, for the callers that hold a name rather than the property it came from.
    /// </summary>
    ///
    /// <param name="value">The declared name, such as a property name or one segment of a model-state key.</param>
    ///
    /// <returns>
    /// The camel-cased name, matching <see cref="JsonNamingPolicy.CamelCase"/> rather than approximating it, so an acronym is lowered in
    /// full as the serializer would lower it. An empty value comes back unchanged.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string FromDeclaredName(string value)
        => string.IsNullOrEmpty(value) ? value : JsonNamingPolicy.CamelCase.ConvertName(value);
}
