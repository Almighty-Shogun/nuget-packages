---
fields:
    - name: RemoteServerSettings
      description: The `RemoteServer` section, bound by [`AddRemoteCommands`](./extensions/add-remote-commands). Required, because `Port` has no default, so an absent section fails validation while the host starts.
      fields:
          - name: Address
            description: Local address the listener binds to. The default accepts only connections from the same machine; a routable address exposes the listener to everything that can reach it.
            type: string
            default: 127.0.0.1

          - name: Port
            description: Port the listener binds to. The one value with no default.
            type: int

          - name: Whitelisted
            description: Addresses or CIDR ranges allowed to connect. An empty list matches nothing, so the listener accepts connections and immediately drops every one.
            type: 'IReadOnlyList<string>'
            default: '[]'

          - name: Secret
            description: Pre-shared key a request must carry. Compared in constant time, and ignored entirely when unset. Plaintext on the wire, so it authenticates rather than protects.
            type: string?
            default: 'null'

          - name: EnableReceiveLog
            description: Logs each accepted command by name. Refusals are logged either way, so turning this off hides ordinary traffic rather than problems.
            type: bool
            default: 'false'

          - name: MaxPayloadBytes
            description: Largest request accepted, in bytes. Checked against the declared length before a buffer is rented, so an oversized frame costs nothing to refuse.
            type: int
            default: '1048576'

          - name: ReadTimeout
            description: How long serving one request may take, in seconds. This bounds the command itself, so a command that outlives it is cancelled and the client gets no response.
            type: int
            default: '30'

          - name: IdleTimeout
            description: How long a connection may sit idle between requests, in seconds, before it is closed.
            type: int
            default: '120'

          - name: MaxConcurrentConnections
            description: How many connections are served at once. A further client waits for a slot rather than being refused.
            type: int
            default: '100'
---

# Configuration

The `RemoteServer` section configures the listener and is required, because `Port` has no default. Everything else falls back to a value that is safe on its own: loopback only, and a whitelist that matches nothing.

```json
{
    "RemoteServer": {
        "Address": "127.0.0.1",
        "Port": 30001,
        "Whitelisted": [
            "127.0.0.1",
            "10.0.0.0/8"
        ],
        "Secret": "a-shared-key",
        "EnableReceiveLog": true,
        "MaxPayloadBytes": 1048576, /* 1 MiB */
        "ReadTimeout": 30,
        "IdleTimeout": 120,
        "MaxConcurrentConnections": 100
    }
}
```

::: warning
`Whitelisted` denies by default. Leave it empty and every client connects and is dropped without a response, which reads from the outside like a listener that is not running.
:::

<FrontmatterDocs/>
