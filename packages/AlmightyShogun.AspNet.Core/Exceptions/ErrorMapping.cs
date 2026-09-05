using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// What one exception should become on the wire, produced by an <see cref="IExceptionMapper"/> and consumed by the
/// handler that owns it. It is the whole presentation decision for a failure, kept away from the exception so a domain
/// type never names an HTTP status or a message file.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ErrorMapping
{
    /// <summary>
    /// The HTTP status this failure should be answered with.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required int StatusCode { get; init; }

    /// <summary>
    /// The stable machine-readable identifier a client branches on, such as <c>invalid_credentials</c>. Treat it as
    /// public API: renaming it breaks consumers without breaking a build.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Code { get; init; }

    /// <summary>
    /// The key resolved through <see cref="IMessageResolver"/> for the human-readable description. It should read
    /// as a key rather than as prose.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string MessageKey { get; init; }

    /// <summary>
    /// The values substituted into the resolved template by position, as <c>{0}</c> and onwards. Pass an empty list
    /// when the message takes none.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required IReadOnlyList<object?> MessageParameters { get; init; }
}
