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
    /// Gets the language used when the request asks for none, and tried last when nothing the caller did ask for has
    /// any messages at all. Set it to a language that actually has message files: it ends the fallback chain, so a key
    /// missing from whichever language wins is returned to the client verbatim.
    /// </summary>
    ///
    /// <remarks>
    /// Validated at startup against the same shape the message provider accepts, so a malformed value fails the host
    /// rather than resolving no messages for the life of the process.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [RegularExpression(LanguageTag.Pattern, ErrorMessage = "DefaultLanguage must be a language tag such as 'en' or 'nl-BE'.")]
    public string DefaultLanguage { get; init; } = "en";

    /// <summary>
    /// Gets whether message files are watched and reloaded while the application runs. Disabled by default, because the
    /// files are normally deployed with the application and a watcher does not fire reliably on container bind mounts
    /// or network filesystems.
    /// </summary>
    ///
    /// <remarks>
    /// A change drops the cache for every language rather than the file that changed, so the next request for each pays
    /// to reload. Intended for development; the watchers are created once, on the first message resolved.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool AutomaticReload { get; init; }
}
