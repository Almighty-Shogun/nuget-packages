using System.Reflection;
using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Addresses one field of a request and reads its value. A field may be named by an expression or by a string, and the rules above work the
/// same way with either.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationField<TRequest> where TRequest : class
{
    public string Name { get; }

    private readonly Func<TRequest, object?> _getter;

    /// <summary>
    /// Builds a field from a name and a reader, which is the form every other factory here reduces to.
    /// </summary>
    ///
    /// <param name="name">The validation field name.</param>
    /// <param name="getter">The field value getter.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private ValidationField(string name, Func<TRequest, object?> getter)
    {
        Name = name;
        _getter = getter;
    }

    /// <summary>
    /// Reads this field's current value from a request instance.
    /// </summary>
    ///
    /// <param name="request">The request being validated, so a rule can read another field as well as its own.</param>
    ///
    /// <returns>The field value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public object? GetValue(TRequest request) => _getter(request);

    /// <summary>
    /// Builds a field from an expression, which is the compiler-checked spelling used by the fluent surface.
    /// </summary>
    ///
    /// <param name="expression">
    /// Points at the property, supplying both its public field name and the reader used to fetch its value.
    /// </param>
    ///
    /// <returns>The validation field.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationField<TRequest> From<TProperty>(Expression<Func<TRequest, TProperty>> expression)
    {
        Func<TRequest, TProperty> getter = expression.Compile();

        return new ValidationField<TRequest>(GetPropertyName(expression), request => getter(request));
    }

    /// <summary>
    /// Builds several fields at once, for the rules that watch a set of other properties.
    /// </summary>
    ///
    /// <param name="expressions">The property expressions.</param>
    ///
    /// <returns>The validation fields.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlyList<ValidationField<TRequest>> FromMany(params Expression<Func<TRequest, object?>>[] expressions)
        => expressions.Select(From).ToArray();

    /// <summary>
    /// Builds several fields from names, which is the attribute spelling since an attribute cannot hold expressions.
    /// </summary>
    ///
    /// <param name="propertyNames">The property names.</param>
    ///
    /// <returns>The validation fields.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlyList<ValidationField<TRequest>> FromMany(params string[] propertyNames)
        => propertyNames.Select(FromPropertyName).ToArray();

    /// <summary>
    /// Builds a field from a name, resolving the property by reflection. A name that matches nothing yields a field that always reads as
    /// absent, so a mistyped name fails the rule rather than the request.
    /// </summary>
    ///
    /// <param name="propertyName">The property name.</param>
    ///
    /// <returns>The validation field.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationField<TRequest> FromPropertyName(string propertyName)
    {
        PropertyInfo property = ResolveProperty(propertyName);

        return new ValidationField<TRequest>(ToCamelCase(property.Name), property.GetValue);
    }

    /// <summary>
    /// Renders a set of field names as the list a message template substitutes.
    /// </summary>
    ///
    /// <param name="fields">The fields to join.</param>
    ///
    /// <returns>The joined field names.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string JoinNames(IEnumerable<ValidationField<TRequest>> fields)
        => ValidationValue.JoinValues(fields.Select(field => field.Name));

    /// <summary>
    /// Finds a property by its public field name, matching case-insensitively so the camel-cased client spelling resolves.
    /// </summary>
    ///
    /// <param name="propertyName">The property name.</param>
    ///
    /// <returns>The resolved property metadata.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static PropertyInfo ResolveProperty(string propertyName)
    {
        Type requestType = typeof(TRequest);
        PropertyInfo? property = requestType
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        return property
               ?? throw new InvalidOperationException($"Validation property '{propertyName}' was not found on '{requestType.Name}'.");
    }

    /// <summary>
    /// Reads the property an expression points at and converts it to the public field name.
    /// </summary>
    ///
    /// <param name="expression">
    /// Points at the property, supplying both its public field name and the reader used to fetch its value.
    /// </param>
    ///
    /// <returns>The camel-cased property name.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string GetPropertyName<TProperty>(Expression<Func<TRequest, TProperty>> expression) => expression.Body switch
    {
        MemberExpression { Member: PropertyInfo propertyInfo } => ToCamelCase(propertyInfo.Name),
        UnaryExpression { Operand: MemberExpression { Member: PropertyInfo unaryPropertyInfo } } => ToCamelCase(unaryPropertyInfo.Name),
        _ => throw new InvalidOperationException("Validation rules only support property access expressions.")
    };

    /// <summary>
    /// Converts a property name to the camel-cased form failures are reported under, which is the shape a JSON client sees.
    /// </summary>
    ///
    /// <param name="value">The property name to convert, as declared in the type rather than as a client would spell it.</param>
    ///
    /// <returns>The camel-cased value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
