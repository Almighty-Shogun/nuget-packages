using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Resolves the message language from the request <c>Accept-Language</c> header, falling back to the configured default
/// when the header is absent or names nothing usable. Registered by <c>AddMessageLocalization</c> as the default
/// <see cref="ILanguageProvider"/>; substitute your own afterwards to negotiate from a cookie or a user profile.
/// </summary>
///
/// <param name="httpContextAccessor">
/// The accessor used to reach the current request. Registered as a singleton, so the context is read per call rather
/// than captured; outside a request there is none, and the default language is used.
/// </param>
/// <param name="localizationOptions">The settings supplying the language used when the request asks for none.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
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
