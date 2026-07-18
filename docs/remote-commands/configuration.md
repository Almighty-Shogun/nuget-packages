# Configuration

Remote Commands reads the `RemoteServer` section from `appsettings.json` when `AddRemoteCommands` receives an `IConfiguration` instance. The section is bound to `RemoteServerSettings` and consumed by `RemoteCommandHandler` through options.

```json
{
    "RemoteServer": {
        "Address": "127.0.0.1",
        "Port": 30001,
        "Whitelisted": [
            "127.0.0.1"
        ],
        "EnableReceiveLog": true
    }
}
```

See [RemoteServerSettings](./configuration/remote-server-settings) for field descriptions and defaults.
