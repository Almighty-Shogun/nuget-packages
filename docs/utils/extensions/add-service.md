---
returns: The `IServiceCollection` returned by the module's `ConfigureService` implementation.
---

# AddService

Constructs a service registry module and calls its [`ConfigureService`](../services/service-registry) method, so a group of related registrations can live in one reusable type instead of being spread across startup code.

The module type must implement [`IServiceRegistry`](../services/service-registry) and have a public parameterless constructor. It is created directly rather than resolved from the container, so it cannot take constructor dependencies.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddService<NotificationsRegistry>();
```

```csharp [NotificationsRegistry.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

public sealed class NotificationsRegistry : IServiceRegistry
{
    public IServiceCollection ConfigureService(
        IServiceCollection serviceCollection
    ) => serviceCollection 
            .AddSingleton<NotificationFormatter>()
            .AddScoped<INotificationSender, EmailNotificationSender>();
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddService<T>() where T : IServiceRegistry, new();
```
