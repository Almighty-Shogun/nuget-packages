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
    /// The rendered HTML body.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Html { get; init; }

    /// <summary>
    /// The plain-text alternative to <see cref="Html"/>, rendered from the same template values.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Text { get; init; }
}
