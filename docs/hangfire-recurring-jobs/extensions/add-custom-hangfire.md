---
params:
    - name: compatibilityLevel
      description: Hangfire data compatibility level used when configuring storage and serialization. Pass a different level only to align with an existing Hangfire storage schema.
      type: CompatibilityLevel
      default: CompatibilityLevel.Version_180

    - name: addServer
      description: Whether this application also runs a background processing server. Set it to `false` on a client that only enqueues work and leaves processing to another process.
      type: bool
      default: 'true'

returns: The `IServiceCollection` instance with Hangfire configured.
---

# AddCustomHangfire

Registers Hangfire with the package's defaults: simple assembly-name serialization, the recommended serializer settings, in-memory storage, and a background processing server. Call it before [`RegisterRecurringJobs`](./register-recurring-jobs), which needs the recurring job manager this call provides.

## Usage

::: code-group

```csharp [Default.cs]
using AlmightyShogun.Hangfire.RecurringJobs;

builder.Services.AddCustomHangfire();
```

```csharp [ClientOnly.cs]
using AlmightyShogun.Hangfire.RecurringJobs;

builder.Services.AddCustomHangfire(addServer: false);
```

:::

::: warning
Storage is in-memory, so job state is lost on restart and every replica keeps its own store. An application running more than one replica runs each recurring job once per replica.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddCustomHangfire(
    CompatibilityLevel compatibilityLevel = CompatibilityLevel.Version_180,
    bool addServer = true
);
```
