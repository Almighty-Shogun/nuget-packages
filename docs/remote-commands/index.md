# Remote Commands

A TCP listener that receives length-prefixed JSON payloads and dispatches each to a typed command handler, for an internal operational channel or automation hook.

Commands inherit from [`RemoteCommand<T>`](./types/remote-command) and carry [`RemoteCommandAttribute`](./attributes/remote-command-attribute).

## Categories

- [Configuration](./configuration) &mdash; the `RemoteServer` section the listener binds and validates at startup.
- [Exceptions](./exceptions) &mdash; what the client throws when a command does not run, one type per reason.
- [Extensions](./extensions/add-remote-commands) &mdash; startup extension methods for registering the listener and command handlers.
- [Attributes](./attributes/remote-command-attribute) &mdash; metadata used to name and describe remote commands.
- [Services](./services/remote-command-handler) &mdash; the listener, the client, and the response handle a command writes through.
- [Types](./types/remote-command) &mdash; the base type application commands inherit.
- [Records](./records/remote-command-payload) &mdash; the request and error frames on the wire.

## Quick Example

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands();
```
