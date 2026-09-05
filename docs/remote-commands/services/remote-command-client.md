# RemoteCommandClient

Sends commands to a [`RemoteCommandHandler`](./remote-command-handler) over TCP, using the same framing and JSON options as the listener so the two sides cannot drift apart.

Construct it with the host, port, and the pre-shared `Secret` when the server requires one. It is `IAsyncDisposable` and keeps the connection open, so commands are sent one after another over a single socket, and it is not safe for concurrent use because two callers would interleave frames and each read the other's response.

## SendAsync

Sends one command and waits for the single frame that answers it, opening the connection on the first call and again after any failure that discarded it. The generic overload binds the envelope's data to `TResponse` and returns `default` when the command ran without writing anything of its own. The overload without `TResponse` still waits for the frame and discards it, because the answer has to leave the connection before the next request can be read.

::: code-group

```csharp [Program.cs]
using AlmightyShogun.RemoteCommands;

await using RemoteCommandClient client = new(
    "127.0.0.1",
    30001,
    "a-shared-key"
);

RestartResponse? restart = await client
    .SendAsync<RestartMessage, RestartResponse>(
        "restart",
        new RestartMessage { Force = true },
        cancellationToken
    );

await client.SendAsync("reload", new ReloadMessage(), cancellationToken);
```

```csharp [RestartMessage.cs]
public sealed record RestartMessage
{
    public bool Force { get; init; }
}
```

```csharp [RestartResponse.cs]
public sealed record RestartResponse
{
    public required string Status { get; init; }
}
```

```csharp [ReloadMessage.cs]
public sealed record ReloadMessage;
```

:::

::: warning
A refusal, an unreachable server, and a disconnection each throw their own [`RemoteCommandException`](../exceptions) subclass, so the three are distinguishable. Three other failures are not of that family: a framing error raises `InvalidDataException`, an unreadable envelope raises `JsonException`, and cancellation propagates as `OperationCanceledException`. All three discard the connection, so the next send opens a clean one; a response body that does not bind to `TResponse` raises `JsonException` too but keeps the connection, because the frame was read in full.
:::

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
