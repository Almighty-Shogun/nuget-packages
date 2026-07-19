---
returns: The same `IServiceCollection` instance after the module has applied its registrations.
---

# AddService

Creates a new [`IServiceRegistry`](../services/service-registry) module of type `T` and calls `ConfigureService` so the module can add its own dependencies to the service collection. The generic type must implement [`IServiceRegistry`](../services/service-registry) and expose a public parameterless constructor.

Use this method for small registration modules that do not need constructor arguments. It keeps startup code compact while still letting each module own its dependency-injection registrations. Prefer a dedicated extension method when registration requires runtime values such as configuration objects or options.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();

services.AddService<ReportingServiceRegistry>();
```

```csharp [ReportingServiceRegistry.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

public sealed class ReportingServiceRegistry : IServiceRegistry
{
    public IServiceCollection ConfigureService(IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<ReportRenderer>();
    }
}
```

```csharp [ReportRenderer.cs]
public sealed class ReportRenderer
{
    public string Render(string title) => $"Report: {title}";
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddService<T>() where T : IServiceRegistry, new();
```
