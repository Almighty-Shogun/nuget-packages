using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Registers the ASP.NET Core helpers provided by the package.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public static class PackageRegistry
{
    /// <summary>
    /// Provides service-collection extension methods for registering ASP.NET utility services.
    /// </summary>
    ///
    /// <param name="serviceCollection">The service collection used to register the ASP.NET functionality.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers MVC controller services and adds the package action filters globally.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with controller action filters configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.1</since>
        public IServiceCollection AddActionFilters()
        {
            serviceCollection.AddControllers(options => options.Filters.Add<SessionContextFilter>());

            return serviceCollection;
        }

        /// <summary>
        /// Registers a named CORS policy using origins from the optional <c>AllowedOrigins</c> configuration section.
        /// </summary>
        ///
        /// <param name="name">The name of the CORS policy to register.</param>
        /// <param name="configuration">The application configuration that may contain the <c>AllowedOrigins</c> string array.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with allowed origins configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.1</since>
        public IServiceCollection AddAllowedOrigins(string name, IConfiguration configuration) => serviceCollection.AddCors(options =>
        {
            string[] allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

            options.AddPolicy(name, policy => policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });

        /// <summary>
        /// Registers standardized HTTP error responses using optional application configuration values.
        /// </summary>
        ///
        /// <param name="configuration">The application configuration that may contain the default message language.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with HTTP error response services configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddHttpErrorResponses(IConfiguration configuration) => serviceCollection
            .AddSingleton(configuration)
            .AddHttpContextAccessor()
            .AddSingleton<ILanguageProvider, LanguageProvider>()
            .AddSingleton<IMessageResolver, JsonMessageResolver>()
            .AddExceptionHandler<HttpErrorExceptionHandler>()
            .AddScoped<HttpErrorResponseFilter>()
            .Configure<MvcOptions>(options => options.Filters.AddService<HttpErrorResponseFilter>());
    }

    /// <summary>
    /// Provides application-builder extension methods for registering ASP.NET utility middleware.
    /// </summary>
    ///
    /// <param name="applicationBuilder">The application builder used to register ASP.NET middleware.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IApplicationBuilder applicationBuilder)
    {
        /// <summary>
        /// Adds standardized HTTP error response middleware to the request pipeline.
        /// </summary>
        ///
        /// <returns>The <see cref="IApplicationBuilder"/> instance with HTTP error response middleware configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IApplicationBuilder UseHttpErrorResponses() => applicationBuilder
            .UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = WriteUnhandledExceptionResponseAsync
            })
            .UseMiddleware<HttpErrorResponseMiddleware>();
    }

    /// <summary>
    /// Writes the standardized fallback response for unhandled exceptions.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    ///
    /// <returns>A task representing the asynchronous write operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static async Task WriteUnhandledExceptionResponseAsync(HttpContext context)
    {
        IMessageResolver messageResolver = context.RequestServices.GetRequiredService<IMessageResolver>();
        HttpErrorResponse response = HttpErrorResponseFactory.Create(StatusCodes.Status500InternalServerError, messageResolver);

        context.Response.ContentLength = null;
        context.Response.StatusCode = response.Code;
        context.Response.ContentType = "application/json; charset=utf-8";

        await context.Response.WriteAsJsonAsync(response, context.RequestAborted);
    }
}
