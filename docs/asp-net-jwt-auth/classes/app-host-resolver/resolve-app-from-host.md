---
outline: deep

params:
    - name: host
      description: Request host name to resolve, such as `api.example.com`.
      type: string?

returns: The configured application audience name for the host.
---

# ResolveAppFromHost

Resolves a request host to its configured application audience name. The method uses `AuthSettings.Hosts` for configured host names and `AuthSettings.LocalhostApp` when the host is `localhost`.

Unlike `TryResolveAppFromHost`, this method throws `UnauthorizedAccessException` when the host is missing, unknown, or `localhost` is used without a configured `LocalhostApp`.

Use this method when an unknown host should be treated as an authentication or authorization failure. This mirrors the package's JWT validation behavior, where a token is rejected when the current host cannot be mapped to an app.

## Usage

```csharp
using AlmightyShogun.AspNet.JwtAuth;

public sealed class AppScopedService(IAppHostResolver appHostResolver)
{
    public string GetAppForRequestHost(string host)
        => appHostResolver.ResolveAppFromHost(host);
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public string ResolveAppFromHost(string? host);
```
