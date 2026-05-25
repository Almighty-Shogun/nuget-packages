---
outline: deep

returns: The `IServiceCollection` instance after the service module has configured it.
---

# AddService

Creates a new `IService` module of type `T` and calls `ConfigureService` so the module can add its own dependencies to the service collection. This is useful when a package or application groups related registrations behind a small startup class.

The generic type must implement `IService` and expose a public parameterless constructor. Use constructor-free service modules for simple registration bundles; use direct extension methods when registration requires runtime values such as configuration objects.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();

services.AddService<ReportingServiceModule>();
```

```csharp [ReportingServiceModule.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

public sealed class ReportingServiceModule : IService
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
public IServiceCollection AddService<T>() where T : IService, new();
```

## Uses

- [IService](../../interfaces/iservice/)
