namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents a message rendered but not sent, for inspecting what a template produces.
/// </summary>
///
/// <param name="Html">The rendered HTML body, with every interpolated value already encoded.</param>
/// <param name="Text">The plain-text alternative, sent alongside the HTML rather than instead of it.</param>
///
/// <remarks>
/// The package returns the rendered strings rather than writing a file, so it does not take on path validation,
/// permissions, and overwrite semantics for what is one line at the call site.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MailPreview(string Html, string Text);
