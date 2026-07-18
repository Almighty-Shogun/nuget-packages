---
params:
    - name: compatibilityLevel
      description: Hangfire data compatibility level used when configuring storage and serialization.
      type: CompatibilityLevel
      default: CompatibilityLevel.Version_180

returns: The `IServiceCollection` instance with Hangfire and its server registered.
---

# AddHangfire

Registers Hangfire with the package's default setup. The method configures Hangfire for data compatibility level `Version_180` by default, simple assembly-name serialization, recommended serializer settings, in-memory storage, and the Hangfire server.

Use this method when the application wants a simple in-memory Hangfire setup. Pass a different `CompatibilityLevel` only when the application must align with an existing Hangfire storage schema or deployment. For production systems that need persistent storage, review whether this package default is appropriate before using it.

## Usage

::: code-group

```csharp [Default.cs]
using AlmightyShogun.Hangfire.Utils;

builder.Services.AddHangfire();
```

```csharp [CompatibilityLevel.cs]
using Hangfire;
using AlmightyShogun.Hangfire.Utils;

builder.Services.AddHangfire(CompatibilityLevel.Version_180);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddHangfire(
    CompatibilityLevel compatibilityLevel = CompatibilityLevel.Version_180
);
```
