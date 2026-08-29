---
params:
    - name: assemblies
      description: The assemblies to scan, in the order they should be searched. An empty array registers nothing; the overload taking no assembly is the one that falls back to the calling assembly.
      type: Assembly[]

returns: The `IServiceCollection` instance with discovered remote command classes registered.
---

# RegisterRemoteCommands

Registers remote command classes from one or more assemblies as transient services under their own concrete type, so [`RemoteCommandHandler`](../services/remote-command-handler) can resolve one per request. Call it after [`AddRemoteCommands`](./add-remote-commands), passing explicit assemblies when commands live outside the startup assembly.

A class is discovered by inheriting [`RemoteCommand<T>`](../types/remote-command) and must declare [`RemoteCommandAttribute`](../attributes/remote-command-attribute), whose name is what registration records, so a class without one throws here.

## Usage

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands();
```

::: tip
Each request resolves its command from its own scope, so a command may take scoped services such as a `DbContext` in its constructor and gets a fresh instance every time.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterRemoteCommands();

public IServiceCollection RegisterRemoteCommands(
    Assembly[] assemblies
);
```
