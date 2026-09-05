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

[`AddRemoteCommands`](./extensions/add-remote-commands) binds the `RemoteServer` section and registers the listener behind [`IRemoteCommandHandler`](./services/remote-command-handler), and [`RegisterRemoteCommands`](./extensions/register-remote-commands) discovers the command classes in the calling assembly and registers each as a transient service. Nothing starts listening on its own: resolve the handler and call `StartAsync`, usually from a hosted service.

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands();
```
