namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// What one rule reported: either success, or the message key describing why it failed.
/// </summary>
///
/// <param name="IsValid">Whether the validation rule passed.</param>
/// <param name="Key">The validation message key returned when the rule fails.</param>
/// <param name="Parameters">The validation message parameters returned when the rule fails.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ValidationRuleResult(bool IsValid, string Key, object?[] Parameters)
{
    /// <summary>
    /// The one success value, shared because a passing rule carries no message and every success is therefore identical.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly ValidationRuleResult Successful = new(true, string.Empty, []);

    /// <summary>
    /// Reports that the rule passed, carrying no message since none is needed.
    /// </summary>
    ///
    /// <returns>The successful validation rule result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationRuleResult Success() => Successful;

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
