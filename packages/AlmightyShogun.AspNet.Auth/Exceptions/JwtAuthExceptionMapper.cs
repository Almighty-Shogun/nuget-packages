using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Maps this package's exceptions to the responses they produce, so the exceptions themselves stay plain. The mappings
/// cannot be replaced one at a time: an application wanting different ones passes <c>registerExceptionHandler: false</c>
/// to <c>AddAuth</c> and answers these exceptions from a handler of its own.
/// </summary>
///
/// <remarks>
/// Registered by <c>AddAuth</c> whether or not it also registers the handler, and only under this concrete type. It is
/// <c>internal</c>, so nothing outside this package can resolve it.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class JwtAuthExceptionMapper : IExceptionMapper
{
    /// <inheritdoc />
    public ErrorMapping? Map(Exception exception) => exception switch
    {
        MissingUserIdClaimException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Code = "missing_user_id_claim",
            MessageKey = "auth.missing-user-id",
            MessageParameters = []
        },

        MissingRefreshTokenException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Code = "missing_refresh_token",
            MessageKey = "auth.missing-refresh-token",
            MessageParameters = []
        },

        UnknownAppException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status403Forbidden,
            Code = "unknown_app",
            MessageKey = "auth.unknown-app",
            MessageParameters = []
        },

        _ => null
    };
}
