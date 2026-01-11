using System.Net;

namespace AlmightyShogun.RemoteCommands.Configuration;

public interface IRemoteConfig
{
    public string Address { get; }
    public int Port { get; }
    public HashSet<string> Whitelisted { get; }
    public bool EnableReceiveLog { get; }
}
