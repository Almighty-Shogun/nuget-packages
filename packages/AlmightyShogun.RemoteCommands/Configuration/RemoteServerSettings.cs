using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// The bound <c>RemoteServer</c> section. A port or timeout outside its range is caught while the host starts; an address
/// or whitelist entry that does not parse is caught when the listener is resolved, since neither is a range check.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>3.0.0</since>
public sealed record RemoteServerSettings
{
    /// <summary>
    /// The local address to bind. The default accepts only connections from the same machine; binding a routable
    /// address exposes the listener to everything that can reach it, subject to the whitelist.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    [Required]
    public string Address { get; init; } = "127.0.0.1";

    /// <summary>
    /// The port to bind. Required, and the reason an absent <c>RemoteServer</c> section fails validation rather than
    /// starting a listener on a port nobody chose.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    [Range(1, 65535)]
    public required int Port { get; init; }

    /// <summary>
    /// The addresses allowed to connect, each a bare address or a CIDR range. Deny by default: an empty list matches nothing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public IReadOnlyList<string> Whitelisted { get; init; } = [];

    /// <summary>
    /// Whether each accepted command is logged by name, which covers ordinary traffic only and not refusals.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public bool EnableReceiveLog { get; init; }

    /// <summary>
    /// The optional pre-shared key a client must present. When set, a request without a matching key is refused.
    /// </summary>
    ///
    /// <remarks>
    /// This raises the floor from "anyone who can reach the port from a whitelisted address" to "anyone who also holds
    /// the key". It is not a substitute for transport security: the connection is still plaintext.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Secret { get; init; }

    /// <summary>
    /// The largest request accepted, in bytes. It bounds what the listener reads only, never the size of what it writes back.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int MaxPayloadBytes { get; init; } = 1024 * 1024;

    /// <summary>
    /// How long serving one request may take, in seconds, before it is abandoned. The window covers the command's
    /// own work, not just the framing around it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int ReadTimeout { get; init; } = 30;

    /// <summary>
    /// How long a connection may sit idle between requests, in seconds, before it is closed.
    /// </summary>
    ///
    /// <remarks>
    /// Distinct from <see cref="ReadTimeout"/>, which only bounds a request that has already started. Without an idle
    /// timeout a client that connects and goes quiet holds a connection slot indefinitely.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int IdleTimeout { get; init; } = 120;

    /// <summary>
    /// How many connections are served at once, which bounds the memory and threads the listener spends on connections.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int MaxConcurrentConnections { get; init; } = 100;
}
