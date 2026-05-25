---
outline: deep

fields:
    - name: Address
      description: IP address that the TCP listener should bind to.
      type: string

    - name: Port
      description: TCP port used by the remote command listener.
      type: int
      default: '0'

    - name: Whitelisted
      description: List of remote IP addresses allowed to send commands.
      type: string[]

    - name: EnableReceiveLog
      description: Enables informational and warning logs for received or unknown commands.
      type: bool
---

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

<FrontmatterDocs/>
