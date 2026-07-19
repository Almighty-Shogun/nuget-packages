# RemoteCommandHandler

Dependency-injection service for controlling the remote command listener. [`AddRemoteCommands`](../extensions/add-remote-commands) registers the package listener for `IRemoteCommandHandler`, and application code should resolve the interface when it needs to start or stop accepting remote command connections.

Use this service from hosted services, startup code, or controlled shutdown paths. Command discovery still happens through [`RegisterRemoteCommands`](../extensions/register-remote-commands); the handler service controls the TCP listener that receives payloads and dispatches them to registered [`RemoteCommand<T>`](../types/remote-command) implementations.

## Usage

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.RemoteCommands;

public sealed class RemoteCommandWorker(IRemoteCommandHandler commandHandler) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return commandHandler.StartAsync(stoppingToken);
    }
}
```

## StartAsync

Starts the remote command listener through the registered handler. The provided implementation binds to the configured address and port, rejects clients outside the whitelist, reads length-prefixed UTF-8 JSON payloads, and dispatches known commands to registered [`RemoteCommand<T>`](../types/remote-command) implementations.

Use this method after [`AddRemoteCommands`](../extensions/add-remote-commands) and [`RegisterRemoteCommands`](../extensions/register-remote-commands) have been called and the service provider is built. Resolve the handler through `IRemoteCommandHandler` so application code depends on the public DI contract instead of the concrete listener implementation.

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.RemoteCommands;

public sealed class RemoteCommandWorker(IRemoteCommandHandler commandHandler) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return commandHandler.StartAsync(stoppingToken);
    }
}
```

### Type signature

```csharp
public Task StartAsync(CancellationToken cancellationToken = default);
```

## Stop

Stops the active remote command listener through the registered handler. If the listener has not been started, the provided implementation logs an error and returns without throwing.

Use this method during controlled shutdown when the application should stop accepting new remote command connections before the process exits.

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.RemoteCommands;

public sealed class RemoteCommandShutdown(IRemoteCommandHandler commandHandler) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

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
