# RemoteCommandClient

Sends commands to a [`RemoteCommandHandler`](./remote-command-handler) over TCP, using the same framing and JSON options as the listener so the two sides cannot drift apart.

Construct it with the host, port, and the pre-shared `Secret` when the server requires one. It is `IAsyncDisposable` and keeps the connection open, so several commands can be sent sequentially over one socket.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.RemoteCommands;

await using RemoteCommandClient client = new(
    "127.0.0.1",
    30001,
    "a-shared-key"
);

await client.SendAsync<RestartMessage, RestartResponse>(
    "restart",
    new RestartMessage { Force = true },
    cancellationToken
);
```

```csharp [Messages.cs]
public sealed record RestartMessage
{
    public bool Force { get; init; }
}

public sealed record RestartResponse
{
    public required string Status { get; init; }
}
```

:::

::: warning
A refusal, an unreachable server, and a disconnection each throw their own [`RemoteCommandException`](../exceptions) subclass, so the three are distinguishable. Three other failures are not of that family: a framing error raises `InvalidDataException`, an unreadable envelope raises `JsonException`, and cancellation propagates as `OperationCanceledException`. Cancellation disposes the connection first, so the next send opens a clean one; the other two leave it in place, and it is no longer in step with the server.
:::

## SendAsync

Sends one command and waits for its framed response. The overload without `TResponse` returns when the response frame arrives and discards it, for a command whose result is not needed.

```csharp
using AlmightyShogun.RemoteCommands;

await client.SendAsync("reload", new ReloadMessage(), cancellationToken);
```

### Type signature

```csharp
public Task<TResponse?> SendAsync<TMessage, TResponse>(
    string command,
    TMessage message,
    CancellationToken cancellationToken = default
);

public Task SendAsync<TMessage>(
    string command,
    TMessage message,
    CancellationToken cancellationToken = default
);
```
