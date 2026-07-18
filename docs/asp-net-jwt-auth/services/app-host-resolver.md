# AppHostResolver

Resolves an application audience name from a request host. ASP.NET JWT Auth uses this during JWT validation to reject tokens that are validly signed but intended for a different configured application.

Application code should depend on `IAppHostResolver`. The concrete resolver is registered by `AddApiAuth`, reads the `Auth` configuration section, uses `AuthSettings.Hosts` for normal hosts, and uses `AuthSettings.LocalhostApp` for `localhost` development requests.

## Usage

```csharp
using AlmightyShogun.AspNet.JwtAuth;

public sealed class CurrentAppService(IAppHostResolver appHostResolver)
{
    public string GetCurrentApp(string host)
        => appHostResolver.ResolveAppFromHost(host);
}
```

## ResolveAppFromHost

Resolves a request host to its configured application audience name. Use this method when an unknown host should be treated as an authentication or authorization failure.

The method returns the configured application name when the host exists in `AuthSettings.Hosts`, or when the host is `localhost` and `AuthSettings.LocalhostApp` has a value. It throws `UnauthorizedAccessException` when the host is missing, unknown, or cannot be resolved for local development.

```csharp
using AlmightyShogun.AspNet.JwtAuth;

public sealed class AppScopedService(IAppHostResolver appHostResolver)
{
    public string GetAppForRequestHost(string host)
        => appHostResolver.ResolveAppFromHost(host);
}
```

### Type signature

```csharp
public string ResolveAppFromHost(string? host);
```

## TryResolveAppFromHost

Attempts to map a request host to an application audience name without throwing for unknown input. Use this method when application code wants to decide how to respond when a host is not configured.

The method returns `false` for `null`, empty, whitespace, unknown hosts, and `localhost` requests without a configured `AuthSettings.LocalhostApp`. When a host is known, the `app` out parameter receives the configured audience name.

```csharp
using AlmightyShogun.AspNet.JwtAuth;

public static bool IsKnownAppHost(IAppHostResolver appHostResolver, string host)
{
    return appHostResolver.TryResolveAppFromHost(host, out string app)
        && app == "api";
}
```

### Type signature

```csharp
public bool TryResolveAppFromHost(
    string? host,
    out string app
);
```
