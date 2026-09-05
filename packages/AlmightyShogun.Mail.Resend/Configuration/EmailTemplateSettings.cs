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
    /// The copyright text for the footer, wherever the application's own base template puts it. It may contain the
    /// <c>{app_name}</c> and <c>{app_url}</c> placeholders.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string CopyrightTextTemplate { get; init; } = "© {app_name}";

    /// <summary>
    /// The visible text of the footer link. It takes the same placeholders as <see cref="CopyrightTextTemplate"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string FooterLinkText { get; init; } = "{app_name}";

    /// <summary>
    /// The line telling a recipient to disregard a message they did not expect. Empty by default, and it takes the same
    /// placeholders as <see cref="CopyrightTextTemplate"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string IgnoreText { get; init; } = string.Empty;
}
