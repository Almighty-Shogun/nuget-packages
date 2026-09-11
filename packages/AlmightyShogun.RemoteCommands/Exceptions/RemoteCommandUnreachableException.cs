namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Thrown when a remote command server cannot be reached.
/// </summary>
///
/// <param name="host">The server host.</param>
/// <param name="port">The server port.</param>
/// <param name="innerException">The underlying connection exception.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class RemoteCommandUnreachableException(
    string host,
    int port,
    Exception innerException
) : RemoteCommandException($"The server at {host}:{port} could not be reached.", innerException)
{
    /// <summary>
    ///  Gets the server host.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string Host { get; } = host;

    /// <summary>
    /// Gets the server port.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int Port { get; } = port;
}
