using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents the <c>Email</c> configuration section. Its data annotations are validated while the host starts, so a
/// missing token or sender address fails startup instead of the first send.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public sealed record EmailSettings
{
    /// <summary>
    /// Gets the sender as Resend expects it, omitting the display name when none is configured so the value does not
    /// start with a stray space.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string From => string.IsNullOrWhiteSpace(FromName) ? FromEmail : $"{FromName} <{FromEmail}>";

    /// <summary>
    /// Gets the token every send is authenticated with. Required, and checked at startup, so an application that forgets it
    /// never reaches a send to fail on.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    [Required]
    public required string ApiToken { get; init; }

    /// <summary>
    /// Gets the product name substituted into the <c>{app_name}</c> placeholder and the brand slot of the base template.
    /// Left empty it renders as nothing rather than falling back to <see cref="FromName"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string BrandName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the address messages are sent from. It must sit on a domain verified with Resend, which startup validation
    /// cannot check, so an unverified domain surfaces as a failed send rather than a failed start.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    [Required]
    [EmailAddress]
    public required string FromEmail { get; init; }

    /// <summary>
    /// Gets the display name paired with <see cref="FromEmail"/>. Leave it unset to send the bare address, which is what
    /// <see cref="From"/> falls back to.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string? FromName { get; init; }

    /// <summary>
    /// Gets the logo shown in the base template header. It is dropped from the rendered HTML unless it is an absolute URL
    /// on an accepted scheme, because an unsafe value cannot be encoded into a <c>src</c> attribute.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string? LogoUrl { get; init; }

    /// <summary>
    /// Gets the URL behind the footer link and the <c>{app_url}</c> placeholder. Dropped under the same scheme rule as
    /// <see cref="LogoUrl"/>, in which case the placeholder is substituted with nothing and what the footer then looks like
    /// is up to the application's own base template.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string? AppUrl { get; init; }

    /// <summary>
    /// Gets named shared links available to application mail templates.
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
    /// Gets the footer and fallback wording shared by every template, bound from the nested <c>Template</c> section. When
    /// that section is absent each value keeps its own default rather than binding to an empty string.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public EmailTemplateSettings Template { get; init; } = new();
}
