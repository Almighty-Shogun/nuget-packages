using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Resolves the message language from the request <c>Accept-Language</c> header, falling back to the configured default
/// when the header is absent or names nothing usable. This is the default <see cref="ILanguageProvider"/> that
/// <see cref="LocalizationExtensions"/> registers, under the substitution rule documented there.
/// </summary>
///
/// <param name="httpContextAccessor">
/// The accessor used to reach the current request. The context is read per call rather than captured; outside a request
/// there is none, and the default language is used.
/// </param>
/// <param name="localizationOptions">The settings supplying the language used when the request asks for none.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class LanguageProvider(
    IHttpContextAccessor httpContextAccessor,
    IOptions<LocalizationSettings> localizationOptions
) : ILanguageProvider
{
    /// <inheritdoc />
    public string GetLanguage()
    {
        string? language = httpContextAccessor.HttpContext?.Request.GetAcceptLanguage();

        return string.IsNullOrWhiteSpace(language) ? localizationOptions.Value.DefaultLanguage : language;
    }

    /// <inheritdoc />
    public IReadOnlyList<string> GetLanguages()
    {
        IReadOnlyList<string>? languages = httpContextAccessor.HttpContext?.Request.GetAcceptLanguages();

        return languages is { Count: > 0 } ? languages : [localizationOptions.Value.DefaultLanguage];
    }
}
