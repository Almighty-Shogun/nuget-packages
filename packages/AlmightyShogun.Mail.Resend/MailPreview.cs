namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents a message rendered but not sent, for inspecting what a template produces.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MailPreview
{
    /// <summary>
    /// Gets the rendered HTML body, exactly as a send would submit it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Html { get; init; }

    /// <summary>
    /// Gets the plain-text alternative, sent alongside the HTML rather than instead of it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Text { get; init; }
}
