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
    public string CopyrightText { get; init; } = "© {app_name}";

    /// <summary>
    /// The visible text of the footer link. It takes the same placeholders as <see cref="CopyrightText"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string FooterLinkText { get; init; } = "{app_name}";

    /// <summary>
    /// The line telling a recipient to disregard a message they did not expect. Empty by default, and it takes the same
    /// placeholders as <see cref="CopyrightText"/>. Unlike the other two, it is substituted into the HTML body as
    /// written rather than encoded, so it may carry markup such as <c>&lt;strong&gt;</c>, and the plain-text alternative
    /// reduces that markup to text. Because it is injected as markup, it holds only values the application controls.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string IgnoreText { get; init; } = string.Empty;
}
