namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// What one rule reported: either success, or the message key describing why it failed.
/// </summary>
///
/// <remarks>
/// Built through <see cref="Success"/> and <see cref="Failure"/> rather than by construction, so a pass carrying a failure message cannot
/// be written: every success is the one shared instance. A failure is not guarded the same way, since <see cref="Failure"/> takes whatever
/// key it is given, an empty one included.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ValidationRuleResult
{
    /// <summary>
    /// Gets whether the validation rule passed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the validation message key returned when the rule fails, and empty when it passed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string Key { get; }

    /// <summary>
    /// Gets the validation message parameters returned when the rule fails, and empty when it passed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public object?[] Parameters { get; }

    /// <summary>
    /// The one success value, shared because a passing rule carries no message and every success is therefore identical.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly ValidationRuleResult _successful = new(true, string.Empty, []);

    /// <summary>
    /// Builds a result. Private so the only results that exist are the ones the two factories below produce.
    /// </summary>
    ///
    /// <param name="isValid">Whether the rule passed.</param>
    /// <param name="key">The message key, empty for a pass.</param>
    /// <param name="parameters">The message parameters, empty for a pass and for a message that takes none.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private ValidationRuleResult(bool isValid, string key, object?[] parameters)
    {
        IsValid = isValid;
        Key = key;
        Parameters = parameters;
    }

    /// <summary>
    /// Reports that the rule passed, carrying no message since none is needed.
    /// </summary>
    ///
    /// <returns>The successful validation rule result, which is one shared instance rather than a new one per call.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationRuleResult Success() => _successful;

    /// <summary>
    /// Reports a failure by message key and parameters, leaving the wording to be resolved when the response is written.
    /// </summary>
    ///
    /// <param name="key">The message key the failure reports, resolved into a sentence only when the response is written.</param>
    /// <param name="parameters">The values substituted into the message template by position, empty when the message takes none.</param>
    ///
    /// <returns>The failed validation rule result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationRuleResult Failure(string key, params object?[] parameters) => new(false, key, parameters);
}
