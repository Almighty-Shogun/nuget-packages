using System.Reflection;
using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Provides typed access to request fields used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationField<TRequest> where TRequest : class
{
    public string Name { get; }

    private readonly Func<TRequest, object?> _getter;

    /// <summary>
    /// Creates a validation field from a display name and getter.
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
    /// Gets the field value from a request.
    /// </summary>
    ///
    /// <param name="request">The request instance.</param>
    ///
    /// <returns>The field value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public object? GetValue(TRequest request) => _getter(request);

    /// <summary>
    /// Creates a validation field from a property expression.
    /// </summary>
    ///
    /// <param name="expression">The property expression.</param>
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
    /// Creates validation fields from property expressions.
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
    /// Creates validation fields from property names.
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
    /// Creates a validation field from a property name.
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
    /// Joins validation field names for message parameters.
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
    /// Resolves request property metadata by name.
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
        PropertyInfo? property = requestType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        return property ?? throw new InvalidOperationException($"Validation property '{propertyName}' was not found on '{requestType.Name}'.");
    }

    /// <summary>
    /// Gets the validation field name from a property expression.
    /// </summary>
    ///
    /// <param name="expression">The property expression.</param>
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
    /// Converts a property name to a camel-cased validation field name.
    /// </summary>
    ///
    /// <param name="value">The property name.</param>
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
