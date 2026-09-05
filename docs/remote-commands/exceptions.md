# Exceptions

[`RemoteCommandClient`](./services/remote-command-client) throws four exceptions deriving from `RemoteCommandException`, so a single `catch` covers an unreachable server, a refusal, and a disconnection. Three further failures escape that base type: a framing error raises `InvalidDataException`, an unreadable envelope raises `JsonException`, and a cancelled token propagates as `OperationCanceledException`. The listener raises none of them, because it answers a request it cannot serve with a refused [`RemoteCommandResponse`](./records/remote-command-response) instead.

| Exception                            | Cause                                           | Retry      |
|--------------------------------------|-------------------------------------------------|------------|
| `RemoteCommandUnreachableException`  | The connection could not be opened              | Reasonable |
| `RemoteCommandDisconnectedException` | Closed before a response arrived                | See below  |
| `RemoteCommandProtocolException`     | The frame deserialized to `null`                | Pointless  |
| `RemoteCommandRefusedException`      | The server answered and declined                | Pointless  |

## RemoteCommandUnreachableException

The connection could never be opened, so nothing was sent. The listener is down, the port is wrong, or a network rule dropped the attempt.
`Host` and `Port` carry what was dialled, and the socket failure is the inner exception.

### Type signature

```csharp
public sealed class RemoteCommandUnreachableException(
    string host,
    int port,
    Exception innerException
) : RemoteCommandException
{
    public string Host { get; }
    public int Port { get; }
}
```

## RemoteCommandDisconnectedException

The connection opened but closed before a response arrived. The usual cause is the address not being in [`Whitelisted`](./configuration),
because the listener drops such a client without answering rather than explaining itself.

Only `RemoteCommandUnreachableException` and `RemoteCommandRefusedException` prove the command did not run. The server runs a command and only
then writes its response, so a disconnection may mean it ran and the answer never came back, which is why retrying one is a decision about the
command rather than about the connection.

### Type signature

```csharp
public sealed class RemoteCommandDisconnectedException(
    Exception? innerException = null
) : RemoteCommandException;
```

## RemoteCommandProtocolException

A frame arrived and deserialized to `null`, meaning the server sent the literal `null` where a
[`RemoteCommandResponse`](./records/remote-command-response) envelope belongs. A frame malformed in any other way fails to deserialize and
raises `JsonException` instead, so this type does not cover every wire-format disagreement. Against a server running this package this is a
bug; against another implementation it means the envelope is not being produced.

### Type signature

```csharp
public sealed class RemoteCommandProtocolException(
    string message
) : RemoteCommandException;
```

## RemoteCommandRefusedException

The server answered and declined. `Reason` says what it objected to, and the message is derived from it, so a caller switches on a value and
never parses text.

### Type signature

```csharp
public sealed class RemoteCommandRefusedException(
    RemoteCommandRefusal reason
) : RemoteCommandException
{
    public RemoteCommandRefusal Reason { get; }
}
```

## RemoteCommandRefusal

The reason on a refusal, and the only place the package names one. It travels on the wire as its underlying number, so neither side spells a
code, and a value introduced by a newer server is read as `Other` rather than rejected.

::: tip
A property the payload omits binds to its default, so a message missing an optional field reaches the command with that field at its default. A
property the message marks `required` is different: its absence makes deserialization throw, which is answered with `InvalidMessage` just as
a wrong type is.
:::

| Member               | Value | Meaning                                         |
|----------------------|-------|-------------------------------------------------|
| `Other`              | `0`   | A reason this client has no name for            |
| `MalformedPayload`   | `1`   | The request was not readable as JSON            |
| `MissingCommandName` | `2`   | The request parsed but named no command         |
| `Unauthorized`       | `3`   | The pre-shared key was missing or wrong         |
| `CommandNotFound`    | `4`   | No command is registered under that name        |
| `InvalidMessage`     | `5`   | The data did not fit the command's message type |

### Type signature

```csharp
public enum RemoteCommandRefusal
{
    Other = 0,
    MalformedPayload,
    MissingCommandName,
    Unauthorized,
    CommandNotFound,
    InvalidMessage
}
```
