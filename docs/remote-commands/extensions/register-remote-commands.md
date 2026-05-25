---
outline: deep

params:
    - name: assemblies
      description: Assemblies scanned for concrete remote command classes. When omitted, the calling assembly is used.
      type: Assembly[]
      default: '[]'

returns: The `IServiceCollection` instance with discovered remote command classes registered.
---

# RegisterRemoteCommands

Registers application remote command classes from one or more assemblies. The method scans for concrete implementations of the public remote command contract and registers them as transient services so `RemoteCommandHandler` can dispatch incoming payloads to them.

Use this method after `AddRemoteCommands`. Pass explicit assemblies when command classes live in another project; relying on the calling assembly is only appropriate when commands are defined in the startup assembly. Command classes should inherit from `RemoteCommand<T>`.

## Usage

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands(typeof(Program).Assembly);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterRemoteCommands(
    params Assembly[] assemblies
);
```
