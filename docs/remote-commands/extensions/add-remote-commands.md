---
params:
    - name: configuration
      description: Application configuration containing the `RemoteServer` section.
      type: IConfiguration

returns: The `IServiceCollection` instance with remote command services configured.
---

# AddRemoteCommands

Registers the listener behind [`IRemoteCommandHandler`](../services/remote-command-handler) and binds the [`RemoteServer`](../configuration) section it runs on. The section's data annotations, such as the port and the timeouts, are validated while the host starts. The address and the whitelist entries are parsed when the handler itself is resolved, so a malformed address or a whitelist entry that is neither an address nor a CIDR range surfaces there rather than at host start.

Pair it with [`RegisterRemoteCommands`](./register-remote-commands), which registers the command classes themselves. Neither call reads what the other registered, so they may be written in either order.

## Usage

::: warning
Requires a `RemoteServer` section in application configuration, usually from `appsettings.json`.
:::

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
