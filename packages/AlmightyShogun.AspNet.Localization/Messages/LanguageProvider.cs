using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Provides request-based language preferences using the <c>Accept-Language</c> header.
/// </summary>
///
/// <param name="httpContextAccessor"> The HTTP context accessor. </param>
/// <param name="localizationOptions">The localization settings.</param>
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
