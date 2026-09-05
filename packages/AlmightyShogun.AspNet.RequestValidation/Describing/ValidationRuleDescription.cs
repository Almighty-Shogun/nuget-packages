namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// One rule as published to a client: its name and the arguments it was declared with, ready to drive client-side validation.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record ValidationRuleDescription
{
    /// <summary>
    /// The rule name, taken from the attribute without its <c>Attribute</c> suffix, such as <c>Min</c>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string Rule { get; init; }

    /// <summary>
    /// The values the rule was declared with, in constructor order.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required IReadOnlyList<object?> Arguments { get; init; }
}
