namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Represents the TCP listener configuration used by Remote Commands.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>3.0.0</since>
public sealed record RemoteServerSettings
{
    /// <summary>
    /// Gets the IP address the remote command listener binds to.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public required string Address { get; init; }

    /// <summary>
    /// Gets the TCP port the remote command listener should listen on.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public int Port { get; init; }

    /// <summary>
    /// Gets the remote IP addresses allowed to connect to the listener.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public required string[] Whitelisted { get; init; }

    /// <summary>
    /// Gets whether received and unknown command messages should be logged.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public required bool EnableReceiveLog { get; init; }
}
