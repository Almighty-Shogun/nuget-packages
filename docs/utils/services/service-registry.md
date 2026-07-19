# ServiceRegistry

Contract for service registry modules that configure an `IServiceCollection`. A type implementing `IServiceRegistry` can be registered through [`AddService<T>`](../extensions/add-service), which constructs the module and calls `ConfigureService`.

Use this contract when a package or application wants to group related dependency-injection registrations behind a reusable module. Keep implementations focused on registration only; runtime behavior should live in the services the module registers.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();

services.AddService<NotificationsRegistry>();
```

```csharp [NotificationsRegistry.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

public sealed class NotificationsRegistry : IServiceRegistry
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

## ConfigureService

Adds the registry module's dependency-injection registrations to the provided `IServiceCollection`. [`AddService<T>`](../extensions/add-service) calls this method after constructing the module, but application code can call it directly when it already owns a module instance.

Return the same service collection to keep registration calls chainable. Implementations should avoid resolving services or performing runtime work here; this method should only describe registrations.

### Type signature

```csharp
IServiceCollection ConfigureService(
    IServiceCollection serviceCollection
);
```
