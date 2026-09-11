using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Configures the remote command server.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>3.0.0</since>
public sealed record RemoteServerSettings
{
    /// <summary>
    /// Gets the local address to bind to.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    [Required]
    public string Address { get; init; } = "127.0.0.1";

    /// <summary>
    /// Gets the port to bind to.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    [Range(1, 65535)]
    public required int Port { get; init; }

    /// <summary>
    /// Gets the addresses allowed to connect, specified as IP addresses or CIDR ranges.
    /// An empty list denies all connections.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public IReadOnlyList<string> Whitelisted { get; init; } = [];

    /// <summary>
    /// Gets whether successfully received commands are logged.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public bool EnableReceiveLog { get; init; }

    /// <summary>
    /// Gets the optional pre-shared key required from clients.
    /// </summary>
    ///
    /// <remarks>
    /// The key does not provide transport security. Connections remain unencrypted.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Secret { get; init; }

    /// <summary>
    /// Gets the maximum request payload size, in bytes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int MaxPayloadBytes { get; init; } = 1024 * 1024;

    /// <summary>
    /// Gets the maximum time, in seconds, allowed to process a request, including command execution.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int ReadTimeout { get; init; } = 30;

    /// <summary>
    /// Gets the maximum time, in seconds, a connection may remain idle between requests.
    /// </summary>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int IdleTimeout { get; init; } = 120;

    /// <summary>
    /// Gets the maximum number of connections served concurrently.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int MaxConcurrentConnections { get; init; } = 100;
}
