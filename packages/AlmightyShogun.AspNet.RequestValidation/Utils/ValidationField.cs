using System.Reflection;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

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
    /// <summary>
    /// Gets the field's public name, resolved through <see cref="ValidationFieldName"/> , so it is the name the client sent rather than the
    /// name the property was declared with.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string Name { get; }

    /// <summary>
    /// Reads the field's value, compiled from the expression or built from reflection so both spellings behave identically afterwards.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="expression"/> is not a property read directly off the request, such as a nested read or a method call. Thrown as
    /// the field is built rather than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationField<TRequest> From<TProperty>(Expression<Func<TRequest, TProperty>> expression)
    {
        string name = ValidationExpression.GetFieldName(expression);
        Func<TRequest, TProperty> getter = expression.Compile();

        return new ValidationField<TRequest>(name, request => getter(request));
    }

    /// <summary>
    /// Builds several fields at once, for the rules that watch a set of other properties.
    /// </summary>
    ///
    /// <param name="expressions">The property expressions.</param>
    ///
    /// <returns>The validation fields.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// One of <paramref name="expressions"/> is not a property read directly off the request.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlyList<ValidationField<TRequest>> FromMany(params Expression<Func<TRequest, object?>>[] expressions)
        => [.. expressions.Select(From)];

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
        => [.. propertyNames.Select(FromPropertyName)];

    /// <summary>
    /// Builds a field from a name, resolving the property by reflection. A name matching no property throws rather than yielding a field
    /// that reads as absent, so a mistyped name surfaces as a fault instead of a quietly passing rule.
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

        return new ValidationField<TRequest>(ValidationFieldName.FromProperty(property), property.GetValue);
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
        => ValidationDisplay.JoinValues(fields.Select(field => field.Name));

    /// <summary>
    /// Finds a property by name, matching case-insensitively so the camel-cased client spelling resolves, and then by the explicit
    /// serialization name so a field a client only knows under a <see cref="JsonPropertyNameAttribute"/> resolves too.
    /// </summary>
    ///
    /// <param name="propertyName">The property name.</param>
    ///
    /// <returns>The resolved property metadata.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// No public instance property on the request type carries that name, which a mistyped field name in a rule produces.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static PropertyInfo ResolveProperty(string propertyName)
    {
        Type requestType = typeof(TRequest);
        PropertyInfo? property = requestType
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        property ??= Array.Find(
            requestType.GetProperties(BindingFlags.Instance | BindingFlags.Public),
            candidate => string.Equals(ValidationFieldName.FromProperty(candidate), propertyName, StringComparison.OrdinalIgnoreCase)
        );

        return property 
               ?? throw new InvalidOperationException($"Validation property '{propertyName}' was not found on '{requestType.Name}'.");
    }
}
