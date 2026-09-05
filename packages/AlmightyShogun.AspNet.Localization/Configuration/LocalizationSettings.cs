using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Configures how localized HTTP messages are resolved. Bound from the optional <c>Localization</c> configuration section;
/// every value has a default, so the section may be absent entirely.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record LocalizationSettings
{
    /// <summary>
    /// The language used when the request asks for none. Set it to a language that actually has message files; see
    /// <see cref="IMessageResolver.ResolveLanguage"/> for where it sits in the fallback chain.
    /// </summary>
    ///
    /// <remarks>
    /// Constrained to the same shape the message provider accepts as a directory name. Blank is rejected separately,
    /// since the pattern check on its own treats an empty value as valid.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required(ErrorMessage = "DefaultLanguage is required.")]
    [RegularExpression(LanguageTag.Pattern, ErrorMessage = "DefaultLanguage must be a language tag such as 'en' or 'nl-BE'.")]
    public string DefaultLanguage { get; init; } = "en";

    /// <summary>
    /// Whether message files are watched and reloaded while the application runs. Disabled by default, and
    /// intended for development, since the files are normally deployed with the application.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool AutomaticReload { get; init; }
}
