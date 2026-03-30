namespace AlmightyShogun.RemoteCommands.Configuration;

internal sealed class RemoteConfig : IRemoteConfig
{
    public required string Address { get; set; }
    
    public int Port { get; set; }
    
    public required string[] Whitelisted { get; set; }
    
    public required bool EnableReceiveLog { get; set; }
}
