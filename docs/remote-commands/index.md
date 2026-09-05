# Remote Commands

A TCP listener that receives length-prefixed JSON payloads and dispatches each to a typed command handler, for an internal operational channel or automation hook.

A command inherits [`RemoteCommand<T>`](./types/remote-command) and carries [`RemoteCommandAttribute`](./attributes/remote-command-attribute), with `T` the message record the payload binds to. Its reply is written through [`ICommandResponse`](./types/command-response), so a command never touches the socket and the client reads one frame per request.

Adopting it is one registration, one assembly scan, and a hosted service that runs the listener. The listener answers a request it cannot serve with a refused [`RemoteCommandResponse`](./records/remote-command-response) rather than throwing, so the failures a caller has to handle live on [`RemoteCommandClient`](./services/remote-command-client) instead.

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

```csharp [DrainCommand.cs]
using AlmightyShogun.RemoteCommands;

[RemoteCommand("drain", "Stops accepting work and reports what is still in flight.")]
public sealed class DrainCommand(WorkQueue queue) : RemoteCommand<DrainMessage>
{
    public override async Task HandleCommandAsync(
        DrainMessage message,
        ICommandResponse response,
        CancellationToken cancellationToken
    )
    {
        int remaining = await queue.DrainAsync(message.Reason, cancellationToken);

        await response.WriteAsync(
            new DrainResponse(remaining, DateTimeOffset.UtcNow),
            cancellationToken
        );
    }
}
```

```csharp [DrainMessage.cs]
public sealed record DrainMessage
{
    public required string Reason { get; init; }
}
```

```csharp [DrainResponse.cs]
public sealed record DrainResponse(
    int Remaining,
    DateTimeOffset DrainedAt
);
```

:::
