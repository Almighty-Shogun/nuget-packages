# Configuration

Remote Commands reads the `RemoteServer` section from `appsettings.json` when [`AddRemoteCommands`](./extensions/add-remote-commands) receives an `IConfiguration` instance. The section is bound to [`RemoteServerSettings`](./configuration/remote-server-settings) and consumed by [`RemoteCommandHandler`](./services/remote-command-handler) through options.

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
