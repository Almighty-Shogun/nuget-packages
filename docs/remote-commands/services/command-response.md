# CommandResponse

The handle a command writes its reply through, passed to `HandleCommandAsync` on [`RemoteCommand<T>`](../types/remote-command).

Application code depends on `ICommandResponse` rather than a raw stream, so a command cannot break the wire framing by writing to the socket directly.

## Usage

```csharp
using AlmightyShogun.RemoteCommands;

[RemoteCommand("status")]
public sealed class StatusCommand : RemoteCommand<StatusMessage>
{
    public override Task HandleCommandAsync(
        StatusMessage message,
        ICommandResponse response,
        CancellationToken cancellationToken = default
    ) => response.WriteAsync(new { status = "ok" }, cancellationToken);
}
```

## WriteAsync

Writes one framed response. Calling it twice throws, because a second frame would leave the client unable to parse the stream. A command that writes nothing still gets an empty frame sent on its behalf, so the client never waits forever.

```csharp
await response.WriteAsync(new { status = "ok" }, cancellationToken);
```

### Type signature

```csharp
Task WriteAsync<TResponse>(
    TResponse data,
    CancellationToken cancellationToken = default
);
```
