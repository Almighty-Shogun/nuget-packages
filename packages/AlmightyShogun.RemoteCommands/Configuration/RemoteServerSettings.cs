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
    /// Gets the local address to bind. The default accepts only connections from the same machine; binding a routable
    /// address exposes the listener to everything that can reach it, subject to the whitelist.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    [Required]
    public string Address { get; init; } = "127.0.0.1";

    /// <summary>
    /// Gets the port to bind. Required, and the reason an absent <c>RemoteServer</c> section fails validation rather than
    /// starting a listener on a port nobody chose.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    [Range(1, 65535)]
    public required int Port { get; init; }

    /// <summary>
    /// Gets the addresses allowed to connect, each a bare address or a CIDR range. Deny by default: an empty list
    /// matches nothing, so a listener configured without one accepts connections and immediately drops every one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public IReadOnlyList<string> Whitelisted { get; init; } = [];

    /// <summary>
    /// Gets whether each accepted command is logged by name. Refusals are logged either way, so turning this off hides
    /// ordinary traffic rather than problems.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public bool EnableReceiveLog { get; init; }

    /// <summary>
    /// Gets the optional pre-shared key a client must present. When set, a request without a matching key is refused.
    /// </summary>
    ///
    /// <remarks>
    /// This raises the floor from "anyone who can reach the port from a whitelisted address" to "anyone who also holds
    /// the key". It is not a substitute for transport security: the connection is still plaintext.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Secret { get; init; }

    /// <summary>
    /// Gets the largest request accepted, in bytes. Checked against the declared length before a buffer is rented, so an
    /// oversized frame costs nothing to refuse.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(1, int.MaxValue)]
    public int MaxPayloadBytes { get; init; } = 1024 * 1024;

    /// <summary>
    /// Gets how long serving one request may take, in seconds, before it is abandoned. This bounds the command itself, so
    /// a command that outlives it is cancelled mid-flight and the client gets no response.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(1, int.MaxValue)]
    public int ReadTimeout { get; init; } = 30;

    /// <summary>
    /// Gets how long a connection may sit idle between requests, in seconds, before it is closed.
    /// </summary>
    ///
    /// <remarks>
    /// Distinct from <see cref="ReadTimeout"/>, which only bounds a request that has already started. Without an idle
    /// timeout a client that connects and goes quiet holds a connection slot indefinitely.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(1, int.MaxValue)]
    public int IdleTimeout { get; init; } = 120;

    /// <summary>
    /// Gets how many connections are served at once. A further client waits for a slot rather than being refused, so
    /// this bounds memory and thread use without dropping traffic.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(1, int.MaxValue)]
    public int MaxConcurrentConnections { get; init; } = 100;
}
