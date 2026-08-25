using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;

namespace AlmightyShogun.AspNet.RequestValidation;

internal readonly record struct ValidationRuleIdentity(string Value)
{
    /// <summary>
    /// Derives a value that is equal for two rules configured the same way, which is what deduplication compares. Rules whose configuration
    /// cannot be read back yield no identity and are therefore never treated as duplicates.
    /// </summary>
    ///
    /// <param name="rule">The validation rule to inspect.</param>
    /// <param name="identity">The generated rule identity.</param>
    ///
    /// <returns><c>true</c> when the rule can be represented safely; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryCreate(object rule, out ValidationRuleIdentity identity)
    {
        StringBuilder builder = new();
        HashSet<object> visited = new(ReferenceEqualityComparer.Instance);

        bool isValid = TryAppendValue(builder, rule, visited);

        identity = new ValidationRuleIdentity(builder.ToString());

        return isValid;
    }

    /// <summary>
    /// Appends a value to the identity builder when the value can be represented deterministically.
    /// </summary>
    ///
    /// <param name="builder">The identity being assembled, appended to in place so one rule yields one string.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="visited">
    /// The objects already written, compared by reference so a cycle in a rule's configuration cannot recurse forever.
    /// </param>
    ///
    /// <returns><c>true</c> when the value was appended; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryAppendValue(StringBuilder builder, object? value, HashSet<object> visited)
    {
        if (value is null)
        {
            builder.Append("null");
            
            return true;
        }

        Type type = value.GetType();

        switch (value)
        {
            case string text:
                AppendLiteral(builder, text);
            
                return true;
            case Type valueType:
                AppendLiteral(builder, valueType.AssemblyQualifiedName ?? valueType.FullName ?? valueType.Name);
                
                return true;
        }

        if (type.IsPrimitive || value is decimal or Guid)
        {
            AppendLiteral(builder, Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);
            return true;
        }

        if (type.IsEnum)
        {
            AppendLiteral(builder, $"{type.FullName}.{value}");
            return true;
        }

        switch (value)
        {
            case DateTime dateTime:
                AppendLiteral(builder, dateTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
                
                return true;
            case DateTimeOffset dateTimeOffset:
                AppendLiteral(builder, dateTimeOffset.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
                
                return true;
        }

        if (!type.IsValueType && !visited.Add(value))
        {
            builder.Append("<cycle>");
            
            return true;
        }

        if (value is IEnumerable enumerable)
            return TryAppendEnumerable(builder, enumerable, visited);

        return type.Namespace?.StartsWith("AlmightyShogun.AspNet.RequestValidation", StringComparison.Ordinal) == true
               && TryAppendObject(builder, value, visited);
    }

    /// <summary>
    /// Writes a sequence into the identity, so two rules configured with the same values in the same order match.
    /// </summary>
    ///
    /// <param name="builder">The identity being assembled, appended to in place so one rule yields one string.</param>
    /// <param name="values">The enumerable values to append.</param>
    /// <param name="visited">
    /// The objects already written, compared by reference so a cycle in a rule's configuration cannot recurse forever.
    /// </param>
    ///
    /// <returns><c>true</c> when all values were appended; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryAppendEnumerable(StringBuilder builder, IEnumerable values, HashSet<object> visited)
    {
        builder.Append('[');

        var isFirst = true;

        foreach (object? value in values)
        {
            if (!isFirst)
                builder.Append(',');

            if (!TryAppendValue(builder, value, visited))
                return false;

            isFirst = false;
        }

        builder.Append(']');

        return true;
    }

    /// <summary>
    /// Writes a nested validation object into the identity by recursing into its own fields.
    /// </summary>
    ///
    /// <param name="builder">The identity being assembled, appended to in place so one rule yields one string.</param>
    /// <param name="value">The object value to append.</param>
    /// <param name="visited">
    /// The objects already written, compared by reference so a cycle in a rule's configuration cannot recurse forever.
    /// </param>
    ///
    /// <returns><c>true</c> when the object was appended; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryAppendObject(StringBuilder builder, object value, HashSet<object> visited)
    {
        Type type = value.GetType();

        builder.Append(type.AssemblyQualifiedName);
        builder.Append('{');

        foreach (FieldInfo field in GetIdentityFields(type))
        {
            if (typeof(Delegate).IsAssignableFrom(field.FieldType)) continue;

            builder.Append(field.Name);
            builder.Append('=');

            if (!TryAppendValue(builder, field.GetValue(value), visited))
                return false;

            builder.Append(';');
        }

        builder.Append('}');

        return true;
    }

    /// <summary>
    /// Selects the fields that describe a rule's configuration, which is what identity is built from rather than the whole object.
    /// </summary>
    ///
    /// <param name="type">The rule type to inspect.</param>
    ///
    /// <returns>The ordered identity fields.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IEnumerable<FieldInfo> GetIdentityFields(Type type) => type
        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(field => field.Name != "_normalizedValues")
        .OrderBy(field => field.Name, StringComparer.Ordinal);

    /// <summary>
    /// Writes a string with its delimiters escaped, so two different configurations cannot produce the same identity by one value
    /// containing the separator of another.
    /// </summary>
    ///
    /// <param name="builder">The identity being assembled, appended to in place so one rule yields one string.</param>
    /// <param name="value">The literal value to append.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void AppendLiteral(StringBuilder builder, string value)
    {
        builder.Append('"');
        builder.Append(value.Replace("\\", @"\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal));
        builder.Append('"');
    }
}
