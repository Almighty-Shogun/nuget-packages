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
    /// <summary>
    /// Runs this filter before any other resource filter, so an unreadable body is refused before anything else inspects the request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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

        context.Result = new HttpErrorResult(ValidationErrorResponseFactory.Create(messageResolver, "validation.invalid-body"));
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
    /// <c>true</c> when the header is missing, unparseable, or claimed by no configured formatter; otherwise <c>false</c> . A formatter
    /// that will not say what it reads counts as claiming the request, so binding decides rather than this filter.
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
    /// Reads the content types a formatter declares, from whichever of the two places it declares them.
    /// </summary>
    ///
    /// <param name="formatter">One registered formatter, of any implementation shape.</param>
    ///
    /// <returns>
    /// The declared content types, or <c>null</c> when the formatter declares none anywhere. Null is not an empty set: it means unknown,
    /// and the caller must not conclude the formatter reads nothing from it.
    /// </returns>
    ///
    /// <remarks>
    /// The collection is typed as <see cref="IInputFormatter"/> , and only the abstract <see cref="InputFormatter"/> carries
    /// <see cref="InputFormatter.SupportedMediaTypes"/> . A formatter implementing the interface directly is asked through
    /// <see cref="IApiRequestFormatMetadataProvider"/> instead, and one implementing neither cannot be interrogated at all.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IEnumerable<string>? GetSupportedMediaTypes(IInputFormatter formatter) => formatter switch
    {
        InputFormatter typed => typed.SupportedMediaTypes,
        IApiRequestFormatMetadataProvider typed => typed.GetSupportedContentTypes(contentType: null, objectType: typeof(object)),
        _ => null
    };

    /// <summary>
    /// Reports whether one declared content type covers the request's, comparing them as media types rather than as strings so a
    /// parameter such as a charset does not decide the outcome.
    /// </summary>
    ///
    /// <param name="mediaType">The request's parsed content type.</param>
    /// <param name="supported">One content type a formatter declared, which may be a wildcard such as <c>text/*</c> .</param>
    ///
    /// <returns><c>true</c> when the declared type covers the request's; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool Reads(MediaTypeHeaderValue mediaType, string supported)
        => MediaTypeHeaderValue.TryParse(supported, out MediaTypeHeaderValue? supportedType) && mediaType.IsSubsetOf(supportedType);
}
