using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// What one exception should become on the wire, produced by an <see cref="IExceptionMapper"/> and consumed by the
/// handler that owns it. It is the whole presentation decision for a failure, kept away from the exception so a domain
/// type never names an HTTP status or a message file.
/// </summary>
///
/// <param name="StatusCode">
/// The status the response is sent with. It also decides the log level, since <c>500</c> and above are logged with the
/// stack trace and anything lower without it.
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
/// the message takes none; a count that disagrees with the template leaves the template unformatted.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ErrorMapping(int StatusCode, string Code, string MessageKey, IReadOnlyList<object?> MessageParameters);
