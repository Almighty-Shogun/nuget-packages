using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Creates standardized HTTP error responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class HttpErrorResponseFactory
{
    /// <summary>
    /// Creates a standardized HTTP error response for a status code.
    /// </summary>
    ///
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="messageResolver">The message resolver used to resolve the response description.</param>
    ///
    /// <returns>The standardized HTTP error response.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static HttpErrorResponse Create(int statusCode, IMessageResolver messageResolver)
    {
        return new HttpErrorResponse
        {
            Code = statusCode,
            Error = GetError(statusCode),
            ErrorDescription = messageResolver.Resolve($"http-error.{statusCode}")
        };
    }

    /// <summary>
    /// Creates a standardized HTTP error response for a status code with a custom message key.
    /// </summary>
    ///
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="messageResolver">The message resolver used to resolve the response description.</param>
    /// <param name="message">The message key to resolve for the response description.</param>
    ///
    /// <returns>The standardized HTTP error response.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static HttpErrorResponse Create(int statusCode, IMessageResolver messageResolver, string message)
        => Create(statusCode, messageResolver, message, []);

    /// <summary>
    /// Creates a standardized HTTP error response for a status code with a custom formatted message key.
    /// </summary>
    ///
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="messageResolver">The message resolver used to resolve the response description.</param>
    /// <param name="message">The message key to resolve for the response description.</param>
    /// <param name="parameters">The optional values used to format the resolved message.</param>
    ///
    /// <returns>The standardized HTTP error response.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static HttpErrorResponse Create(int statusCode, IMessageResolver messageResolver, string message, params object?[] parameters)
    {
        string? errorDescription = string.IsNullOrWhiteSpace(message)
            ? null
            : messageResolver.Resolve(message, parameters);

        errorDescription ??= messageResolver.Resolve($"http-error.{statusCode}");

        return new HttpErrorResponse
        {
            Code = statusCode,
            Error = GetError(statusCode),
            ErrorDescription = errorDescription
        };
    }

    /// <summary>
    /// Resolves the stable error name for an HTTP status code.
    /// </summary>
    ///
    /// <param name="statusCode">The HTTP status code.</param>
    ///
    /// <returns>The stable error name.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string GetError(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "bad_request",
        StatusCodes.Status401Unauthorized => "unauthorized",
        StatusCodes.Status402PaymentRequired => "payment_required",
        StatusCodes.Status403Forbidden => "forbidden",
        StatusCodes.Status404NotFound => "not_found",
        StatusCodes.Status405MethodNotAllowed => "method_not_allowed",
        StatusCodes.Status406NotAcceptable => "not_acceptable",
        StatusCodes.Status407ProxyAuthenticationRequired => "proxy_authentication_required",
        StatusCodes.Status408RequestTimeout => "request_timeout",
        StatusCodes.Status409Conflict => "conflict",
        StatusCodes.Status410Gone => "gone",
        StatusCodes.Status411LengthRequired => "length_required",
        StatusCodes.Status412PreconditionFailed => "precondition_failed",
        StatusCodes.Status413PayloadTooLarge => "payload_too_large",
        StatusCodes.Status414UriTooLong => "uri_too_long",
        StatusCodes.Status415UnsupportedMediaType => "unsupported_media_type",
        StatusCodes.Status416RangeNotSatisfiable => "range_not_satisfiable",
        StatusCodes.Status417ExpectationFailed => "expectation_failed",
        StatusCodes.Status418ImATeapot => "im_a_teapot",
        StatusCodes.Status421MisdirectedRequest => "misdirected_request",
        StatusCodes.Status422UnprocessableEntity => "unprocessable_entity",
        StatusCodes.Status423Locked => "locked",
        StatusCodes.Status424FailedDependency => "failed_dependency",
        425 => "too_early",
        StatusCodes.Status426UpgradeRequired => "upgrade_required",
        StatusCodes.Status428PreconditionRequired => "precondition_required",
        StatusCodes.Status429TooManyRequests => "too_many_requests",
        StatusCodes.Status431RequestHeaderFieldsTooLarge => "request_header_fields_too_large",
        StatusCodes.Status451UnavailableForLegalReasons => "unavailable_for_legal_reasons",
        StatusCodes.Status500InternalServerError => "internal_server_error",
        StatusCodes.Status501NotImplemented => "not_implemented",
        StatusCodes.Status502BadGateway => "bad_gateway",
        StatusCodes.Status503ServiceUnavailable => "service_unavailable",
        StatusCodes.Status504GatewayTimeout => "gateway_timeout",
        StatusCodes.Status505HttpVersionNotsupported => "http_version_not_supported",
        StatusCodes.Status506VariantAlsoNegotiates => "variant_also_negotiates",
        StatusCodes.Status507InsufficientStorage => "insufficient_storage",
        StatusCodes.Status508LoopDetected => "loop_detected",
        StatusCodes.Status510NotExtended => "not_extended",
        StatusCodes.Status511NetworkAuthenticationRequired => "network_authentication_required",
        _ => $"http_error_{statusCode}"
    };
}
