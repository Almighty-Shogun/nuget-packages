using AlmightyShogun.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Provides extensions for configuring message localization.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public static class LocalizationExtensions
{
    /// <summary>
    /// Provides message localization extensions for an <see cref="IServiceCollection"/>.
    /// </summary>
    ///
    /// <param name="serviceCollection">The service collection.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        ///  Adds message localization services.
        /// </summary>
        ///
        /// <param name="configuration">The application configuration.</param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <remarks>
        /// Configuration is read from the <c>Localization</c> section.
        /// Existing <see cref="ILanguageProvider"/> and <see cref="IMessageResolver"/> registrations are preserved.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IServiceCollection AddMessageLocalization(IConfiguration configuration)
        {
            serviceCollection
                .AddConfiguration<LocalizationSettings>(configuration.GetSection("Localization"))
                .AddHttpContextAccessor()
                .AddSingleton<IMessageProvider, JsonMessageProvider>();

            serviceCollection.TryAddSingleton<ILanguageProvider, LanguageProvider>();
            serviceCollection.TryAddSingleton<IMessageResolver, JsonMessageResolver>();

            return serviceCollection;
        }
    }

    /// <summary>
    /// Provides message localization extensions for an <see cref="IApplicationBuilder"/>.
    /// </summary>
    ///
    /// <param name="applicationBuilder">The application builder.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    extension(IApplicationBuilder applicationBuilder)
    {
        /// <summary>
        /// Adds middleware that sets the response <c>Content-Language</c> header to the resolved language.
        /// </summary>
        ///
        /// <returns>The application builder.</returns>
        ///
        /// <remarks>
        /// Requires services registered by <see cref="AddMessageLocalization"/>.
        /// An existing <c>Content-Language</c> header is preserved.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IApplicationBuilder UseMessageLocalization() => applicationBuilder.UseMiddleware<ContentLanguageMiddleware>();
    }
}
