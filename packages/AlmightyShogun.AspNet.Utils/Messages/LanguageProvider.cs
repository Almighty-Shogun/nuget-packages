using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Resolves the active message language from the current HTTP request.
/// </summary>
///
/// <param name="httpContextAccessor">The HTTP context accessor used to read and write language headers.</param>
/// <param name="configuration">The application configuration used to read the default message language.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class LanguageProvider(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : ILanguageProvider
{
    /// <inheritdoc />
    public string GetLanguage()
    {
        string? language = httpContextAccessor.HttpContext?.Request.GetAcceptLanguage();

        return string.IsNullOrWhiteSpace(language)
            ? HttpMessageConfiguration.GetDefaultLanguage(configuration)
            : language;
    }

    /// <inheritdoc />
    public void SetContentLanguage(string language)
        => httpContextAccessor.HttpContext?.Response.SetContentLanguage(language);
}
