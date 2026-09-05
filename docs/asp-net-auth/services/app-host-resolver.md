# AppHostResolver

Resolves which application a request belongs to, from its host, which is the mapping app-audience authorization is built on. Application code depends on `IAppHostResolver`.

[`Hosts`](../configuration) supplies the mapping and decides whether scoping is active at all, with [`LocalhostApp`](../configuration) covering localhost in development.

## Resolve

Resolves the authentication app for the current request. The method returns the configured app when app scoping is active and the current request host maps to an app. It returns `null` only when app scoping is disabled.

When app scoping is active and the request host maps to nothing, or there is no request in flight at all, the method throws [`UnknownAppException`](../exceptions), which reaches the client as `403`. Use [`TryResolve`](#tryresolve) when application code wants to decide how to handle an unknown host without an exception.

```csharp
using AlmightyShogun.AspNet.Auth;

public sealed class TokenAudienceService(IAppHostResolver appHostResolver)
{
    public string? GetAudience() => appHostResolver.Resolve();
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
using AlmightyShogun.AspNet.Auth;

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

Resolves a host the caller already holds rather than the one on the current request, such as a background job acting on behalf of a tenant. Use [`Resolve`](#resolve) or [`TryResolve`](#tryresolve) when the host should come from the request being served.

The method returns the configured application name when the host exists in [`AuthSettings.Hosts`](../configuration), matched case-insensitively, or when the host is a localhost value and [`AuthSettings.LocalhostApp`](../configuration) has a value. It throws [`UnknownAppException`](../exceptions), carrying the host it could not resolve, when the host is blank or maps to no configured application.

```csharp
using AlmightyShogun.AspNet.Auth;

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

The method returns `false` for `null`, empty, whitespace, unknown hosts, and localhost requests without a configured [`AuthSettings.LocalhostApp`](../configuration). When a host is known, the `app` out parameter receives the configured audience name; otherwise it is an empty string rather than `null`.

```csharp
using AlmightyShogun.AspNet.Auth;

public static bool IsKnownAppHost(
    IAppHostResolver appHostResolver,
    string host
)
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
