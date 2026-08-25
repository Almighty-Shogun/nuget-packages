using System.Reflection;
using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Compares this field against another on the same request, for confirmation pairings and must-differ pairings.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class FieldComparisonValidationRule<TRequest, TProperty, TCompare>
    : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    private readonly FieldComparisonMode _mode;

    private readonly string? _compareFieldName;

    private readonly Func<TRequest, object?>? _compareGetter;

    /// <summary>
    /// Builds the rule against another property named by expression, which is the fluent spelling the compiler checks.
    /// </summary>
    ///
    /// <param name="mode">How this field must relate to the other one, which also decides the message the failure reports.</param>
    /// <param name="compareExpression">Points at the field this one is compared against, so both are read from the same request.</param>
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
    /// Builds the rule against another property named as a string, which is the attribute spelling.
    /// </summary>
    ///
    /// <param name="mode">How this field must relate to the other one, which also decides the message the failure reports.</param>
    /// <param name="comparePropertyName">
    /// Names the field this one is compared against, which is the attribute spelling of the expression form.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public FieldComparisonValidationRule(FieldComparisonMode mode, string comparePropertyName)
    {
        ValidationField<TRequest> field = ValidationField<TRequest>.FromPropertyName(comparePropertyName);

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
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        object? compareValue = _compareGetter is not null ? _compareGetter(request) : GetConventionalConfirmationValue(request, field);

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
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
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
    /// Maps the configured mode onto the values a message template substitutes, so the bounds a rule was built with appear in the sentence
    /// the client reads.
    /// </summary>
    ///
    /// <returns>The validation message parameters.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private object?[] GetMessageParameters() => _mode == FieldComparisonMode.Confirmed ? [] : [_compareFieldName];

    /// <summary>
    /// Reads the field a confirmation defaults to, which is the property's own name with <c>Confirmation</c> appended rather than a name
    /// the caller had to supply.
    /// </summary>
    ///
    /// <param name="request">The request being validated, so a rule can read another field as well as its own.</param>
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
    private static string GetPropertyName(Expression<Func<TRequest, TCompare>> expression) => expression.Body switch
    {
        MemberExpression { Member: PropertyInfo propertyInfo } => ToCamelCase(propertyInfo.Name),
        UnaryExpression { Operand: MemberExpression { Member: PropertyInfo unaryPropertyInfo } } => ToCamelCase(unaryPropertyInfo.Name),
        _ => throw new InvalidOperationException("Field comparison rules only support property access expressions.")
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

    /// <summary>
    /// Converts a public field name back to the property name, which is the inverse of the camel-casing applied when failures are reported.
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
