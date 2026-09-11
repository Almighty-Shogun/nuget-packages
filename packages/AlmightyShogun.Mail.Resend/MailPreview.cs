namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents a rendered email preview.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MailPreview
{
    /// <summary>
    /// Gets the rendered HTML body.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string Html { get; init; }

    /// <summary>
    /// Gets the rendered plain-text body.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string Text { get; init; }
}
