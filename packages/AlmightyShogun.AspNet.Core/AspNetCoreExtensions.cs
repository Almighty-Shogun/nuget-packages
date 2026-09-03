using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using IPNetwork = System.Net.IPNetwork;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Core;

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
        /// Registers a named CORS policy using origins from the optional <c>AllowedOrigins</c> configuration section.
        /// </summary>
        ///
        /// <param name="name">
        /// The policy name, which must be passed to <c>UseCors</c> as well; a policy registered here and never named
        /// there is silently inactive.
        /// </param>
        /// <param name="configuration">
        /// The configuration read for the <c>AllowedOrigins</c>, <c>AllowedHeaders</c> and <c>AllowedMethods</c> string
        /// arrays. An absent <c>AllowedOrigins</c> registers a policy that allows no origin at all, which fails closed
        /// rather than open; an absent header or method list allows any of them, which is what the policy did before
        /// either could be configured.
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
        /// <remarks>
        /// Credentials are always allowed, which is what the wildcard check above exists to protect. Narrow the headers
        /// and methods through configuration for a deployment that should not accept any of them; call <c>AddCors</c>
        /// directly for a policy this shape cannot express.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.1</since>
        public IServiceCollection AddCorsPolicy(string name, IConfiguration configuration) => serviceCollection.AddCors(options =>
        {
            string[] allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
            string[] allowedHeaders = configuration.GetSection("AllowedHeaders").Get<string[]>() ?? [];
            string[] allowedMethods = configuration.GetSection("AllowedMethods").Get<string[]>() ?? [];

            if (allowedOrigins.Contains("*"))
                throw new InvalidOperationException(
                    "AllowedOrigins contains the '*' wildcard, which browsers reject when credentials are allowed."
                );

            options.AddPolicy(name, policy =>
            {
                policy.WithOrigins(allowedOrigins).AllowCredentials();

                if (allowedHeaders.Length is 0)
                    policy.AllowAnyHeader();
                else
                    policy.WithHeaders(allowedHeaders);

                if (allowedMethods.Length is 0)
                    policy.AllowAnyMethod();
                else
                    policy.WithMethods(allowedMethods);
            });
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
            string clientIpHeader = CloudflareDefaults.ClientIpHeader,
            IEnumerable<IPNetwork>? additionalNetworks = null,
            int? forwardLimit = null
        ) => serviceCollection.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardedForHeaderName = clientIpHeader;
            options.ForwardLimit = forwardLimit;

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();

            foreach (IPNetwork network in CloudflareDefaults.Networks.Concat(additionalNetworks ?? []))
                options.KnownIPNetworks.Add(network);
        });

        /// <summary>
        /// Registers the standardized error response writer, which the exception handlers and the status code pages
        /// callback produce their body through.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the response writer registered.</returns>
        ///
        /// <remarks>
        /// Takes no configuration: the body shape is fixed, so there is nothing to bind. Register it once, before the
        /// handlers or the pipeline helper that resolve it. Every error body the package writes goes through it, so an
        /// application missing this registration fails when the first error is answered rather than at startup.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddHttpErrorResponseWriter() => serviceCollection
            .AddSingleton<IHttpErrorResponseWriter, HttpErrorResponseWriter>();

        /// <summary>
        /// Registers the two exception handlers this package owns, in the order they must run: the framework exceptions
        /// that map to their own status code, then the fallback that turns anything else into a <c>500</c>. Also decides
        /// whether MVC is allowed to rewrite a bodiless error result into a <c>ProblemDetails</c> body of its own.
        /// </summary>
        ///
        /// <param name="suppressMapClientErrors">
        /// Whether MVC's client-error mapping is turned off. Left on, a controller marked <c>[ApiController]</c> rewrites
        /// a bodiless error result such as a bare <c>NotFound()</c> into <c>ProblemDetails</c>, which
        /// <c>UseHttpErrorResponses</c> then leaves alone. Pass <c>false</c> to keep that, and this package's shape
        /// applies only to errors raised below MVC.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the exception handlers registered.</returns>
        ///
        /// <remarks>
        /// Requires <c>AddMessageLocalization</c> and <see cref="AddHttpErrorResponseWriter"/>, which this method
        /// does not register, and <c>UseHttpErrorResponses</c> to run the chain. It answers nothing an application threw
        /// deliberately: register your own handler ahead of this call, built on <see cref="IExceptionMapper"/>, or every
        /// domain exception becomes a <c>500</c>. Order is the reason these two are registered together: the fallback
        /// claims every exception it is given, so a handler registered after it runs only in the one case the fallback
        /// declines, which is a response that has already started.
        /// </remarks>
        ///
        /// <remarks>
        /// <c>SuppressMapClientErrors</c> is configured here because it decides whether an error raised inside MVC
        /// reaches the client in the same shape as one raised below it. <c>UseStatusCodePages</c> only fills in a
        /// response that has no body, so a <c>ProblemDetails</c> body MVC already wrote survives untouched and the
        /// application answers with two different shapes depending on where the failure came from.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddExceptionHandling(bool suppressMapClientErrors = true) => serviceCollection
            .Configure<ApiBehaviorOptions>(options => options.SuppressMapClientErrors = suppressMapClientErrors)
            .AddExceptionHandler<FrameworkExceptionHandler>()
            .AddExceptionHandler<UnhandledExceptionHandler>();
    }

    /// <summary>
    /// Provides the pipeline helper as an extension on the builder. It appends middleware, so where it is called
    /// decides what it covers, and it registers none of the services that middleware resolves.
    /// </summary>
    ///
    /// <param name="applicationBuilder">
    /// The pipeline the middleware is appended to. Order matters, so this should be called at the point in
    /// <c>Program.cs</c> where the middleware belongs rather than grouped with other registrations for tidiness.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IApplicationBuilder applicationBuilder)
    {
        /// <summary>
        /// Adds the exception handler that runs the registered <see cref="IExceptionHandler"/> chain, and the status
        /// code pages handler that gives any bodiless error response the standardized JSON body.
        /// </summary>
        ///
        /// <returns>The <see cref="IApplicationBuilder"/> instance with HTTP error responses configured.</returns>
        ///
        /// <remarks>
        /// Call it early, before routing and authentication, so a failure in those still produces the standard body.
        /// The exception handler is given a no-op delegate because <c>UseExceptionHandler</c> requires either a
        /// delegate or an exception-handling path, and a path would re-execute the pipeline. It is reached only when no
        /// registered <see cref="IExceptionHandler"/> claimed the exception, which the fallback handler does for
        /// everything except a response that has already started.
        /// </remarks>
        ///
        /// <remarks>
        /// The body is written from the status code pages callback rather than from middleware of this package's own,
        /// so every bodiless error the framework produces is covered by the same code path, including the ones MVC
        /// returns from a bare <c>NotFound()</c>. Requires <c>AddMessageLocalization</c> and
        /// <see cref="AddHttpErrorResponseWriter"/>, both resolved per request from the callback.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IApplicationBuilder UseHttpErrorResponses() => applicationBuilder
            .UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = _ => Task.CompletedTask })
            .UseStatusCodePages(async statusCodeContext =>
            {
                HttpContext httpContext = statusCodeContext.HttpContext;

                int statusCode = httpContext.Response.StatusCode;
                var messageResolver = httpContext.RequestServices.GetRequiredService<IMessageResolver>();

                await httpContext.RequestServices.GetRequiredService<IHttpErrorResponseWriter>().WriteAsync(
                    httpContext,
                    statusCode,
                    HttpErrorCodes.FromStatusCode(statusCode),
                    messageResolver.Resolve($"http-error.{statusCode}"),
                    httpContext.RequestAborted
                );
            });
    }
}
