# Installation

Install `AlmightyShogun.AspNet.Maintenance` in the ASP.NET Core application that needs to block normal requests during maintenance windows. The package targets `net10.0`, uses ASP.NET Core middleware APIs, and stores maintenance state in a `maintenance.json` file under the application content root.

```sh
dotnet add package AlmightyShogun.AspNet.Maintenance
```

## Dependencies

- `Microsoft.AspNetCore.App` framework reference &mdash; provides `IApplicationBuilder`, middleware, HTTP response, and ASP.NET Core hosting APIs used by the package.
- `AlmightyShogun.Utils` project reference &mdash; provides configuration binding and JSON serialization helpers used by the package.

## Startup Registration

Register the service once during application startup, then add the middleware to the request pipeline. Place `UseMaintenanceMode` early enough that it can block normal application endpoints, but after middleware that must always run for every request.

The `Maintenance` configuration section is optional. When it is omitted, the package uses `/maintenance` as the maintenance details path, no default message, no automatic expiration, redirects blocked requests to the maintenance path, and has no allowed paths configured.

```csharp
using AlmightyShogun.AspNet.Maintenance;

builder.Services.AddMaintenanceMode(builder.Configuration);

WebApplication app = builder.Build();

app.UseMaintenanceMode();
```
