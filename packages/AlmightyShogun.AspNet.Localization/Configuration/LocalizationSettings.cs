using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Configures message localization.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record LocalizationSettings
{
    /// <summary>
    /// Gets the default language used for message resolution.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required(ErrorMessage = "DefaultLanguage is required.")]
    [RegularExpression(LanguageTag.Pattern, ErrorMessage = "DefaultLanguage must be a language tag such as 'en' or 'nl-BE'.")]
    public string DefaultLanguage { get; init; } = "en";

    /// <summary>
    /// Gets whether message files are automatically reloaded when they change.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool AutomaticReload { get; init; }
}
