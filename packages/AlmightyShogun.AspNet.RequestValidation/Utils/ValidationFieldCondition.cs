using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// A condition on another field, addressed by expression, which is the fluent counterpart of the named condition.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationFieldCondition<TRequest, TCompare>(
    Expression<Func<TRequest, TCompare>> expression,
    IReadOnlyList<TCompare?> values
) where TRequest : class
{
    private readonly ValidationField<TRequest> _field = ValidationField<TRequest>.From(expression);

    public string FieldName => _field.Name;

    /// <summary>
    /// Renders the condition's values as the list a message template substitutes, so the failure names what the field was compared against.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string ValuesText => ValidationValue.JoinDisplayValues(values.Select(value => (object?)value));

    /// <summary>
    /// Reads the controlling field and reports whether it equals any configured value, comparing on the rendered form so a number and its
    /// text match.
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
