---
outline: deep

fields:
    - name: Serilog
      description: Root section read by Serilog's configuration package.
      type: object

    - name: Serilog:MinimumLevel
      description: Controls the minimum log levels used by Serilog.
      type: object

    - name: Serilog:MinimumLevel:Override
      description: Sets namespace-specific minimum levels.
      type: 'Dictionary<string, string>'
---

# Configuration

Logging can optionally read Serilog settings from `appsettings.json` when `AddCustomLogging` receives an `IConfiguration` instance. The package always adds log-context enrichment and its custom asynchronous console sink; configuration can add or override extra Serilog behavior supported by `Serilog.Settings.Configuration`.

```json
{
    "Serilog": {
        "MinimumLevel": {
            "Default": "Information",
            "Override": {
                "Microsoft": "Warning",
                "Microsoft.Hosting.Lifetime": "Information"
            }
        }
    }
}
```

::: tip
This configuration is optional. `AddCustomLogging` always creates a Serilog logger with log-context enrichment and the package console formatter. Add the `Serilog` section only when the application needs to override levels or apply extra Serilog configuration from `appsettings.json`.
:::

<FrontmatterDocs/>
