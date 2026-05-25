# Installation

Install `AlmightyShogun.RemoteCommands` in the application that should listen for remote command payloads and dispatch them to typed command handlers. The package targets `net10.0` and uses dependency injection for the listener and discovered commands.

```sh
dotnet add package AlmightyShogun.RemoteCommands
```

## Dependencies

- `Microsoft.Extensions.DependencyInjection.Abstractions` `10.0.8` &mdash; provides service registration APIs.
- `AlmightyShogun.Logging` project reference &mdash; provides logging behavior used by the listener.
- `AlmightyShogun.Utils` project reference &mdash; provides assembly scanning and inherited-type registration helpers.

## Startup Registration

Register the listener services, then scan the assemblies that contain remote command classes. Resolve `IRemoteCommandHandler` from dependency injection when the application is ready to start or stop the listener.

::: warning
Requires a `RemoteServer` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands(typeof(Program).Assembly);
```
