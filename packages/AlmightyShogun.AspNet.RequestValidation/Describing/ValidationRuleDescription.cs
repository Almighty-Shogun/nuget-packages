namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// One rule as published to a client: its name and the arguments it was declared with, ready to drive client-side validation.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ValidationRuleDescription
{
    /// <summary>
    /// Gets the rule name, taken from the attribute without its <c>Attribute</c> suffix, such as <c>Min</c>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Rule { get; init; }

    /// <summary>
    /// Gets the values the rule was declared with, in constructor order.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required IReadOnlyList<object?> Arguments { get; init; }
}
