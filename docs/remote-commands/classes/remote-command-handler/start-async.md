---
outline: deep

params:
    - name: cancellationToken
      description: Token used to stop accepting new clients.
      type: CancellationToken
      default: default

returns: A task representing the remote command listener loop.
---

# StartAsync

Starts the TCP listener using the address and port from `RemoteServerSettings`. The method accepts clients until cancellation is requested, then handles each connection by validating the remote IP address against the configured whitelist, reading a length-prefixed UTF-8 JSON payload, and dispatching the payload to a registered remote command.

Use this method after `AddRemoteCommands` and `RegisterRemoteCommands` have been called and the service provider is built. Resolve the handler through `IRemoteCommandHandler` instead of constructing `RemoteCommandHandler` manually.

## Usage

```csharp
using AlmightyShogun.RemoteCommands;
using Microsoft.Extensions.DependencyInjection;

IRemoteCommandHandler handler = serviceProvider.GetRequiredService<IRemoteCommandHandler>();

await handler.StartAsync(CancellationToken.None);
```

<FrontmatterDocs/>

## Type signature

```csharp
public Task StartAsync(CancellationToken cancellationToken = default);
```

## Uses

- [RemoteServerSettings](../../configuration/remote-server-settings/)
