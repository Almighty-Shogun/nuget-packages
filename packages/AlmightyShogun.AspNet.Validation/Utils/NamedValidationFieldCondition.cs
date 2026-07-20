namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Stores a named field condition used by conditional validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class NamedValidationFieldCondition<TRequest>(string propertyName, IReadOnlyList<object?> values) where TRequest : class
{
    public string FieldName => _field.Name;

    /// <summary>
    /// Executes the values text operation.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string ValuesText => ValidationValue.JoinDisplayValues(values);

    private readonly ValidationField<TRequest> _field = ValidationField<TRequest>.FromPropertyName(propertyName);

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
