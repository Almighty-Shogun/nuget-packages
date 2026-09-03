using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Registers the rule cache, the validators, and the filters as one unit, and configures MVC so a binding failure returns the same shape a
/// rule failure does.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class ValidationExtensions
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
        /// Registers the validation services and scans the calling assembly for validators, which is the usual case when the requests
        /// live in the startup project. Reach for the overload taking assemblies when they do not.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the validation services registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddAspNetValidation() => serviceCollection.AddAspNetValidation([Assembly.GetCallingAssembly()]);

        /// <summary>
        /// Registers the validation services, the MVC filters, and the validators declared in the given assemblies. The middleware is
        /// separate: add it with <c>UseAspNetValidation</c> on the application builder.
        /// </summary>
        ///
        /// <param name="assemblies">
        /// The assemblies to scan for <see cref="Validator{TRequest}"/> subclasses, in the order they should be searched. An empty array
        /// finds no validators; the overload taking no assembly at all is the one that falls back to the calling assembly.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the validation services registered.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// Two validators cover the same request type, a validator has no public parameterless constructor, or a request type's
        /// attribute rules cannot be built. Raised here so the offending class is named at startup rather than on whichever request
        /// first reaches it.
        /// </exception>
        ///
        /// <remarks>
        /// Requires <c>AddMessageLocalization</c> from <c>AlmightyShogun.AspNet.Localization</c> , which every failure message is
        /// resolved through.
        ///
        /// MVC is configured rather than added. The filters and the model-state response take effect if the application registers
        /// controllers, whether it does so before or after this call, and cost nothing if it never does, so a minimal API application
        /// does not acquire the controller stack by asking for validation.
        ///
        /// Two framework options are set deliberately, and both are load-bearing rather than incidental.
        /// <c>RouteHandlerOptions.ThrowOnBadRequest</c> is what turns a minimal API binding failure into a
        /// <see cref="BadHttpRequestException"/> , which is the only form <c>InvalidRequestBodyMiddleware</c> can catch; without it such
        /// a request is answered with the framework's own empty response instead of this package's envelope.
        /// <c>MvcOptions.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes</c> stops MVC synthesising a required rule on
        /// every non-nullable reference property, which would report through model state and pre-empt this package's own presence rules
        /// and their localized messages.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddAspNetValidation(Assembly[] assemblies) => serviceCollection
            .AddSingleton<ValidationResponseWriter>()
            .AddSingleton<IValidationRuleDescriber, ValidationRuleDescriber>()
            .AddSingleton(new ValidatorRegistry(assemblies))
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
            .Configure<MvcOptions>(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                options.Filters.AddService<RequestBodyValidationFilter>();
                options.Filters.AddService<RequestValidationFilter>();
            });
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
