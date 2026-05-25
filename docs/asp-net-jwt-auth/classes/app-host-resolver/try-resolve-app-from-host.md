---
outline: deep

params:
    - name: host
      description: Request host name to resolve, such as `api.example.com`.
      type: string?

    - name: app
      description: Resolved application audience name when the host is known.
      type: string

returns: '`true` when the host is configured, otherwise `false`.'
---

# TryResolveAppFromHost

Attempts to map a request host to the application audience name configured for that host. The method checks `AuthSettings.Hosts` first, and when the host is `localhost`, it uses `AuthSettings.LocalhostApp` as the resolved application name.

The method returns `false` for null, empty, whitespace, unknown hosts, and `localhost` requests without a configured `LocalhostApp` instead of throwing.

Use this method when unknown hosts are expected and application code wants to decide how to respond. For strict validation flows where an unknown host should immediately fail, use `ResolveAppFromHost`.

## Usage

```csharp
using AlmightyShogun.AspNet.JwtAuth;

public static bool IsKnownAppHost(IAppHostResolver appHostResolver, string host)
{
    return appHostResolver.TryResolveAppFromHost(host, out string app)
        && app == "api";
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public bool TryResolveAppFromHost(
    string? host,
    out string app
);
```
