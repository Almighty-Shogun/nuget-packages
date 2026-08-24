namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// The error body every failure in the application serializes to, so a client can parse one shape regardless of whether
/// the failure came from an exception, a filter, or the pipeline below MVC.
/// </summary>
///
/// <remarks>
/// Serialized with the application's own JSON options, so property casing follows whatever the host configured.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public record HttpErrorResponse
{
    /// <summary>
    /// Gets the status code, repeated in the body so a client that has lost the response headers, as through a logging
    /// or proxy layer, can still tell what happened.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required int Code { get; init; }

    /// <summary>
    /// Gets the machine-readable identifier to branch on, such as <c>not_found</c> or a code an
    /// <see cref="ErrorMapping"/> supplied. This is the field a client should switch on, never the description.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Error { get; init; }

    /// <summary>
    /// Gets the human-readable description, localized to the negotiated language. Holds the unresolved message key when
    /// no message file defines it, and is <c>null</c> only when the error was written without one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? ErrorDescription { get; init; }
}
