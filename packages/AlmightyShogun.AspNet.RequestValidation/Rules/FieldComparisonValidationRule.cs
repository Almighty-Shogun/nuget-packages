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
    /// <summary>
    /// Which comparison this rule performs, and with it which message key a failure reports.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly FieldComparisonMode _mode;

    /// <summary>
    /// The field being compared against, by the name a client sees, so a must-differ failure can name the other field in its message.
    /// Always set, the confirmation convention included, since that target is resolved when the rule is built.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly string _compareFieldName;

    /// <summary>
    /// Reads the compared field's value. Always set, including for the confirmation convention, whose target is resolved when the rule is
    /// built rather than per request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Func<TRequest, object?> _compareGetter;

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
        string fieldName = ValidationExpression.GetFieldName(compareExpression);
        Func<TRequest, TCompare> getter = compareExpression.Compile();

        _mode = mode;
        _compareFieldName = fieldName;
        _compareGetter = request => getter(request);
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
    /// Builds the confirmation rule against the property the convention names, which is the declared name with <c>Confirmation</c>
    /// appended or <c>Confirm</c> prefixed.
    /// </summary>
    ///
    /// <param name="declaredPropertyName">
    /// The validated property's name as the type declares it, rather than the name a client sees, since the sibling being looked for is a
    /// property and not a payload field.
    /// </param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// Neither conventional property exists on the request type, so the rule could only ever compare against nothing. Thrown as the rule
    /// is built rather than passing silently whenever the validated value happens to be null too.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public FieldComparisonValidationRule(string declaredPropertyName)
    {
        PropertyInfo property = ResolveConfirmationProperty(declaredPropertyName);

        _mode = FieldComparisonMode.Confirmed;
        _compareFieldName = ValidationFieldName.FromProperty(property);
        _compareGetter = property.GetValue;
    }

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        object? compareValue = _compareGetter(request);

        bool isValid = _mode switch
        {
            FieldComparisonMode.Same => Equals(value, compareValue),
            FieldComparisonMode.Different => !Equals(value, compareValue),
            FieldComparisonMode.Confirmed => Equals(value, compareValue),
            _ => throw new InvalidOperationException($"Unsupported FieldComparisonMode value '{_mode}'.")
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
        FieldComparisonMode.Confirmed => "validation.confirmed",
        _ => throw new InvalidOperationException($"Unsupported FieldComparisonMode value '{_mode}'.")
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
    /// Finds the property the confirmation convention names, so the rule holds a reader rather than searching per request.
    /// </summary>
    ///
    /// <param name="declaredPropertyName">The validated property's declared name, which both conventional spellings are built from.</param>
    ///
    /// <returns>The confirmation property.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// Neither <c>{Name}Confirmation</c> nor <c>Confirm{Name}</c> exists on the request type. Refused here rather than treated as an
    /// absent value, since a rule comparing against a property that does not exist passes whenever the validated value is null.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static PropertyInfo ResolveConfirmationProperty(string declaredPropertyName)
    {
        Type requestType = typeof(TRequest);

        PropertyInfo? property = requestType.GetProperty($"{declaredPropertyName}Confirmation")
                                 ?? requestType.GetProperty($"Confirm{declaredPropertyName}");

        return property ?? throw new ArgumentOutOfRangeException(
            nameof(declaredPropertyName),
            declaredPropertyName,
            $"'{requestType.Name}' declares neither '{declaredPropertyName}Confirmation' nor 'Confirm{declaredPropertyName}', "
            + "so the confirmation rule has nothing to compare against."
        );
    }
}
