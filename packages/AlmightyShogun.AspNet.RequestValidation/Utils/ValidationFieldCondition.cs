using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// A condition on another field, addressed by expression, which is the fluent counterpart of the named condition.
/// </summary>
///
/// <typeparam name="TRequest">The request type the controlling field is read from.</typeparam>
/// <typeparam name="TCompare">The controlling field's type, which the configured values are declared as.</typeparam>
/// <param name="expression">
/// Points at the controlling property, compiled to a reader once when the condition is built rather than on each request.
/// </param>
/// <param name="values">
/// The values the controlling field is compared against by <see cref="Matches"/>, and the list <see cref="ValuesText"/> renders for the
/// failure message. The truthiness checks ignore them entirely.
/// </param>
///
/// <exception cref="ArgumentOutOfRangeException">
/// The expression does not reduce to a property read directly off the request, on the terms
/// <see cref="ValidationExpression.GetProperty"/> sets. Thrown as the condition is built, since the field is resolved then rather than
/// per request.
/// </exception>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationFieldCondition<TRequest, TCompare>(
    Expression<Func<TRequest, TCompare>> expression,
    IReadOnlyList<TCompare?> values
) where TRequest : class
{
    /// <summary>
    /// The field the condition reads, resolved once so each request only fetches its value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ValidationField<TRequest> _field = ValidationField<TRequest>.From(expression);

    /// <summary>
    /// The controlling field's public name, surfaced for the failure message so a client is told which field decided the outcome.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string FieldName => _field.Name;

    /// <summary>
    /// Renders the condition's values as the list a message template substitutes, so the failure names what the field was compared against.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string ValuesText => ValidationDisplay.JoinDisplayValues(values.Select(value => (object?)value));

    /// <summary>
    /// Reads the controlling field and reports whether it equals any configured value, compared as objects, so a number and its text
    /// spelling are not the same value.
    /// </summary>
    ///
    /// <param name="request">The request being validated, so a rule can read another field as well as its own.</param>
    ///
    /// <returns><c>true</c> when the condition matches; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool Matches(TRequest request)
    {
        object? value = _field.GetValue(request);

        return values.Any(expectedValue => Equals(value, expectedValue));
    }

    /// <summary>
    /// Reads the controlling field and reports whether it means yes, which is the truthiness check rather than an equality one.
    /// </summary>
    ///
    /// <param name="request">The request being validated, so a rule can read another field as well as its own.</param>
    ///
    /// <returns><c>true</c> when the condition field is accepted; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsAccepted(TRequest request) => ValidationValue.IsAccepted(_field.GetValue(request));

    /// <summary>
    /// Reads the controlling field and reports whether it means no. Not the negation of the accepted check: a value in neither vocabulary
    /// is neither.
    /// </summary>
    ///
    /// <param name="request">The request being validated, so a rule can read another field as well as its own.</param>
    ///
    /// <returns><c>true</c> when the condition field is declined; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsDeclined(TRequest request) => ValidationValue.IsDeclined(_field.GetValue(request));
}
