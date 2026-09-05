# CommandResponse

The handle a command writes its reply through, passed to `HandleCommandAsync` on [`RemoteCommand<T>`](../types/remote-command).

Application code depends on `ICommandResponse` rather than a raw stream, so a command cannot break the wire framing by writing to the socket directly.

## WriteAsync

Serializes the value, wraps it in a [`RemoteCommandResponse`](../records/remote-command-response) envelope, and writes that one frame to the connection. Calling it twice throws `InvalidOperationException`, because the protocol carries one frame per request and a second would be read as the answer to whatever the client sends next. The slot is claimed before the frame goes out, so a write that fails still spends it. Calling it at all is optional: a command that returns without writing is answered with an empty envelope on its behalf, so the client is never left waiting.

::: code-group

```csharp [StatusCommand.cs]
using AlmightyShogun.RemoteCommands;

[RemoteCommand("status")]
public sealed class StatusCommand : RemoteCommand<StatusMessage>
{
    public override Task HandleCommandAsync(
        StatusMessage message,
        ICommandResponse response,
        CancellationToken cancellationToken
    ) => response.WriteAsync(new { status = "ok" }, cancellationToken);
}
```

```csharp [StatusMessage.cs]
public sealed record StatusMessage;
```

:::

### Type signature

```csharp
public Task WriteAsync<TResponse>(
    TResponse data,
    CancellationToken cancellationToken = default
);
```
