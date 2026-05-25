# RemoteCommandHandler

TCP listener and dispatcher for remote commands. The handler binds to the configured address and port, rejects clients outside the whitelist, reads length-prefixed UTF-8 JSON payloads, and dispatches known commands to registered `RemoteCommand<T>` implementations.

`AddRemoteCommands` registers this class for the `IRemoteCommandHandler` interface. Application code should resolve `IRemoteCommandHandler` from dependency injection when it needs to start or stop the listener.

## Usage

```csharp
using AlmightyShogun.RemoteCommands;
using Microsoft.Extensions.DependencyInjection;

IRemoteCommandHandler handler = serviceProvider.GetRequiredService<IRemoteCommandHandler>();

await handler.StartAsync();
```

## Methods

- [StartAsync](./start-async) &mdash; starts the TCP listener and dispatch loop.
- [Stop](./stop) &mdash; stops the active listener.
