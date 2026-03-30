using System.Net;

namespace AlmightyShogun.RemoteCommands.Configuration;

public interface IRemoteConfig
{
    public string Address { get; }
    
    public int Port { get; }
    
    public string[] Whitelisted { get; }
    
    public bool EnableReceiveLog { get; }
}
