namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Thrown when no configured application audience can be resolved, whether a request host maps to none, a caller-supplied
/// host does, or there is no request in flight at all, so the request cannot be attributed
/// to an application and must not be served.
/// </summary>
///
/// <param name="host">
/// The host that mapped to no application, or <c>null</c> when there was no request to read one from. Carried so a log
/// line can name the host that needs adding to the mapping.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class UnknownAppException(string? host) : Exception
{
    /// <summary>
    /// Gets the host that mapped to no application, which is the one detail needed to fix the configuration.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Host { get; } = host;
}
