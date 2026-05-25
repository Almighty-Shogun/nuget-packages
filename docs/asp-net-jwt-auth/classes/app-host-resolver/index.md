# AppHostResolver

Resolves an application audience name from a request host using the `Auth:Hosts` configuration map. JWT validation uses this resolver to reject tokens that are validly signed but intended for a different configured application.

When the request host is `localhost`, the resolver uses `AuthSettings.LocalhostApp` instead of the `Hosts` map. This keeps local development simple while still resolving to a real application audience name.

`AddApiAuth` registers the concrete resolver for the `IAppHostResolver` interface. Application code should depend on `IAppHostResolver` when resolving it from dependency injection, because the interface is the public service contract and keeps consumers independent from the concrete implementation.

## Usage

```csharp
using AlmightyShogun.AspNet.JwtAuth;

public sealed class CurrentAppService(IAppHostResolver appHostResolver)
{
    public string GetCurrentApp(string host)
        => appHostResolver.ResolveAppFromHost(host);
}
```

## Methods

- [ResolveAppFromHost](./resolve-app-from-host) &mdash; returns the mapped app or throws when the host is unknown.
- [TryResolveAppFromHost](./try-resolve-app-from-host) &mdash; returns whether a host can be mapped to an app.
