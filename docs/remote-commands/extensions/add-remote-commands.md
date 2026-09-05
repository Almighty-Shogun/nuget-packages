---
params:
    - name: configuration
      description: Application configuration containing the `RemoteServer` section.
      type: IConfiguration

returns: The `IServiceCollection` instance with remote command services configured.
---

# AddRemoteCommands

Registers the listener behind [`IRemoteCommandHandler`](../services/remote-command-handler) and binds the [`RemoteServer`](../configuration) section it runs on. The section's range annotations, such as the port and the timeouts, are checked while the host starts, while the address and the whitelist entries are parsed only when the handler itself is resolved. Pair it with [`RegisterRemoteCommands`](./register-remote-commands), which registers the command classes themselves.

## Usage

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services.AddRemoteCommands(builder.Configuration);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddRemoteCommands(
    IConfiguration configuration
);
```
