---
params:
    - name: configure
      description: Selects the storage provider and data compatibility level. Hangfire throws when no storage is set by the final configuration; the compatibility level has a default and may be left alone.
      type: 'Action<IGlobalConfiguration>'

    - name: addServer
      description: Whether this application also runs a background processing server. Set it to `false` on a client that only enqueues work and leaves processing to another process.
      type: bool
      default: 'true'

returns: The `IServiceCollection` instance with Hangfire configured.
---

# AddCustomHangfire

Registers Hangfire with the package's serializer defaults and a background processing server. Called without a delegate it uses in-memory storage and data compatibility level `Version_180`; pass a delegate to select another storage provider and set the compatibility level yourself. Pair it with [`RegisterRecurringJobs`](./register-recurring-jobs), in either order, since neither reads what the other registered.

## Usage

::: code-group

```csharp [InMemory.cs]
using AlmightyShogun.Hangfire.RecurringJobs;

builder.Services.AddCustomHangfire();
```

```csharp [SqlServer.cs]
using Hangfire;
using AlmightyShogun.Hangfire.RecurringJobs;

string connection = builder.Configuration.GetConnectionString("Hangfire")!;

builder.Services.AddCustomHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSqlServerStorage(connection));
```

```csharp [SharedStore.cs]
using Hangfire;
using AlmightyShogun.Hangfire.RecurringJobs;

string connection = builder.Configuration.GetConnectionString("Hangfire")!;

builder.Services.AddCustomHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSqlServerStorage(connection));
```

```csharp [ClientOnly.cs]
using AlmightyShogun.Hangfire.RecurringJobs;

builder.Services.AddCustomHangfire(addServer: false);
```

:::

::: warning
In-memory storage loses job state on restart and gives every replica its own store, so an application running more than one replica runs each recurring job once per replica. Pass a delegate selecting a durable provider to avoid both.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddCustomHangfire(
    bool addServer = true
);

public IServiceCollection AddCustomHangfire(
    Action<IGlobalConfiguration> configure,
    bool addServer = true
);
```
