---
outline: deep
---

# Stop

Stops the active TCP listener and clears the listener instance. If the listener has not been started, the method returns without doing anything.

Use this method when the application needs to stop accepting remote command connections before the process exits or during a controlled shutdown flow.

## Usage

```csharp
using AlmightyShogun.RemoteCommands;
using Microsoft.Extensions.DependencyInjection;

IRemoteCommandHandler handler = serviceProvider.GetRequiredService<IRemoteCommandHandler>();

handler.Stop();
```

<FrontmatterDocs/>

## Type signature

```csharp
public void Stop();
```
