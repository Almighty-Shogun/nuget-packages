---
fields:
    - name: Serilog
      description: The optional `Serilog` section itself, read by `Serilog.Settings.Configuration`. Every key that package understands is accepted here, including sinks and enrichers this package does not add on its own. Nothing binds it to an options type, and it is read once while the logger is being built, so there is no settings object to inject afterwards.
      fields:
          - name: MinimumLevel
            description: The nested `MinimumLevel` object.
            type: object

    - name: MinimumLevel
      description: The nested `Serilog:MinimumLevel` object, deciding which events reach the sinks at all.
      fields:
          - name: Default
            description: Level applied to every source without an override, as one of `Verbose`, `Debug`, `Information`, `Warning`, `Error`, or `Fatal`.
            type: string

          - name: Override
            description: Minimum level per source-context prefix, usually a namespace, so one noisy component can be raised without moving the default for everything else.
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
