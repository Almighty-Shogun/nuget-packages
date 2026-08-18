namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Marks an exception that carries everything needed to produce a standardized error response. Implement it on a domain
/// exception so the throw site names the failure rather than an HTTP status code.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAppException
{
    /// <summary>
    /// Gets the HTTP status code the response should return. It also decides the log level: <c>500</c> and above are
    /// logged as errors with the stack trace, anything lower as a warning without it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    int StatusCode { get; }

    /// <summary>
    /// Gets the stable machine-readable error identifier, such as <c>invalid_credentials</c>. Clients branch on this,
    /// so it should not change once released.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string Code { get; }

    /// <summary>
    /// Gets the key looked up through <see cref="IMessageResolver"/> to produce the human-readable description. A key
    /// with no message file entry is returned to the client verbatim, so it should read as a key and not as prose.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string MessageKey { get; }

    /// <summary>
    /// Gets the values substituted into the resolved message by position, as <c>{0}</c> and onwards. Return an empty
    /// array when the message takes none; a count that does not match the template leaves the template unformatted.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    object?[] MessageParameters { get; }
}
