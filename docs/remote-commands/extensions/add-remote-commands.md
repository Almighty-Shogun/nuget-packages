---
outline: deep

params:
    - name: configuration
      description: Application configuration containing the `RemoteServer` section.
      type: IConfiguration

returns: The `IServiceCollection` instance with remote command services configured.
---

# AddRemoteCommands

Registers the remote command listener services and configuration. The method validates that the `RemoteServer` configuration section exists, binds it to `RemoteServerSettings`, and registers `RemoteCommandHandler` for `IRemoteCommandHandler`.

Use this method before registering command classes with `RegisterRemoteCommands`.

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

## Uses

- [RemoteServerSettings](../configuration/remote-server-settings)
- [RemoteCommandHandler](../classes/remote-command-handler/)
