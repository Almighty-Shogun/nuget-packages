namespace AlmightyShogun.Resend.Utils;

/// <summary>
/// Represents the shared footer and fallback text used by Resend mail templates.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public sealed record EmailTemplateSettings
{
    /// <summary>
    /// Gets the footer copyright text template.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required string CopyrightTextTemplate { get; init; }

    /// <summary>
    /// Gets the text rendered for the footer application link.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required string FooterLinkText { get; init; }

    /// <summary>
    /// Gets the fallback text shown when a recipient can ignore an unexpected email.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required string IgnoreText { get; init; }
}
