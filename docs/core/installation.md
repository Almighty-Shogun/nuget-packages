# Installation

Install `AlmightyShogun.Core` in applications or packages that need the shared helper APIs directly. The package targets `net10.0` and depends on the Microsoft configuration, options, and dependency-injection abstractions.

```sh
dotnet add package AlmightyShogun.Core
```

## Dependencies

### Package references

- `Microsoft.Extensions.Configuration.Abstractions` `10.0.11` &mdash; provides `IConfigurationSection` for options binding.
- `Microsoft.Extensions.Configuration.Binder` `10.0.11` &mdash; binds configuration sections to strongly typed options.
- `Microsoft.Extensions.DependencyInjection.Abstractions` `10.0.11` &mdash; provides `IServiceCollection` and service lifetime types.
- `Microsoft.Extensions.Options` `10.0.11` &mdash; provides the options infrastructure.
- `Microsoft.Extensions.Options.ConfigurationExtensions` `10.0.11` &mdash; connects options to configuration binding.
- `Microsoft.Extensions.Options.DataAnnotations` `10.0.11` &mdash; enables data-annotation validation for configured options.
