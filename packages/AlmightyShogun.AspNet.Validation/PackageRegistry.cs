using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Registers ASP.NET validation package services and filters.
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
        /// Registers ASP.NET validation services, filters, middleware, and controller integration.
        /// </summary>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddAspNetValidation()
        {
            serviceCollection
                .AddExceptionHandler<ValidationExceptionHandler>()
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
        /// Adds validation exception middleware to the request pipeline.
        /// </summary>
        ///
        /// <returns>The application builder so additional middleware can be chained.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IApplicationBuilder UseAspNetValidation()
            => applicationBuilder.UseMiddleware<ValidationExceptionMiddleware>();
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
        /// Adds request validation to a minimal API route handler.
        /// </summary>
        ///
        /// <returns>The route handler builder.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public RouteHandlerBuilder UseAspNetValidation()
            => routeHandlerBuilder.AddEndpointFilter<EndpointValidationFilter>();
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
        /// Adds request validation to a minimal API route group.
        /// </summary>
        ///
        /// <returns>The route group builder.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public RouteGroupBuilder UseAspNetValidation()
            => routeGroupBuilder.AddEndpointFilter<EndpointValidationFilter>();
    }
}
