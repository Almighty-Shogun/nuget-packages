# RemoteCommandHandler

Binds the configured address and port and dispatches each accepted request to the matching [`RemoteCommand<T>`](../types/remote-command). Application code depends on `IRemoteCommandHandler`.

Command classes are discovered separately, by [`RegisterRemoteCommands`](../extensions/register-remote-commands).

## StartAsync

Binds the configured endpoint, refuses connections from outside the whitelist, and serves length-prefixed UTF-8 JSON requests until the token is cancelled or [`Stop`](#stop) is called. A connection is kept open between requests, so one client may run many commands on it.

Only one listener may run at a time. Calling this while one is already running logs an error and returns. A failure to bind, such as the port already being in use, is logged rather than thrown.

::: code-group

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

```csharp [Program.cs]
using AlmightyShogun.RemoteCommands;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands()
    .AddHostedService<RemoteCommandWorker>();
```

:::

### Type signature

```csharp
public Task StartAsync(CancellationToken cancellationToken = default);
```

## Stop

Stops accepting new connections and cancels the ones in flight. Cancellation reaches a running command immediately, so a client waiting on a response may receive nothing. The five-second wait that follows lets the connection tasks unwind; it is not a grace period for finishing the work in progress.

Calling it when no listener is running logs an error and returns, so it is safe from shutdown code that cannot know.

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.RemoteCommands;

public sealed class RemoteCommandShutdown(
    IRemoteCommandHandler commandHandler
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
    {
        commandHandler.Stop();

        return Task.CompletedTask;
    }
}
```

### Type signature

```csharp
public void Stop();
```
