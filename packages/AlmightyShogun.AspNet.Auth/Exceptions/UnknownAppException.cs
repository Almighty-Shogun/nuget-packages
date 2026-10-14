namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Thrown when an application cannot be resolved from a host.
/// </summary>
/// <param name="host">The unresolved host, or null if unavailable.</param>
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class UnknownAppException(string? host) : Exception
{
    /// <summary>
    /// The host that could not be resolved.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Host { get; } = host;
}
