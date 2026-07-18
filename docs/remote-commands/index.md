# Remote Commands

Adds a TCP-based remote command listener with attribute-discovered command handlers. The package reads listener settings from configuration, registers command handlers through dependency injection, receives JSON payloads over a network stream, and dispatches each command to a typed handler.

Use this package when an application needs a small internal command channel for operational commands, automation hooks, or local network integrations. Commands are implemented as classes that inherit from `RemoteCommand<T>` and are marked with `RemoteCommandAttribute`.

## Categories

- [Configuration](./configuration/remote-server-settings) &mdash; public configuration values bound from application configuration.
- [Extensions](./extensions/add-remote-commands) &mdash; startup extension methods for registering the listener and command handlers.
- [Attributes](./attributes/remote-command-attribute) &mdash; metadata used to name and describe remote commands.
- [Services](./services/remote-command-handler) &mdash; dependency-injection services used to start and stop the listener.
- [Types](./types/remote-command) &mdash; base types used by application-defined remote commands.

## Quick Example

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands(typeof(Program).Assembly);
```
