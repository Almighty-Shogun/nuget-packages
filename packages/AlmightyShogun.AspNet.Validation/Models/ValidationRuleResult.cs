namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Represents the result of running a validation rule.
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
    private static readonly ValidationRuleResult _successful = new(true, string.Empty, []);

    /// <summary>
    /// Creates a successful validation rule result.
    /// </summary>
    ///
    /// <returns>The successful validation rule result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationRuleResult Success() => _successful;

    /// <summary>
    /// Creates a failed validation rule result.
    /// </summary>
    ///
    /// <param name="key">The validation message key.</param>
    /// <param name="parameters">The validation message parameters.</param>
    ///
    /// <returns>The failed validation rule result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationRuleResult Failure(string key, params object?[] parameters) => new(false, key, parameters);
}
