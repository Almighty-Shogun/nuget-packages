namespace AlmightyShogun.Resend.Utils;

/// <summary>
/// Represents the email configuration used by Resend Utils.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public sealed record EmailSettings
{
    /// <summary>
    /// Gets the Resend API token used to authenticate send requests.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required string ApiToken { get; init; }

    /// <summary>
    /// Gets the product or application name rendered in shared email templates.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required string BrandName { get; init; }

    /// <summary>
    /// Gets the sender email address used for outgoing messages.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required string FromEmail { get; init; }

    /// <summary>
    /// Gets the sender display name used for outgoing messages.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required string FromName { get; init; }

    /// <summary>
    /// Gets the public logo URL rendered in the base email template.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required string LogoUrl { get; init; }

    /// <summary>
    /// Gets the public application URL used by footer links and placeholders.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required string AppUrl { get; init; }

    /// <summary>
    /// Gets named shared links available to application mail templates.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required Dictionary<string, string> Links { get; init; }

    /// <summary>
    /// Gets the nested shared template text settings.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public required EmailTemplateSettings Template { get; init; }
}
