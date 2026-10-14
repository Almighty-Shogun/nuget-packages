using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Maps authentication exceptions to standardized HTTP errors.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
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
