# AddCustomLogging

Registers Serilog with the package's custom console formatter. The package exposes overloads for both `IServiceCollection` and `IHostBuilder`, so applications can choose the registration style that matches their startup code.

Both overloads create a Serilog logger with log-context enrichment and an asynchronous console sink. When configuration is provided, the logger also reads Serilog settings from `IConfiguration`.

## Overloads

- [IServiceCollection](./service-collection) &mdash; registers Serilog through the service collection logging builder.
- [IHostBuilder](./host-builder) &mdash; registers Serilog through the host builder.
