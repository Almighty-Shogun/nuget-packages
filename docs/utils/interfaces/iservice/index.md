# IService

Contract for service modules that configure an `IServiceCollection`. A type implementing this interface can be registered through `AddService<T>`, which constructs the module and calls `ConfigureService`.

Use this interface when a package or application wants to group related dependency-injection registrations behind a reusable module. Keep implementations focused on registration only; runtime behavior should live in the services the module registers.

## Usage

::: code-group

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

## Methods

- [ConfigureService](./configure-service) &mdash; adds module services to an `IServiceCollection`.
