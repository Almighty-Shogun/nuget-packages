using AlmightyShogun.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using IPNetwork = System.Net.IPNetwork;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// The package's startup surface: each helper registers one feature, and none of them registers another's dependencies,
/// so the composition stays explicit and a feature can be adopted without pulling in the rest.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public static class PackageRegistry
{
    /// <summary>
    /// Provides the startup helpers as extensions on the collection. Each registers one feature and nothing else, so
    /// the ones a feature depends on have to be called too; every summary below names what it expects.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The collection that receives the registrations. Every helper returns it, so calls can be chained or written as
    /// separate statements without difference.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers MVC controller services and adds the session context filter globally, so every action can read the
        /// request IP address and User-Agent through <see cref="HttpContextExtensions"/>.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the session context filter configured.</returns>
        ///
        /// <remarks>
        /// Configures <c>MvcOptions</c> rather than calling <c>AddControllers</c>, so it composes with however the
        /// application sets MVC up and can be called before or after it. The filter only runs for actions, so a request
        /// short-circuited earlier still has no stored context.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.1</since>
        public IServiceCollection AddSessionContextFilter() => serviceCollection
            .Configure<MvcOptions>(options => options.Filters.Add<SessionContextFilter>());

        /// <summary>
        /// Registers a named CORS policy using origins from the optional <c>AllowedOrigins</c> configuration section.
        /// </summary>
        ///
        /// <param name="name">
        /// The policy name, which must be passed to <c>UseCors</c> as well; a policy registered here and never named
        /// there is silently inactive.
        /// </param>
        /// <param name="configuration">
        /// The configuration read for the <c>AllowedOrigins</c> string array. An absent section registers a policy that
        /// allows no origin at all, which fails closed rather than open.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the CORS policy configured.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// An origin is the <c>*</c> wildcard. The policy allows credentials, and browsers reject the two together, so
        /// this fails at startup rather than producing a policy every credentialed request would be blocked by.
        /// </exception>
        ///
        /// <remarks>
        /// Thrown while the CORS options are being built, which happens the first time the policy is resolved rather
        /// than during this call.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.1</since>
        public IServiceCollection AddCorsPolicy(string name, IConfiguration configuration) => serviceCollection.AddCors(options =>
        {
            string[] allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

            if (allowedOrigins.Contains("*"))
            {
                throw new InvalidOperationException(
                    "AllowedOrigins contains the '*' wildcard, which browsers reject when credentials are allowed."
                );
            }

            options.AddPolicy(name, policy => policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });

        /// <summary>
        /// Trusts the Cloudflare edge network for forwarded headers, so <c>HttpContext.Connection.RemoteIpAddress</c> and
        /// <c>HttpRequest.IsHttps</c> reflect the original client rather than the proxy.
        /// </summary>
        ///
        /// <param name="clientIpHeader">
        /// The header the client address is read from. Override it only when something between Cloudflare and the
        /// application rewrites the header.
        /// </param>
        /// <param name="additionalNetworks">
        /// Further networks to trust, for an application reached through Cloudflare and then an internal load balancer.
        /// Anything not listed here or published by Cloudflare has its forwarded headers ignored.
        /// </param>
        /// <param name="forwardLimit">
        /// How many proxy hops to walk back. Unset walks every trusted hop, which is correct when the whole chain is
        /// listed; set it to the exact hop count when part of the chain is outside your control.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with forwarded headers configured.</returns>
        ///
        /// <remarks>
        /// Any previously known proxies and networks are cleared, so this replaces rather than extends an existing
        /// forwarded-headers configuration. It only configures the options; <c>UseForwardedHeaders</c> must still run,
        /// and must run before anything that reads the client address.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddCloudflareHeaders(
            string clientIpHeader = Cloudflare.ClientIpHeader,
            IEnumerable<IPNetwork>? additionalNetworks = null,
            int? forwardLimit = null
        ) => serviceCollection.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardedForHeaderName = clientIpHeader;
            options.ForwardLimit = forwardLimit;

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();

            foreach (IPNetwork network in Cloudflare.Networks.Concat(additionalNetworks ?? []))
                options.KnownIPNetworks.Add(network);
        });

        /// <summary>
        /// Registers the standardized error response writer and the <c>HttpErrors</c> settings it reads.
        /// </summary>
        ///
        /// <param name="configuration">
        /// The configuration read for the optional <c>HttpErrors</c> section. Every setting has a default, so an absent
        /// section is valid and leaves the package shape and logging behavior in place.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the response writer registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddHttpErrorResponseWriter(IConfiguration configuration) => serviceCollection
            .AddConfiguration<HttpErrorSettings>(configuration.GetSection("HttpErrors"))
            .AddSingleton<IHttpErrorResponseWriter, HttpErrorResponseWriter>();

        /// <summary>
        /// Registers message resolution: the language provider that negotiates a language from the request, the store
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
            .AddSingleton<IMessageStore, JsonMessageStore>()
            .AddSingleton<IMessageResolver, JsonMessageResolver>();

        /// <summary>
        /// Registers the exception handlers in the order they must run: application exceptions first, then the framework
        /// exceptions that map to their own status code, then the fallback that turns anything else into a <c>500</c>.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the exception handlers registered.</returns>
        ///
        /// <remarks>
        /// Requires <see cref="AddMessageLocalization"/> and <see cref="AddHttpErrorResponseWriter"/>, which this method
        /// does not register, and <c>UseHttpErrorResponses</c> to run the chain. Order is the reason these are registered
        /// together: the fallback handles every exception, so anything registered after it never runs.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddExceptionHandling() => serviceCollection
            .AddExceptionHandler<AppExceptionHandler>()
            .AddExceptionHandler<FrameworkExceptionHandler>()
            .AddExceptionHandler<UnhandledExceptionHandler>();

        /// <summary>
        /// Registers the MVC filter that fills in a standardized body for an error result that carries a status code but
        /// no content, so a bare <c>NotFound()</c> returns a full error response.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the error response filter registered.</returns>
        ///
        /// <remarks>
        /// Requires <see cref="AddMessageLocalization"/> and <see cref="AddHttpErrorResponseWriter"/>, neither of which
        /// this registers. It covers only results MVC produces; an error raised below MVC is left to
        /// <c>UseHttpErrorResponses</c>, which is why the two are normally used together.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddHttpErrorResponseFilter() => serviceCollection
            .AddScoped<HttpErrorResponseFilter>()
            .Configure<MvcOptions>(options => options.Filters.AddService<HttpErrorResponseFilter>());
    }

    /// <summary>
    /// Provides the pipeline helpers as extensions on the builder. Both append middleware, so where they are called
    /// decides what they cover, and neither registers the services its middleware resolves.
    /// </summary>
    ///
    /// <param name="applicationBuilder">
    /// The pipeline the middleware is appended to. Order matters for both helpers, so each should be called at the
    /// point in <c>Program.cs</c> where its middleware belongs rather than grouped for tidiness.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IApplicationBuilder applicationBuilder)
    {
        /// <summary>
        /// Adds the standardized HTTP error response middleware and the exception handler that runs the registered
        /// <see cref="IExceptionHandler"/> chain.
        /// </summary>
        ///
        /// <returns>The <see cref="IApplicationBuilder"/> instance with HTTP error response middleware configured.</returns>
        ///
        /// <remarks>
        /// Call it early, before routing and authentication, so a failure in those still produces the standard body.
        /// The exception handler is given a no-op delegate on purpose: the registered <see cref="IExceptionHandler"/>
        /// chain writes the response, and a real fallback delegate here would write a second one.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IApplicationBuilder UseHttpErrorResponses() => applicationBuilder
            .UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = _ => Task.CompletedTask })
            .UseMiddleware<HttpErrorResponseMiddleware>();

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
