namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Thrown when the connection could never be opened, so nothing was sent. The listener is down, the port is wrong, or a
/// network rule dropped the attempt; retrying is reasonable, unlike for a refusal.
/// </summary>
///
/// <param name="host">The host that was dialed, as configured on the client rather than as resolved.</param>
/// <param name="port">The port that was dialed.</param>
/// <param name="innerException">The socket failure underneath, which carries the specific reason.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class RemoteCommandUnreachableException(string host, int port, Exception innerException)
    : RemoteCommandException($"The server at {host}:{port} could not be reached.", innerException)
{
    /// <summary>
    /// Gets the host that was dialed, for logging which endpoint is unreachable when a client is built from settings.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string Host { get; } = host;

    /// <summary>
    /// Gets the port that was dialed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int Port { get; } = port;
}
