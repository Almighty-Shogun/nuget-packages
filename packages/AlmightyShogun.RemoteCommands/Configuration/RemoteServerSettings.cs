namespace AlmightyShogun.RemoteCommands;

public sealed record RemoteServerSettings
{
    /// <summary>
    /// The IP-address the remote server lives on.
    /// </summary>
    public required string Address { get; init; }
    
    /// <summary>
    /// The TCP port the remote server should listen on.
    /// </summary>
    public int Port { get; init; }
    
    /// <summary>
    /// A list of whitelisted IP-addresses.
    /// </summary>
    public required string[] Whitelisted { get; init; }
    
    /// <summary>
    /// If logs for receiving connections should be enabled.
    /// </summary>
    public required bool EnableReceiveLog { get; init; }
}
