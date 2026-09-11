namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents shared email template settings.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public sealed record EmailTemplateSettings
{
    /// <summary>
    /// Gets the footer copyright text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string CopyrightText { get; init; } = "© {app_name}";

    /// <summary>
    /// Gets the footer link text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string FooterLinkText { get; init; } = "{app_name}";

    /// <summary>
    /// Gets the message ignore text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string IgnoreText { get; init; } = string.Empty;
}
