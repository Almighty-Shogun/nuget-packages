using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;

namespace AlmightyShogun.AspNet.Validation;

internal readonly record struct ValidationRuleIdentity(string Value)
{
    /// <summary>
    /// Attempts to create a stable identity for a validation rule instance.
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
    /// <param name="builder">The identity builder.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="visited">The already visited reference values.</param>
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

        if (value is string text)
        {
            AppendLiteral(builder, text);
            return true;
        }

        if (value is Type valueType)
        {
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

        if (value is DateTime dateTime)
        {
            AppendLiteral(builder, dateTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
            return true;
        }

        if (value is DateTimeOffset dateTimeOffset)
        {
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

        return type.Namespace?.StartsWith("AlmightyShogun.AspNet.Validation", StringComparison.Ordinal) == true
               && TryAppendObject(builder, value, visited);
    }

    /// <summary>
    /// Appends enumerable values to the identity builder.
    /// </summary>
    ///
    /// <param name="builder">The identity builder.</param>
    /// <param name="values">The enumerable values to append.</param>
    /// <param name="visited">The already visited reference values.</param>
    ///
    /// <returns><c>true</c> when all values were appended; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryAppendEnumerable(StringBuilder builder, IEnumerable values, HashSet<object> visited)
    {
        builder.Append('[');

        bool isFirst = true;

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
    /// Appends an internal validation object to the identity builder.
    /// </summary>
    ///
    /// <param name="builder">The identity builder.</param>
    /// <param name="value">The object value to append.</param>
    /// <param name="visited">The already visited reference values.</param>
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
            if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                continue;

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
    /// Gets the fields that participate in a rule identity.
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
    /// Appends an escaped string literal to the identity builder.
    /// </summary>
    ///
    /// <param name="builder">The identity builder.</param>
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
