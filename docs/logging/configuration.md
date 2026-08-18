---
fields:
    - name: Serilog
      description: Root section read by Serilog's configuration package.
      type: object

    - name: MinimumLevel
      description: Controls the minimum log levels used by Serilog.
      type: object

    - name: MinimumLevel:Override
      description: Sets namespace-specific minimum levels.
      type: 'Dictionary<string, string>'
---

# Configuration

Serilog settings are read from `appsettings.json` when [`AddCustomLogging`](./extensions/add-custom-logging) receives an `IConfiguration`. The section is optional and the call creates a working logger without it; add `Serilog` only to override levels or apply extra Serilog configuration. The package always adds log-context enrichment, and adds its asynchronous console sink unless `includeConsoleSink` is `false`; configuration can add sinks and override levels through anything `Serilog.Settings.Configuration` supports.

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

::: warning
A `WriteTo` entry for the console in configuration is **added to** the package console sink, not merged with it, so the same line is written twice. Pass `includeConsoleSink: false` when the console sink is declared in configuration.
:::

<FrontmatterDocs/>
