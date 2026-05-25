---
outline: deep

returns: The `IServiceCollection` instance with Hangfire and its server registered.
---

# AddHangfire

Registers Hangfire with the package's default setup. The method configures Hangfire for compatibility level `Version_180`, simple assembly-name serialization, recommended serializer settings, in-memory storage, and the Hangfire server.

Use this method when the application wants a simple in-memory Hangfire setup. For production systems that need persistent storage, review whether this package default is appropriate before using it.

## Usage

```csharp
using AlmightyShogun.Hangfire.Utils;

builder.Services.AddHangfire();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddHangfire();
```
