using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Maps this package's exceptions to the responses they produce, so the exceptions themselves stay plain. The mappings
/// cannot be replaced one at a time: an application wanting different ones passes <c>registerExceptionHandler: false</c>
/// to <c>AddJwtAuth</c> and answers these exceptions from a handler of its own.
/// </summary>
///
/// <remarks>
/// Registered by <c>AddJwtAuth</c> whether it also registers the handler chain, since a mapper nothing consults
/// is inert and an application that runs its own handler still wants these mappings available to it.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class JwtAuthExceptionMapper : IExceptionMapper
{
    /// <inheritdoc />
    public ErrorMapping? Map(Exception exception) => exception switch
    {
        MissingUserIdClaimException => new ErrorMapping(
            StatusCodes.Status401Unauthorized,
            "missing_user_id_claim",
            "auth.missing-user-id",
            []
        ),

        MissingRefreshTokenException => new ErrorMapping(
            StatusCodes.Status401Unauthorized,
            "missing_refresh_token",
            "auth.missing-refresh-token",
            []
        ),

        UnknownAppException => new ErrorMapping(
            StatusCodes.Status403Forbidden,
            "unknown_app",
            "auth.unknown-app",
            []
        ),

        _ => null
    };
}
