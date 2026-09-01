using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// What one exception should become on the wire, produced by an <see cref="IExceptionMapper"/> and consumed by the
/// handler that owns it. It is the whole presentation decision for a failure, kept away from the exception so a domain
/// type never names an HTTP status or a message file.
/// </summary>
///
/// <param name="StatusCode">
/// The status the response is sent with, and the value carried into <see cref="HttpErrorResponse.Code"/> so the body
/// repeats it.
/// </param>
/// <param name="Code">
/// The stable machine-readable identifier a client branches on, such as <c>invalid_credentials</c>. Treat it as public
/// API: renaming it breaks consumers without breaking a build.
/// </param>
/// <param name="MessageKey">
/// The key resolved through <see cref="IMessageResolver"/> for the human-readable description. A key no message file
/// defines reaches the client verbatim, so it should read as a key rather than as prose.
/// </param>
/// <param name="MessageParameters">
/// The values substituted into the resolved template by position, as <c>{0}</c> and onwards. Pass an empty list when
/// the message takes none. Too few for the template leaves it unformatted rather than throwing, so a placeholder can
/// reach the client; surplus values are ignored.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ErrorMapping(int StatusCode, string Code, string MessageKey, IReadOnlyList<object?> MessageParameters);
