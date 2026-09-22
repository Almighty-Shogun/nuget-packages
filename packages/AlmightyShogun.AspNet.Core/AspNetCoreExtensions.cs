using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using IPNetwork = System.Net.IPNetwork;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Provides ASP.NET Core service and middleware extensions.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public static class AspNetCoreExtensions
{
    /// <param name="serviceCollection">
    /// The service collection to configure.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Adds a named CORS policy from the <c>AllowedOrigins</c>, <c>AllowedHeaders</c>, and
        /// <c>AllowedMethods</c> configuration sections.
        /// </summary>
        ///
        /// <param name="name">The policy name.</param>
        /// <param name="configuration">The configuration containing the CORS settings.</param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// <c>AllowedOrigins</c> contains <c>*</c>, which cannot be used with credentials.
        /// </exception>
        ///
        /// <remarks>
        /// Missing headers or methods allow any value, while missing origins allow none.
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
                    "AllowedOrigins contains the '*' wildcard, which browsers reject when credentials are allowed.");

            options.AddPolicy(
                name,
                policy =>
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
        /// Configures forwarded headers for requests proxied through Cloudflare.
        /// </summary>
        ///
        /// <param name="clientIpHeader">The header containing the originating client IP address.</param>
        /// <param name="additionalNetworks">Additional proxy networks to trust.</param>
        /// <param name="forwardLimit">The maximum number of forwarding entries to process.</param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <remarks>
        /// Replaces the existing known proxy and network configuration. <c>UseForwardedHeaders</c> must still be added
        /// to the request pipeline.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
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
        /// Registers the default HTTP error response writer.
        /// </summary>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <remarks>
        /// Existing <see cref="IHttpErrorResponseWriter"/> registrations are preserved.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IServiceCollection AddHttpErrorResponseWriter()
        {
            serviceCollection.TryAddSingleton<IHttpErrorResponseWriter, HttpErrorResponseWriter>();

            return serviceCollection;
        }

        /// <summary>
        /// Registers the framework and fallback exception handlers.
        /// </summary>
        ///
        /// <param name="suppressMapClientErrors">
        /// Whether MVC client-error mapping to <see cref="ProblemDetails"/> is suppressed.
        /// </param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <remarks>
        /// Application exception handlers must be registered before this method because the fallback handler claims
        /// otherwise unhandled exceptions. Requires <see cref="AddHttpErrorResponseWriter"/> and message localization.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IServiceCollection AddExceptionHandling(bool suppressMapClientErrors = true) => serviceCollection
            .Configure<ApiBehaviorOptions>(options => options.SuppressMapClientErrors = suppressMapClientErrors)
            .AddExceptionHandler<FrameworkExceptionHandler>()
            .AddExceptionHandler<UnhandledExceptionHandler>();
    }

  
    /// <param name="applicationBuilder">The application builder.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    extension(IApplicationBuilder applicationBuilder)
    {
        /// <summary>
        /// Adds standardized exception and status-code error responses to the request pipeline.
        /// </summary>
        ///
        /// <returns>The application builder.</returns>
        ///
        /// <remarks>
        /// Add this before middleware whose failures should use the standardized error response. Requires
        /// <see cref="AddExceptionHandling"/>, <see cref="AddHttpErrorResponseWriter"/>, and message localization.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IApplicationBuilder UseHttpErrorResponses() => applicationBuilder
            .UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = _ => Task.CompletedTask })
            .UseStatusCodePages(async statusCodeContext =>
            {
                HttpContext httpContext = statusCodeContext.HttpContext;

                int statusCode = httpContext.Response.StatusCode;
                var messageKey = $"http-error.{statusCode}";
                string description;

                try
                {
                    description = httpContext.RequestServices
                        .GetRequiredService<IMessageResolver>()
                        .Resolve(messageKey);
                }
                catch (Exception)
                {
                    description = messageKey;
                }

                await httpContext.RequestServices.GetRequiredService<IHttpErrorResponseWriter>().WriteAsync(
                    httpContext,
                    statusCode,
                    HttpErrorCodes.FromStatusCode(statusCode),
                    description,
                    httpContext.RequestAborted);
            });
    }
}
