using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Stores a typed field condition used by conditional validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationFieldCondition<TRequest, TCompare>(Expression<Func<TRequest, TCompare>> expression, IReadOnlyList<TCompare?> values) where TRequest : class
{
    private readonly ValidationField<TRequest> _field = ValidationField<TRequest>.From(expression);

    public string FieldName => _field.Name;

    /// <summary>
    /// Executes the values text operation.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string ValuesText => ValidationValue.JoinDisplayValues(values.Select(value => (object?)value));

    /// <summary>
    /// Checks whether the condition field matches any configured value.
    /// </summary>
    ///
    /// <param name="request">The request instance.</param>
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
    /// Checks whether the condition field is accepted.
    /// </summary>
    ///
    /// <param name="request">The request instance.</param>
    ///
    /// <returns><c>true</c> when the condition field is accepted; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsAccepted(TRequest request) => ValidationValue.IsAccepted(_field.GetValue(request));

    /// <summary>
    /// Checks whether the condition field is declined.
    /// </summary>
    ///
    /// <param name="request">The request instance.</param>
    ///
    /// <returns><c>true</c> when the condition field is declined; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsDeclined(TRequest request) => ValidationValue.IsDeclined(_field.GetValue(request));
}
