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
    /// Gets the copyright text substituted into the template's copyright placeholder, wherever the application's own base
    /// template puts it, and appended to the plain-text footer. The <c>{app_name}</c> and <c>{app_url}</c> placeholders are
    /// substituted first, and the HTML rendering then encodes the result so a brand name containing markup cannot escape
    /// into the document. The plain-text rendering encodes nothing, having no markup to escape.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string CopyrightTextTemplate { get; init; } = "© {app_name}";

    /// <summary>
    /// Gets the text substituted into the footer link placeholder and appended to the plain-text footer. It takes the same
    /// placeholders as <see cref="CopyrightTextTemplate"/>, and still renders when <see cref="EmailSettings.AppUrl"/> is
    /// unset or was rejected.
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
