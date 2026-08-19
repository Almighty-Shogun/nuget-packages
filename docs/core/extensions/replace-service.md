---
params:
    - name: serviceLifetime
      description: Lifetime applied to the replacement. It need not match the lifetime of the registration being displaced.
      type: ServiceLifetime
      default: ServiceLifetime.Singleton

returns: The same `IServiceCollection` instance with the registration replaced.
---

# ReplaceService

Swaps whatever is registered for `TService` with `TImplementation`, for overriding a default that a framework or another package registered first. Adds the registration when there was nothing to replace, so it is safe to call before the default exists. Only the first registration for the service is replaced, so reach for something else when the service is registered many times and resolved as a sequence.

## Usage

```csharp
using AlmightyShogun.Core;
using Microsoft.Extensions.DependencyInjection;

builder.Services.ReplaceService<IClock, TestClock>();
```

::: warning
Order still matters in the other direction. A plain `Add` for the same service after this call wins when the service is resolved singly, because the last registration is the one returned.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection ReplaceService<TService, TImplementation>(
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton
) where TService : class where TImplementation : class, TService;
```
