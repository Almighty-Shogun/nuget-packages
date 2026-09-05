namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// The attribute form of the multi-field presence rule, reading its related fields by name because an attribute cannot hold expressions. An
/// empty value is not skipped: it is what the target mode tests once the trigger has fired.
/// </summary>
///
/// <typeparam name="TRequest">The request type the rule is declared on and reads the watched fields from.</typeparam>
/// <typeparam name="TProperty">The bound property's type, read as an object and tested for presence or emptiness alone.</typeparam>
/// <param name="targetMode">
/// What this field must be once the trigger fires, which also decides whether a watched field counts as present or as non-empty.
/// </param>
/// <param name="triggerMode">Which arrangement of the watched fields makes the requirement apply.</param>
/// <param name="comparePropertyNames">
/// Name the watched fields, resolved once when the rule is built. An empty set passes every value under every trigger.
/// </param>
///
/// <exception cref="InvalidOperationException">
/// One of <paramref name="comparePropertyNames"/> names no public instance property on the request type. Thrown as the rule is built
/// rather than when a request arrives.
/// </exception>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class NamedMultiFieldPresenceValidationRule<TRequest, TProperty>(
    MultiFieldPresenceTargetMode targetMode,
    MultiFieldPresenceTriggerMode triggerMode,
    params string[] comparePropertyNames
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValidationRulePriority Priority => ValidationRulePriority.Required;

    /// <summary>
    /// The fields this rule watches, resolved once when the rule is built so each request only reads their values.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlyList<ValidationField<TRequest>> _fields = ValidationField<TRequest>.FromMany(comparePropertyNames);

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        if (TriggerNeedsFields() && _fields.Count == 0)
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (targetMode == MultiFieldPresenceTargetMode.Prohibits)
        {
            bool isValid = ValidationValue.IsEmpty(value)
                           || _fields.All(compareField => ValidationValue.IsEmpty(compareField.GetValue(request)));

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
            MultiFieldPresenceTargetMode.Prohibits => true,
            _ => throw new InvalidOperationException($"Unsupported MultiFieldPresenceTargetMode value '{targetMode}'.")
        };

        return ValueTask.FromResult(targetIsValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), ValidationField<TRequest>.JoinNames(_fields)));
    }

    /// <summary>
    /// Reports whether the watched fields are in the state that makes this rule apply at all.
    /// </summary>
    ///
    /// <param name="request">The request being validated, so a rule can read another field as well as its own.</param>
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
        MultiFieldPresenceTriggerMode.Prohibits => true,
        _ => throw new InvalidOperationException($"Unsupported MultiFieldPresenceTriggerMode value '{triggerMode}'.")
    };

    /// <summary>
    /// Checks whether a trigger value counts as present for the configured target mode.
    /// </summary>
    ///
    /// <param name="value">
    /// The controlling field's value, tested for presence rather than compared against anything.
    /// </param>
    ///
    /// <returns><c>true</c> when the trigger is present; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool IsTriggerPresent(object? value) => targetMode == MultiFieldPresenceTargetMode.Required
        ? !ValidationValue.IsEmpty(value)
        : ValidationValue.IsPresent(value);

    /// <summary>
    /// Reports whether the trigger needs every watched field rather than any one of them, which decides whether the explicit guard is
    /// needed. An empty field list passes under every trigger; the <c>All</c> ones are the only ones that would be vacuously satisfied and
    /// make the rule apply, so they are the only ones guarded.
    /// </summary>
    ///
    /// <returns><c>true</c> for the <c>WithAll</c> and <c>WithoutAll</c> triggers; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool TriggerNeedsFields()
        => triggerMode is MultiFieldPresenceTriggerMode.WithAll or MultiFieldPresenceTriggerMode.WithoutAll;

    /// <summary>
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
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
        (MultiFieldPresenceTargetMode.Missing, MultiFieldPresenceTriggerMode.WithAll) => "validation.missing.with-all",
        _ => throw new InvalidOperationException(
            $"Unsupported MultiFieldPresenceTargetMode and MultiFieldPresenceTriggerMode pairing: ({targetMode}, {triggerMode})."
        )
    };
}
