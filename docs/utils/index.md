# Utils

Provides shared helpers used by the other packages in this repository and by applications that need the same small building blocks directly. The package contains console helpers, reflection-based type discovery, JSON deserialization extension methods, dependency-injection registration helpers, and the `IService` module contract.

Use this package when an application needs lightweight infrastructure utilities without taking a dependency on one of the more specific packages. The APIs are intentionally small and are most useful in startup code, command-line applications, and packages that need to scan assemblies or bind strongly typed options.

## Categories

- [Classes](./classes/application-utils/) &mdash; console and assembly scanning helpers.
- [Extensions](./extensions/deserialize-extensions/) &mdash; JSON and dependency-injection extension methods.
- [Interfaces](./interfaces/iservice/) &mdash; service module contracts used by the DI helpers.

## Quick Example

```csharp
using AlmightyShogun.Utils;

ApplicationUtils.Title("Worker");

builder.Services.AddConfiguration<MySettings>(
    builder.Configuration.GetSection("MySettings")
);
```
