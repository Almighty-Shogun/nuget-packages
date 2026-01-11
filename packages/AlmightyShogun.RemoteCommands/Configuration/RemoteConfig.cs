using System.Net;

namespace AlmightyShogun.RemoteCommands.Configuration;

internal sealed class RemoteConfig : IRemoteConfig
{
    public string Address { get; set; } = IPAddress.Loopback.ToString();
    public int Port { get; set; } = 30001;
    public HashSet<string> Whitelisted { get; } = [IPAddress.Loopback.ToString()];
    public bool EnableReceiveLog { get; } = true;
}
