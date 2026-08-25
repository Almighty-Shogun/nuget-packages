using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using AlmightyShogun.AspNet.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using AlmightyShogun.AspNet.Localization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Rejects a request whose body is missing or unreadable before model binding reports it as a field failure, so an unparseable payload is
/// answered as a bad body rather than as every field being wrong.
/// </summary>
///
/// <param name="messageResolver">The resolver the invalid-body description is read from.</param>
/// <param name="mvcOptions">
/// The MVC configuration, read for its input formatters so the set of readable content types follows what the application registered.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class RequestBodyValidationFilter(
    IMessageResolver messageResolver,
    IOptions<MvcOptions> mvcOptions
) : IAsyncResourceFilter, IOrderedFilter
{
    private const int StatusCode = StatusCodes.Status422UnprocessableEntity;

    public int Order => int.MinValue;

    /// <summary>
    /// Rejects missing, empty, or unsupported request bodies before model binding runs.
    /// </summary>
    ///
    /// <param name="context">The context whose result is replaced when the body cannot be read.</param>
    /// <param name="next">The rest of the pipeline, invoked only when the body is worth binding.</param>
    ///
    /// <returns>A task that completes once the pipeline has run, or immediately when the request was answered here.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        if (!HasBodyParameter(context.ActionDescriptor) || !HasInvalidBody(context.HttpContext.Request))
        {
            await next();

            return;
        }

        context.Result = HttpErrorResult.Create(new HttpErrorResponse
        {
            Code = StatusCode,
            Error = "validation_error",
            ErrorDescription = messageResolver.Resolve("validation.invalid-body", [])
        });
    }

    /// <summary>
    /// Reports whether the action expects a body at all, since an action that does not should not be failed for lacking one.
    /// </summary>
    ///
    /// <param name="actionDescriptor">The action being invoked, inspected for a parameter bound from the body.</param>
    ///
    /// <returns><c>true</c> when the action expects a request body; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// Reports whether the request declared an empty body. Only an explicit zero counts: an absent length means the body is streamed, which
    /// is the normal shape over HTTP/2 since that protocol forbids the chunked-encoding header entirely.
    /// </summary>
    ///
    /// <param name="request">The request whose declared content length is read.</param>
    ///
    /// <returns><c>true</c> when the request body is empty; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasEmptyBody(HttpRequest request) => request.ContentLength == 0;

    /// <summary>
    /// Decides whether to answer now rather than let binding produce a field error for a body that was never readable.
    /// </summary>
    ///
    /// <param name="request">The request whose length and content type are both inspected.</param>
    ///
    /// <returns><c>true</c> when the body is empty or uses an unsupported content type; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool HasInvalidBody(HttpRequest request) => HasEmptyBody(request) || HasUnsupportedContentType(request);

    /// <summary>
    /// Checks whether any registered input formatter can read the request's content type, rather than testing it against a fixed list, so
    /// an application that adds a formatter is not refused by this filter.
    /// </summary>
    ///
    /// <param name="request">The request whose content type is inspected.</param>
    ///
    /// <returns>
    /// <c>true</c> when the header is missing, unparseable, or claimed by no configured formatter; otherwise <c>false</c> .
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool HasUnsupportedContentType(HttpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ContentType))
            return true;

        if (!MediaTypeHeaderValue.TryParse(request.ContentType, out MediaTypeHeaderValue? mediaType))
            return true;

        return !mvcOptions.Value.InputFormatters
            .OfType<InputFormatter>()
            .SelectMany(formatter => formatter.SupportedMediaTypes)
            .Any(supported => MediaTypeHeaderValue.TryParse(supported, out MediaTypeHeaderValue? supportedType)
                              && mediaType.IsSubsetOf(supportedType));
    }
}
