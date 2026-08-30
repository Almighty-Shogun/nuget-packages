using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Registers the rule cache, the filters, and the response factories as one unit, and configures MVC so a binding failure returns the same
/// shape a rule failure does.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class PackageRegistry
{
    /// <summary>
    /// Provides service-collection extension methods for registering ASP.NET validation services.
    /// </summary>
    ///
    /// <param name="serviceCollection">The service collection used to register validation services.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the validation services, the MVC filters, and the controller integration. The middleware is separate: add it with
        /// <c>UseAspNetValidation</c> on the application builder.
        /// </summary>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <remarks>
        /// Requires <c>AddMessageLocalization</c> from <c>AlmightyShogun.AspNet.Localization</c> , which every failure message is
        /// resolved through.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddAspNetValidation()
        {
            serviceCollection
                .AddSingleton<ValidationResponseWriter>()
                .AddSingleton<IValidationRuleDescriber, ValidationRuleDescriber>()
                .AddSingleton<IValidationResponseFactory, DefaultValidationResponseFactory>()
                .AddSingleton<ValidationRuleCache>()
                .AddScoped<RequestValidator>()
                .AddScoped<RequestBodyValidationFilter>()
                .AddScoped<RequestValidationFilter>()
                .AddScoped<EndpointValidationFilter>()
                .Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true)
                .Configure<ApiBehaviorOptions>(options =>
                {
                    options.InvalidModelStateResponseFactory = ModelStateValidationResponseFactory.Create;
                })
                .AddControllers(options =>
                {
                    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                    options.Filters.AddService<RequestBodyValidationFilter>();
                    options.Filters.AddService<RequestValidationFilter>();
                });

            return serviceCollection;
        }
    }

    /// <summary>
    /// Provides application-builder extension methods for registering ASP.NET validation middleware.
    /// </summary>
    ///
    /// <param name="applicationBuilder">The application builder used to register validation middleware.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IApplicationBuilder applicationBuilder)
    {
        /// <summary>
        /// Adds the middleware that answers an unreadable request body with the standardized error shape.
        /// </summary>
        ///
        /// <returns>The application builder so additional middleware can be chained.</returns>
        ///
        /// <remarks>
        /// Rule failures never reach it, since the filters answer those before an action runs. Call it early, ahead of routing, so a body
        /// the framework could not read is caught wherever it failed.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IApplicationBuilder UseAspNetValidation() => applicationBuilder.UseMiddleware<InvalidRequestBodyMiddleware>();
    }

    /// <summary>
    /// Provides route-handler extension methods for adding validation to minimal API endpoints.
    /// </summary>
    ///
    /// <param name="routeHandlerBuilder">The route handler builder that receives validation.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(RouteHandlerBuilder routeHandlerBuilder)
    {
        /// <summary>
        /// Validates one minimal API endpoint's arguments. Controllers need no equivalent, since the MVC filters are registered globally.
        /// </summary>
        ///
        /// <returns>The route handler builder.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public RouteHandlerBuilder UseAspNetValidation() => routeHandlerBuilder.AddEndpointFilter<EndpointValidationFilter>();
    }

    /// <summary>
    /// Provides route-group extension methods for adding validation to grouped minimal API endpoints.
    /// </summary>
    ///
    /// <param name="routeGroupBuilder">The route group builder that receives validation.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(RouteGroupBuilder routeGroupBuilder)
    {
        /// <summary>
        /// Validates every endpoint in a group, which is usually preferable to repeating the per-endpoint call.
        /// </summary>
        ///
        /// <returns>The route group builder.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public RouteGroupBuilder UseAspNetValidation() => routeGroupBuilder.AddEndpointFilter<EndpointValidationFilter>();
    }
}
