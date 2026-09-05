# Remote Commands

A TCP listener that receives length-prefixed JSON payloads and dispatches each to a typed command handler, for an internal operational channel or automation hook.

Commands inherit from [`RemoteCommand<T>`](./types/remote-command) and carry [`RemoteCommandAttribute`](./attributes/remote-command-attribute).

## Categories

- [Configuration](./configuration) &mdash; the `RemoteServer` section the listener binds and validates at startup.
- [Exceptions](./exceptions) &mdash; what the client throws when a send fails, one type per reason.
- [Extensions](./extensions/add-remote-commands) &mdash; startup extension methods for registering the listener and command handlers.
- [Attributes](./attributes/remote-command-attribute) &mdash; metadata used to name and describe remote commands.
- [Services](./services/remote-command-handler) &mdash; the listener that answers commands and the client that sends them.
- [Types](./types/remote-command) &mdash; the base type application commands inherit, and the response handle one writes its reply through.
- [Records](./records/remote-command-payload) &mdash; the request and response frames on the wire.

## Quick Example

::: code-group

```csharp [Program.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.RemoteCommands;
using Microsoft.Extensions.DependencyInjection;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands()
    .AddHostedService<RemoteCommandWorker>();

await builder.Build().RunAsync();
```

```csharp [RemoteCommandWorker.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.RemoteCommands;

public sealed class RemoteCommandWorker(
    IRemoteCommandHandler commandHandler
) : BackgroundService
{
    protected override Task ExecuteAsync(
        CancellationToken cancellationToken
    ) => commandHandler.StartAsync(cancellationToken);
}
```

:::
