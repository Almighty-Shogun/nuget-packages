using System.Reflection;
using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates field comparison constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class FieldComparisonValidationRule<TRequest, TProperty, TCompare> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    private readonly FieldComparisonMode _mode;

    private readonly string? _compareFieldName;

    private readonly Func<TRequest, object?>? _compareGetter;

    /// <summary>
    /// Creates a field comparison rule from a property expression.
    /// </summary>
    ///
    /// <param name="mode">The field comparison mode.</param>
    /// <param name="compareExpression">The comparison property expression.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public FieldComparisonValidationRule(FieldComparisonMode mode, Expression<Func<TRequest, TCompare>> compareExpression)
    {
        Func<TRequest, TCompare> getter = compareExpression.Compile();

        _mode = mode;
        _compareGetter = request => getter(request);
        _compareFieldName = GetPropertyName(compareExpression);
    }

    /// <summary>
    /// Creates a field comparison rule from a property name.
    /// </summary>
    ///
    /// <param name="mode">The field comparison mode.</param>
    /// <param name="comparePropertyName">The comparison property name.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public FieldComparisonValidationRule(FieldComparisonMode mode, string comparePropertyName)
    {
        var field = ValidationField<TRequest>.FromPropertyName(comparePropertyName);

        _mode = mode;
        _compareFieldName = field.Name;
        _compareGetter = field.GetValue;
    }

    /// <summary>
    /// Creates a confirmed field comparison rule that uses conventional confirmation names.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public FieldComparisonValidationRule() => _mode = FieldComparisonMode.Confirmed;

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        object? compareValue = _compareGetter is not null
            ? _compareGetter(request)
            : GetConventionalConfirmationValue(request, field);

        bool isValid = _mode switch
        {
            FieldComparisonMode.Same => Equals(value, compareValue),
            FieldComparisonMode.Different => !Equals(value, compareValue),
            FieldComparisonMode.Confirmed => Equals(value, compareValue),
            _ => false
        };

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters()));
    }

    /// <summary>
    /// Gets the validation message key for the configured field comparison mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => _mode switch
    {
        FieldComparisonMode.Same => "validation.same",
        FieldComparisonMode.Different => "validation.different",
        _ => "validation.confirmed"
    };

    /// <summary>
    /// Gets the validation message parameters for the configured field comparison mode.
    /// </summary>
    ///
    /// <returns>The validation message parameters.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private object?[] GetMessageParameters() => _mode == FieldComparisonMode.Confirmed ? [] : [_compareFieldName];

    /// <summary>
    /// Gets the conventional confirmation value for a field.
    /// </summary>
    ///
    /// <param name="request">The request instance.</param>
    /// <param name="field">The field name being validated.</param>
    ///
    /// <returns>The conventional confirmation value when found; otherwise, <c>null</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static object? GetConventionalConfirmationValue(TRequest request, string field)
    {
        string pascalField = ToPascalCase(field);
        Type requestType = typeof(TRequest);

        PropertyInfo? property = requestType.GetProperty($"{pascalField}Confirmation")
            ?? requestType.GetProperty($"Confirm{pascalField}");

        return property?.GetValue(request);
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
    private static string GetPropertyName(Expression<Func<TRequest, TCompare>> expression) => expression.Body switch
    {
        MemberExpression { Member: PropertyInfo propertyInfo } => ToCamelCase(propertyInfo.Name),
        UnaryExpression { Operand: MemberExpression { Member: PropertyInfo unaryPropertyInfo } } => ToCamelCase(unaryPropertyInfo.Name),
        _ => throw new InvalidOperationException("Field comparison rules only support property access expressions.")
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

    /// <summary>
    /// Converts a validation field name to a Pascal-cased property name.
    /// </summary>
    ///
    /// <param name="value">The validation field name.</param>
    ///
    /// <returns>The Pascal-cased value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ToPascalCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsUpper(value[0]))
            return value;

        return char.ToUpperInvariant(value[0]) + value[1..];
    }
}
