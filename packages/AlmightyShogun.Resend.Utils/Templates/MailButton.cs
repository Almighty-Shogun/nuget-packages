namespace AlmightyShogun.Resend.Utils;

/// <summary>
/// Represents a call-to-action button rendered inside a mail template.
/// </summary>
///
/// <param name="Label">The visible button label.</param>
/// <param name="Url">The URL the button points to.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public sealed record MailButton(string Label, string Url);
