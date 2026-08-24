using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Maps a status code to the snake-case identifier clients branch on, covering every error status the framework names
/// plus <c>425 Too Early</c>. These strings are part of the response contract, so an existing mapping must not change
/// once released, even to correct its wording.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class HttpErrorCodes
{
    /// <summary>
    /// Looks up the identifier a client sees in the <c>error</c> field, for a status the application is about to return.
    /// </summary>
    ///
    /// <param name="statusCode">The status code being returned. Only error codes are mapped; anything else falls through.</param>
    ///
    /// <returns>
    /// The identifier for a known error status, such as <c>not_found</c>, or <c>http_error_{code}</c> for one this map
    /// does not name. The fallback keeps the field populated, so a client can always read a code.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static string FromStatusCode(int statusCode) => statusCode switch
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
