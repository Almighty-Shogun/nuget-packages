using AlmightyShogun.AspNet.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Runs request body validation filter behavior in the ASP.NET request pipeline.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class RequestBodyValidationFilter(IMessageResolver messageResolver) : IAsyncResourceFilter, IOrderedFilter
{
    private const int StatusCode = StatusCodes.Status422UnprocessableEntity;

    public int Order => int.MinValue;

    /// <summary>
    /// Rejects missing, empty, or unsupported request bodies before model binding runs.
    /// </summary>
    ///
    /// <param name="context">The resource execution context.</param>
    /// <param name="next">The next resource filter delegate.</param>
    ///
    /// <returns>A task representing the asynchronous filter operation.</returns>
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
            ErrorDescription = messageResolver.Resolve("http-error.invalid-body", [])
        });
    }

    /// <summary>
    /// Checks whether an action has a body-bound parameter.
    /// </summary>
    ///
    /// <param name="actionDescriptor">The action descriptor to inspect.</param>
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
    /// Checks whether the request has no body content.
    /// </summary>
    ///
    /// <param name="request">The HTTP request.</param>
    ///
    /// <returns><c>true</c> when the request body is empty; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasEmptyBody(HttpRequest request)
    {
        if (request.ContentLength == 0)
            return true;

        return !request.ContentLength.HasValue && !request.Headers.ContainsKey("Transfer-Encoding");
    }

    /// <summary>
    /// Checks whether the request body should be rejected before model binding.
    /// </summary>
    ///
    /// <param name="request">The HTTP request.</param>
    ///
    /// <returns><c>true</c> when the body is empty or uses an unsupported content type; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasInvalidBody(HttpRequest request)
        => HasEmptyBody(request) || HasUnsupportedContentType(request);

    /// <summary>
    /// Checks whether the request content type is unsupported for JSON body validation.
    /// </summary>
    ///
    /// <param name="request">The HTTP request.</param>
    ///
    /// <returns><c>true</c> when the content type is missing, invalid, or not JSON; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasUnsupportedContentType(HttpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ContentType))
            return true;

        if (!MediaTypeHeaderValue.TryParse(request.ContentType, out MediaTypeHeaderValue? mediaType))
            return true;

        string? value = mediaType.MediaType.Value;

        return value is null
            || (!value.Equals("application/json", StringComparison.OrdinalIgnoreCase)
                && !value.EndsWith("+json", StringComparison.OrdinalIgnoreCase));
    }
}
