# Installation

Install `AlmightyShogun.Utils` in applications or packages that need the shared helper APIs directly. The package targets `net10.0` and depends on the Microsoft configuration, options, and dependency-injection abstractions.

```sh
dotnet add package AlmightyShogun.Utils
```

## Dependencies

### Package references

- `Microsoft.Extensions.Configuration.Abstractions` `10.0.11` &mdash; provides `IConfigurationSection` for options binding.
- `Microsoft.Extensions.Configuration.Binder` `10.0.11` &mdash; binds configuration sections to strongly typed options.
- `Microsoft.Extensions.DependencyInjection.Abstractions` `10.0.11` &mdash; provides `IServiceCollection` and service lifetime types.
- `Microsoft.Extensions.Options` `10.0.11` &mdash; provides the options infrastructure.
- `Microsoft.Extensions.Options.ConfigurationExtensions` `10.0.11` &mdash; connects options to configuration binding.
- `Microsoft.Extensions.Options.DataAnnotations` `10.0.11` &mdash; enables data-annotation validation for configured options.

## Usage

The package registers nothing at startup. Its `IServiceCollection` extensions are called from startup code once the namespace is imported, its [`TryDeserialize`](./extensions/try-deserialize) and [`DeserializeAsync`](./extensions/deserialize-async) extensions hang off `string` and `Stream` instead, and the [`ConsoleUtils`](./utilities/console-utils) and [`TypeDiscovery`](./utilities/type-discovery) helpers are static and called wherever they are needed:

```csharp
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConfiguration<ImportSettings>(
        builder.Configuration.GetSection("Import")
    )
    .RegisterOnInherit<IImportStep>(ServiceLifetime.Transient)
    .AddService<NotificationsRegistry>();
```
