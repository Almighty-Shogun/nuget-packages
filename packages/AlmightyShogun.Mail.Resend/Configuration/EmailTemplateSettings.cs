namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents the footer and fallback wording every rendered template shares, so copy that appears in each message is
/// configured once instead of restated by every template class.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public sealed record EmailTemplateSettings
{
    /// <summary>
    /// Gets the copyright line closing the footer. The <c>{app_name}</c> and <c>{app_url}</c> placeholders are substituted
    /// before the result is HTML encoded, so a brand name containing markup cannot escape into the document.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string CopyrightTextTemplate { get; init; } = "© {app_name}";

    /// <summary>
    /// Gets the label of the footer link pointing at <see cref="EmailSettings.AppUrl"/>. It takes the same placeholders as
    /// <see cref="CopyrightTextTemplate"/>, and still renders when that URL is unset or was rejected.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string FooterLinkText { get; init; } = "{app_name}";

    /// <summary>
    /// Gets the line telling a recipient to disregard a message they did not expect. Empty by default, which drops the
    /// line from the text rendering entirely and substitutes nothing for its HTML placeholder. Whether that leaves an empty
    /// paragraph behind depends on the application's own base template, which this package does not ship.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string IgnoreText { get; init; } = string.Empty;
}
