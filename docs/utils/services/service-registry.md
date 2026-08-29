# ServiceRegistry

Contract for service registry modules that configure an `IServiceCollection`. A type implementing it is added through [`AddService<T>`](../extensions/add-service), which constructs the module and calls `ConfigureService`.

Implementations need a public parameterless constructor, because `AddService<T>` constructs the module directly rather than resolving it from the container.

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
    public void ConfigureService(
        IServiceCollection serviceCollection
    ) => serviceCollection.AddSingleton<NotificationFormatter>();
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

Adds the module's registrations to the provided `IServiceCollection`. [`AddService<T>`](../extensions/add-service) calls this after constructing the module, and application code can call it directly when it already owns an instance.

The collection passed in is the application's live one, not a copy, so registrations made here are visible to the rest of startup and there is nothing to return. Do not resolve services or perform runtime work here; this method should only describe registrations.

### Type signature

```csharp
void ConfigureService(
    IServiceCollection serviceCollection
);
```
