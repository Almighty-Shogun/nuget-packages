using Microsoft.Extensions.Configuration;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Reads HTTP message configuration values from application configuration.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class HttpMessageConfiguration
{
    /// <summary>
    /// The configuration key used to read the default message language.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const string DefaultLanguageKey = "DefaultLanguage";

    /// <summary>
    /// The fallback language used when configuration does not provide a value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const string _fallbackDefaultLanguage = "en";

    /// <summary>
    /// Gets the configured default message language.
    /// </summary>
    ///
    /// <param name="configuration">The application configuration that may contain the default language.</param>
    ///
    /// <returns>The configured default language, or <c>en</c> when no value is configured.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static string GetDefaultLanguage(IConfiguration configuration)
    {
        string? defaultLanguage = configuration.GetValue<string>(DefaultLanguageKey);

        return string.IsNullOrWhiteSpace(defaultLanguage) ? _fallbackDefaultLanguage : defaultLanguage;
    }
}
