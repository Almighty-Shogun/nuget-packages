namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// One rule as published to a client: its name and the arguments it was declared with, ready to drive client-side validation.
/// </summary>
///
/// <param name="Rule">The rule name, taken from the attribute without its <c>Attribute</c> suffix, such as <c>Min</c>.</param>
/// <param name="Arguments">The values the rule was declared with, in constructor order.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ValidationRuleDescription(string Rule, IReadOnlyList<object?> Arguments);
