---
outline: deep

params:
    - name: serviceCollection
      description: Service collection that should receive the module registrations.
      type: IServiceCollection

returns: The updated `IServiceCollection` after the module has added its registrations.
---

# ConfigureService

Adds the module's dependency-injection registrations to the provided `IServiceCollection`. `AddService<T>` calls this method after constructing the module, but application code can call it directly when it already owns a module instance.

Return the same service collection to keep registration calls chainable. Implementations should avoid resolving services or performing runtime work here; this method should only describe registrations.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Utils;

builder.Services.AddService<NotificationsModule>();
```

```csharp [NotificationsModule.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

public sealed class NotificationsModule : IService
{
    public IServiceCollection ConfigureService(IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<NotificationFormatter>();
    }
}
```

```csharp [NotificationFormatter.cs]
public sealed class NotificationFormatter
{
    public string Format(string message) => $"Notification: {message}";
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
IServiceCollection ConfigureService(
    IServiceCollection serviceCollection
);
```
