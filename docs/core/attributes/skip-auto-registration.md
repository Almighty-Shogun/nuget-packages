# SkipAutoRegistration

Excludes a type from assembly scanning performed by [`RegisterOnInherit`](../extensions/register-on-inherit). Apply it to a concrete type that would otherwise be discovered but should be registered by hand.

It is honored unconditionally, so a type carrying it is never registered even when a `filter` predicate would accept it, while [`TypeDiscovery.FindAssignableTypes`](../utilities/type-discovery) ignores it entirely as a raw reflection primitive.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Core;
using Microsoft.Extensions.DependencyInjection;

builder.Services.RegisterOnInherit<IImportStep>(ServiceLifetime.Scoped);

builder.Services
    .AddScoped<IImportStep>(provider => new LegacyImportStep("legacy"));
```

```csharp [LegacyImportStep.cs]
using AlmightyShogun.Core;

[SkipAutoRegistration]
public sealed class LegacyImportStep(string profile) : IImportStep
{
    public Task RunAsync() => Task.CompletedTask;
}
```

:::

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class SkipAutoRegistrationAttribute : Attribute;
```
