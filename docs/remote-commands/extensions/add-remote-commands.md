---
params:
    - name: configuration
      description: Application configuration containing the `RemoteServer` section.
      type: IConfiguration

returns: The `IServiceCollection` instance with remote command services configured.
---

# AddRemoteCommands

Registers the listener behind [`IRemoteCommandHandler`](../services/remote-command-handler) and binds the [`RemoteServer`](../configuration) section it runs on. The section is validated while the host starts, so a missing port, a malformed address, or a whitelist entry that is neither an address nor a CIDR range stops the application there.

Use this method before registering command classes with [`RegisterRemoteCommands`](./register-remote-commands).

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
