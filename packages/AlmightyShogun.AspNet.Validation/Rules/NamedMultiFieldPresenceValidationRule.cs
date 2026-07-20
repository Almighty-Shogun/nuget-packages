namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates named multi-field presence constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class NamedMultiFieldPresenceValidationRule<TRequest, TProperty>(MultiFieldPresenceTargetMode targetMode, MultiFieldPresenceTriggerMode triggerMode, params string[] comparePropertyNames) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    private readonly IReadOnlyList<ValidationField<TRequest>> _fields = ValidationField<TRequest>.FromMany(comparePropertyNames);

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (TriggerNeedsFields() && _fields.Count == 0)
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (targetMode == MultiFieldPresenceTargetMode.Prohibits)
        {
            bool isValid = ValidationValue.IsEmpty(value) || _fields.All(compareField => ValidationValue.IsEmpty(compareField.GetValue(request)));

            return ValueTask.FromResult(isValid
                ? ValidationRuleResult.Success()
                : ValidationRuleResult.Failure("validation.prohibits", ValidationField<TRequest>.JoinNames(_fields)));
        }

        if (!ShouldValidate(request))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        bool targetIsValid = targetMode switch
        {
            MultiFieldPresenceTargetMode.Required => !ValidationValue.IsEmpty(value),
            MultiFieldPresenceTargetMode.Present => ValidationValue.IsPresent(value),
            MultiFieldPresenceTargetMode.Missing => value is null,
            _ => true
        };

        return ValueTask.FromResult(targetIsValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), ValidationField<TRequest>.JoinNames(_fields)));
    }

    /// <summary>
    /// Checks whether the related fields trigger validation for the target field.
    /// </summary>
    ///
    /// <param name="request">The request instance.</param>
    ///
    /// <returns><c>true</c> when validation should run; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool ShouldValidate(TRequest request) => triggerMode switch
    {
        MultiFieldPresenceTriggerMode.WithAny => _fields.Any(field => IsTriggerPresent(field.GetValue(request))),
        MultiFieldPresenceTriggerMode.WithAll => _fields.All(field => IsTriggerPresent(field.GetValue(request))),
        MultiFieldPresenceTriggerMode.WithoutAny => _fields.Any(field => !IsTriggerPresent(field.GetValue(request))),
        MultiFieldPresenceTriggerMode.WithoutAll => _fields.All(field => !IsTriggerPresent(field.GetValue(request))),
        _ => true
    };

    /// <summary>
    /// Checks whether a trigger value counts as present for the configured target mode.
    /// </summary>
    ///
    /// <param name="value">The trigger value.</param>
    ///
    /// <returns><c>true</c> when the trigger is present; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool IsTriggerPresent(object? value)
        => targetMode == MultiFieldPresenceTargetMode.Required
            ? !ValidationValue.IsEmpty(value) : ValidationValue.IsPresent(value);

    /// <summary>
    /// Checks whether the configured trigger mode requires related fields to exist.
    /// </summary>
    ///
    /// <returns><c>true</c> when related fields are required; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool TriggerNeedsFields()
        => triggerMode is MultiFieldPresenceTriggerMode.WithAll or MultiFieldPresenceTriggerMode.WithoutAll;

    /// <summary>
    /// Gets the validation message key for the configured multi-field presence mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => (targetMode, triggerMode) switch
    {
        (MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithAny) => "validation.required.with",
        (MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithAll) => "validation.required.with-all",
        (MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithoutAny) => "validation.required.without",
        (MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithoutAll) => "validation.required.without-all",
        (MultiFieldPresenceTargetMode.Present, MultiFieldPresenceTriggerMode.WithAny) => "validation.present.with",
        (MultiFieldPresenceTargetMode.Present, MultiFieldPresenceTriggerMode.WithAll) => "validation.present.with-all",
        (MultiFieldPresenceTargetMode.Missing, MultiFieldPresenceTriggerMode.WithAny) => "validation.missing.with",
        _ => "validation.missing.with-all"
    };
}
