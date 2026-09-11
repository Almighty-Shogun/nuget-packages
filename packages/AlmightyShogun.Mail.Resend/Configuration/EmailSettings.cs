using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents email configuration settings.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public sealed record EmailSettings
{
    /// <summary>
    /// Gets the formatted sender address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string From => string.IsNullOrWhiteSpace(FromName) ? FromEmail : $"{FromName} <{FromEmail}>";

    /// <summary>
    /// Gets the Resend API token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    [Required]
    public required string ApiToken { get; init; }

    /// <summary>
    /// Gets the brand name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string BrandName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the sender email address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    [Required]
    [EmailAddress]
    public required string FromEmail { get; init; }

    /// <summary>
    /// Gets the sender display name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string? FromName { get; init; }

    /// <summary>
    /// Gets the logo URL.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string? LogoUrl { get; init; }

    /// <summary>
    /// Gets the application URL.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string? AppUrl { get; init; }

    /// <summary>
    /// Gets the named application links.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public IReadOnlyDictionary<string, string> Links { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets the shared template settings.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public EmailTemplateSettings Template { get; init; } = new();
}
