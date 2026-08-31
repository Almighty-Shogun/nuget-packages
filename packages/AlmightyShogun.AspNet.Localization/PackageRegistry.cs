using AlmightyShogun.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// The package's startup surface: one call registers message resolution, the other adds the middleware that reports the
/// negotiated language, and neither registers the other, so an application that only reads messages skips the middleware.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class PackageRegistry
{
    /// <summary>
    /// Provides the registration helper as an extension on the collection.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The collection that receives the registrations. It is returned, so calls can be chained or written as separate
    /// statements without difference.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers message resolution: the language provider that negotiates a language from the request, the provider
        /// that reads the message files, and the resolver that turns a message key into localized text.
        /// </summary>
        ///
        /// <param name="configuration">
        /// The configuration read for the optional <c>Localization</c> section. Every setting has a default, so an
        /// absent section resolves messages in English with reloading off.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with message localization registered.</returns>
        ///
        /// <remarks>
        /// Each of the three is registered unconditionally, so a custom <see cref="ILanguageProvider"/> must be
        /// substituted after this call rather than before it. Also registers the HTTP context accessor, which the
        /// default provider needs to read the request.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddMessageLocalization(IConfiguration configuration) => serviceCollection
            .AddConfiguration<LocalizationSettings>(configuration.GetSection("Localization"))
            .AddHttpContextAccessor()
            .AddSingleton<ILanguageProvider, LanguageProvider>()
            .AddSingleton<IMessageProvider, JsonMessageProvider>()
            .AddSingleton<IMessageResolver, JsonMessageResolver>();
    }

    /// <summary>
    /// Provides the middleware helper as an extension on the application builder.
    /// </summary>
    ///
    /// <param name="applicationBuilder">
    /// The pipeline the middleware is added to. It is returned, so calls can be chained in the order the middleware runs.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IApplicationBuilder applicationBuilder)
    {
        /// <summary>
        /// Adds the middleware that writes the <c>Content-Language</c> header from the negotiated message language.
        /// </summary>
        ///
        /// <returns>The <see cref="IApplicationBuilder"/> instance with the message localization middleware configured.</returns>
        ///
        /// <remarks>
        /// The header is set from a response callback, so this only needs to run before anything that writes a body.
        /// Requires <see cref="AddMessageLocalization"/>, which it does not register.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IApplicationBuilder UseMessageLocalization() => applicationBuilder.UseMiddleware<ContentLanguageMiddleware>();
    }
}
