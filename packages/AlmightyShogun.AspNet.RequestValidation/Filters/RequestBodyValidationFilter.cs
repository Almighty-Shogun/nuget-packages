
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Filters;
using AlmightyShogun.AspNet.Localization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Rejects invalid MVC request bodies before model binding, while respecting configured empty-body behavior.
/// </summary>
/// 
/// <param name="messageResolver">Resolves the invalid-body error message.</param>
/// <param name="mvcOptions">Provides MVC input formatters and empty-body settings.</param>
/// 
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class RequestBodyValidationFilter(
    IMessageResolver messageResolver,
    IOptions<MvcOptions> mvcOptions
) : IAsyncResourceFilter, IOrderedFilter
{
    /// <summary>
    /// Runs before other resource filters.
    /// </summary>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int Order => int.MinValue;

    /// <summary>
    /// Rejects disallowed empty bodies and unsupported content types before model binding.
    /// </summary>
    /// 
    /// <param name="context">The current resource filter context.</param>
    /// <param name="next">Executes the remaining resource filter pipeline.</param>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        if (!HasBodyParameter(context.ActionDescriptor) ||
            !HasInvalidBody(context.HttpContext.Request, context.ActionDescriptor))
        {
            await next();
            return;
        }

        context.Result = new HttpErrorResult(ValidationErrorResponseFactory.Create(messageResolver, "validation.invalid-body"));
    }

    /// <summary>
    /// Checks whether the action binds a parameter from the request body.
    /// </summary>
    /// 
    /// <param name="actionDescriptor">The action to inspect.</param>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool HasBodyParameter(ActionDescriptor actionDescriptor)
    {
        foreach (ParameterDescriptor parameter in actionDescriptor.Parameters)
        {
            if (parameter.BindingInfo?.BindingSource == BindingSource.Body)
                return true;

            if (parameter is ControllerParameterDescriptor controllerParameter
                && controllerParameter.ParameterInfo.GetCustomAttributes(typeof(FromBodyAttribute), true).Length > 0)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Checks whether the declared content length is zero.
    /// </summary>
    /// 
    /// <param name="request">The request to inspect.</param>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool HasEmptyBody(HttpRequest request) => request.ContentLength == 0;

    /// <summary>
    /// Checks body validity while respecting MVC's empty-body configuration.
    /// </summary>
    /// 
    /// <param name="request">The request to validate.</param>
    /// <param name="actionDescriptor">The action defining body binding behavior.</param>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private bool HasInvalidBody(HttpRequest request, ActionDescriptor actionDescriptor)
    {
        if (HasEmptyBody(request))
            return !AllowsEmptyBody(actionDescriptor);

        return HasUnsupportedContentType(request);
    }

    /// <summary>
    /// Resolves parameter-level empty-body behavior, falling back to the global MVC setting.
    /// </summary>
    /// 
    /// <param name="actionDescriptor">The action whose body parameter is inspected.</param>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>unreleased</since>
    private bool AllowsEmptyBody(ActionDescriptor actionDescriptor)
    {
        foreach (ParameterDescriptor parameter in actionDescriptor.Parameters)
        {
            if (parameter.BindingInfo?.BindingSource != BindingSource.Body &&
                parameter is not ControllerParameterDescriptor controllerParameterWithAttribute)
            {
                continue;
            }

            EmptyBodyBehavior behavior =
                parameter.BindingInfo?.EmptyBodyBehavior ?? EmptyBodyBehavior.Default;

            if (parameter is ControllerParameterDescriptor controllerParameter)
            {
                FromBodyAttribute? attribute = controllerParameter.ParameterInfo
                    .GetCustomAttributes(typeof(FromBodyAttribute), true)
                    .OfType<FromBodyAttribute>()
                    .FirstOrDefault();

                if (behavior == EmptyBodyBehavior.Default && attribute is not null)
                    behavior = attribute.EmptyBodyBehavior;
            }

            switch (behavior)
            {
                case EmptyBodyBehavior.Allow:
                    return true;

                case EmptyBodyBehavior.Disallow:
                    return false;

                default:
                    return mvcOptions.Value.AllowEmptyInputInBodyModelBinding;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether a registered input formatter supports the request content type.
    /// </summary>
    /// 
    /// <param name="request">The request whose content type is inspected.</param>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private bool HasUnsupportedContentType(HttpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ContentType))
            return true;

        if (!MediaTypeHeaderValue.TryParse(request.ContentType, out MediaTypeHeaderValue? mediaType))
            return true;

        foreach (IInputFormatter formatter in mvcOptions.Value.InputFormatters)
        {
            IEnumerable<string>? supportedMediaTypes = GetSupportedMediaTypes(formatter);

            if (supportedMediaTypes is null)
                return false;

            if (supportedMediaTypes.Any(supported => Reads(mediaType, supported)))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the media types declared by an input formatter, if available.
    /// </summary>
    /// 
    /// <param name="formatter">The formatter to inspect.</param>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static IEnumerable<string>? GetSupportedMediaTypes(IInputFormatter formatter) => formatter switch
    {
        InputFormatter typed => typed.SupportedMediaTypes,
        IApiRequestFormatMetadataProvider typed => typed.GetSupportedContentTypes(contentType: null, objectType: typeof(object)),
        _ => null
    };

    /// <summary>
    /// Checks whether a supported media type covers the request media type.
    /// </summary>
    /// 
    /// <param name="mediaType">The request's media type.</param>
    /// <param name="supported">The formatter's supported media type.</param>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool Reads(MediaTypeHeaderValue mediaType, string supported)
        => MediaTypeHeaderValue.TryParse(supported, out MediaTypeHeaderValue? supportedType) && mediaType.IsSubsetOf(supportedType);
}
