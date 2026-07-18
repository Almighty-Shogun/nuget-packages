---
fields:
    - name: Address
      description: IP address used by the TCP listener.
      type: string

    - name: Port
      description: TCP port used by the remote command listener.
      type: int
      default: '0'

    - name: Whitelisted
      description: Remote IP addresses allowed to connect to the listener.
      type: string[]

    - name: EnableReceiveLog
      description: Whether the listener should log received and unknown commands.
      type: bool
---

# RemoteServerSettings

Represents the `RemoteServer` configuration section used by Remote Commands. `AddRemoteCommands` binds this record through the package's configuration registration, and `RemoteCommandHandler` receives it through `IOptions<RemoteServerSettings>`.

Application code normally does not create this record manually. Inject `IOptions<RemoteServerSettings>` when a service needs to inspect the configured remote command endpoint or whitelist.

## Usage

The JSON shape is documented on the [configuration page](/remote-commands/configuration). The example below shows how application services can consume the already-bound options.

```csharp
using Microsoft.Extensions.Options;
using AlmightyShogun.RemoteCommands;

public sealed class RemoteCommandDiagnostics(IOptions<RemoteServerSettings> options)
{
    private readonly RemoteServerSettings _settings = options.Value;

    public string Endpoint => $"{_settings.Address}:{_settings.Port}";
}
```

<FrontmatterDocs/>
