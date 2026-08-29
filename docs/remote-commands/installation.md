# Installation

Install `AlmightyShogun.RemoteCommands` in the application that should listen for remote command payloads and dispatch them to typed command handlers. The package targets `net10.0` and uses dependency injection for the listener and discovered commands.

```sh
dotnet add package AlmightyShogun.RemoteCommands
```

## Dependencies

### Package references

- `Microsoft.Extensions.Configuration.Binder` `10.0.11` &mdash; binds the `RemoteServer` configuration section.
- `Microsoft.Extensions.DependencyInjection.Abstractions` `10.0.11` &mdash; the service collection the commands register into.
- `Microsoft.Extensions.Logging.Abstractions` `10.0.11` &mdash; `ILogger<T>`, which the listener writes through.
- `Microsoft.Extensions.Options` `10.0.11` &mdash; reads the bound settings at runtime.

### Project references

- `AlmightyShogun.Utils` &mdash; provides assembly scanning and inherited-type registration helpers.

## Startup Registration

Register the listener services, then scan the assemblies that contain remote command classes. Resolve [`IRemoteCommandHandler`](./services/remote-command-handler) from dependency injection when the application is ready to start or stop the listener.

::: warning
Requires a `RemoteServer` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands();
```
