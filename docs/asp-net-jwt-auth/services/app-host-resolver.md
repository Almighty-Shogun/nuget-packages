# AppHostResolver

Resolves an authentication app from the current request or from an explicit host string. ASP.NET JWT Auth uses this service as the host-to-app mapping primitive behind app-audience authorization.

Application code should depend on `IAppHostResolver`. The registered implementation is added by [`AddJwtAuth`](../extensions/add-jwt-auth), reads [`AuthSettings.Hosts`](../configuration/auth-settings) to determine whether app scoping is active, resolves the current request host when needed, and caches the resolved app in `HttpContext.Items` for the rest of the request.

The concrete resolver is registered by [`AddJwtAuth`](../extensions/add-jwt-auth), reads the `Auth` configuration section, uses [`AuthSettings.Hosts`](../configuration/auth-settings) for normal hosts, and uses [`AuthSettings.LocalhostApp`](../configuration/auth-settings) for `localhost`, `127.0.0.1`, and `::1` development requests.

## Usage

```csharp
using AlmightyShogun.AspNet.JwtAuth;

public sealed class CurrentAppService(IAppHostResolver appHostResolver)
{
    public string? GetCurrentApp()
        => appHostResolver.Resolve();
}
```

## Resolve

Resolves the authentication app for the current request. The method returns the configured app when app scoping is active and the current request host maps to an app. It returns `null` only when app scoping is disabled.

When app scoping is active and the current request cannot be resolved, the method throws [`HttpErrorException`](/asp-net-utils/types/http-error-exception) with status code `403 Forbidden`. Use [`TryResolve`](#tryresolve) when application code wants to decide how to handle an unknown host without an exception.

```csharp
using AlmightyShogun.AspNet.JwtAuth;

public sealed class TokenAudienceService(IAppHostResolver appHostResolver)
{
    public string? GetAudience()
        => appHostResolver.Resolve();
}
```

### Type signature

```csharp
public string? Resolve();
```

## TryResolve

Attempts to resolve the authentication app for the current request. The method returns `true` with `app` set to `null` when app scoping is disabled, `true` with an app value when the current host maps to a configured app, and `false` when app scoping is active but the current request cannot be resolved.

Use this method when application code needs to decide what to do with an unknown host instead of receiving a nullable app value.

```csharp
using AlmightyShogun.AspNet.JwtAuth;

public sealed class CurrentAppReader(IAppHostResolver appHostResolver)
{
    public bool TryGetCurrentApp(out string? app)
        => appHostResolver.TryResolve(out app);
}
```

### Type signature

```csharp
public bool TryResolve(out string? app);
```

## ResolveAppFromHost

Resolves a provided host to its configured application audience name. This method is kept for existing code that already depends on the older host resolver contract. For new request-scoped app resolution, prefer [`Resolve`](#resolve) or [`TryResolve`](#tryresolve).

The method returns the configured application name when the host exists in [`AuthSettings.Hosts`](../configuration/auth-settings), or when the host is a localhost value and [`AuthSettings.LocalhostApp`](../configuration/auth-settings) has a value. It throws [`HttpErrorException`](/asp-net-utils/types/http-error-exception) with status code `403 Forbidden` when the host is missing, unknown, or cannot be resolved for local development.

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

Attempts to map a provided host to an application audience name without throwing for unknown input. Use this method when application code already has a host string and wants to decide how to respond when that host is not configured.

The method returns `false` for `null`, empty, whitespace, unknown hosts, and localhost requests without a configured [`AuthSettings.LocalhostApp`](../configuration/auth-settings). When a host is known, the `app` out parameter receives the configured audience name.

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
