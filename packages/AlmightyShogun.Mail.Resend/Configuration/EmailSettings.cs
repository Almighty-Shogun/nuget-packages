using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents the <c>Email</c> configuration section. Its data annotations are validated while the host starts, so a
/// missing token or sender address fails startup instead of the first send.
/// </summary>
///
/// <remarks>
/// A URL configured here that is not absolute, or that uses a scheme other than http, https, or mailto, is dropped from the
/// rendered message rather than failing the send, so a mistyped one costs a logo or a footer link and nothing more.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public sealed record EmailSettings
{
    /// <summary>
    /// The sender, as <c>Name &lt;address&gt;</c> when a display name is configured and as the bare address otherwise,
    /// so an unset name does not leave the value starting with a stray space.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string From => string.IsNullOrWhiteSpace(FromName) ? FromEmail : $"{FromName} <{FromEmail}>";

    /// <summary>
    /// The token every send is authenticated with.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    [Required]
    public required string ApiToken { get; init; }

    /// <summary>
    /// The product name substituted into the <c>{app_name}</c> placeholder and the brand slot of the base template.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string BrandName { get; init; } = string.Empty;

    /// <summary>
    /// The address messages are sent from. The annotations check only that a value is present and shaped like an
    /// address, so anything Resend requires of a sender beyond that goes unchecked here.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    [Required]
    [EmailAddress]
    public required string FromEmail { get; init; }

    /// <summary>
    /// The display name paired with <see cref="FromEmail"/>. Leave it unset to send the bare address, which is what
    /// <see cref="From"/> falls back to.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string? FromName { get; init; }

    /// <summary>
    /// The logo URL substituted into the template's logo placeholder, wherever the application's own base template
    /// puts it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string? LogoUrl { get; init; }

    /// <summary>
    /// The URL behind the footer link and the <c>{app_url}</c> placeholder.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string? AppUrl { get; init; }

    /// <summary>
    /// Named shared links available to application mail templates.
    /// </summary>
    ///
    /// <remarks>
    /// The package never reads this. It exists so an application can keep its shared link set alongside the rest of its
    /// mail configuration and read it through <c>IOptions&lt;EmailSettings&gt;</c>.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public IReadOnlyDictionary<string, string> Links { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// The footer and fallback wording shared by every template, bound from the nested <c>Template</c> section.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public EmailTemplateSettings Template { get; init; } = new();
}
