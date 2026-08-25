---
params:
    - name: assemblies
      description: The assemblies to scan, in the order they should be searched. An empty array registers nothing; the overload taking no assembly is the one that falls back to the calling assembly.
      type: Assembly[]

returns: The `IServiceCollection` instance with discovered remote command classes registered.
---

# RegisterRemoteCommands

Registers remote command classes from one or more assemblies as transient services, so [`RemoteCommandHandler`](../services/remote-command-handler) can dispatch payloads to them. Call it after [`AddRemoteCommands`](./add-remote-commands), passing explicit assemblies when commands live outside the startup assembly.

A class is discovered by inheriting [`RemoteCommand<T>`](../types/remote-command), whose constructor then throws when the class does not also declare [`RemoteCommandAttribute`](../attributes/remote-command-attribute).

## Usage

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterRemoteCommands();

public IServiceCollection RegisterRemoteCommands(
    Assembly[] assemblies
);
```
