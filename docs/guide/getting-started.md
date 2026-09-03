# Getting started

This guide shows how to install one or more `AlmightyShogun.*` packages and use them in a .NET application.

## Prerequisites

- .NET 10 SDK.
- ASP.NET Core when using any `AlmightyShogun.AspNet.*` package.
- Entity Framework Core when using `AlmightyShogun.EntityFrameworkCore.ModelBuilding` or `AlmightyShogun.AspNet.Auth.Credentials`.
- Hangfire when using `AlmightyShogun.Hangfire.RecurringJobs`.
- A Resend account and API key when using `AlmightyShogun.Mail.Resend`.
- Application configuration from `appsettings.json` when a package reads options through `builder.Configuration`.

## Install your first package

Most ASP.NET Core APIs start with `AlmightyShogun.AspNet.Auth`. It registers JWT bearer authentication, permission authorization, refresh-token cookie helpers, and host-to-application audience validation.

```sh
dotnet add package AlmightyShogun.AspNet.Auth
```

```csharp
using AlmightyShogun.AspNet.Auth;

builder.Services.AddAuth(builder.Configuration);
```

The package expects an `Auth` section in `appsettings.json`. See the [ASP.NET Auth configuration page](/asp-net-auth/configuration) for the full JSON shape and field descriptions.

## Common ASP.NET setup

`AlmightyShogun.AspNet.Core` is the layer the other web packages build on. It owns the one error body every package writes, and that body carries a localized description, so `AlmightyShogun.AspNet.Localization` is registered alongside it.

```sh
dotnet add package AlmightyShogun.AspNet.Core
dotnet add package AlmightyShogun.AspNet.Localization
dotnet add package AlmightyShogun.AspNet.Auth
```

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Localization;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddHttpErrorResponseFilter()
    .AddCorsPolicy("DefaultCors", builder.Configuration)
    .AddAuth(builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseMessageLocalization();
```

Controllers can then use the attributes and helpers from the installed packages:

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;

[ApiController]
[Route("admin/users")]
public sealed class AdminUsersController : ControllerBase
{
    [HttpGet]
    [AuthPermission("users.read")]
    public IActionResult ListUsers() => Ok();
}
```

## Request validation

`AlmightyShogun.AspNet.RequestValidation` validates request models through attributes, fluent rules, or both, and reports every failure in one `422` response.

```sh
dotnet add package AlmightyShogun.AspNet.RequestValidation
```

```csharp
using AlmightyShogun.AspNet.RequestValidation;

builder.Services.AddAspNetValidation();

WebApplication app = builder.Build();

app.UseAspNetValidation();
```

## Credential login

Use `AlmightyShogun.AspNet.Auth.Credentials` when the API owns username and password accounts. It builds on ASP.NET Auth for access tokens, ASP.NET Core for request metadata and error responses, ASP.NET Request Validation for the request models, and Entity Framework Core for storage.

```sh
dotnet add package AlmightyShogun.AspNet.Auth.Credentials
```

```csharp
using AlmightyShogun.AspNet.Core;
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Localization;
using AlmightyShogun.AspNet.Auth.Credentials;
using AlmightyShogun.AspNet.RequestValidation;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddAuth(builder.Configuration)
    .AddAspNetValidation()
    .AddDbContext<AppDbContext>(options => ...)
    .AddAuthCredentials<AppDbContext, AppUser>(builder.Configuration);
```

Application code derives its context from [`AuthDbContext<TUser>`](/asp-net-auth-credentials/types/auth-db-context) and its user entity from [`AuthUser`](/asp-net-auth-credentials/types/auth-user). Use [`IAuthUserService<TUser>`](/asp-net-auth-credentials/services/auth-user-service) for login and registration, [`IAuthSessionService<TUser>`](/asp-net-auth-credentials/services/auth-session-service) for refresh-token rotation, [`IAuthPasswordService`](/asp-net-auth-credentials/services/auth-password-service) for password changes and resets, and [`IAuthTwoFactorService<TUser>`](/asp-net-auth-credentials/services/auth-two-factor-service) for two-factor enrolment.

## Console commands

Use `AlmightyShogun.ConsoleCommands` when a hosted console application should discover command classes from its own assemblies and run them from an input loop.

```sh
dotnet add package AlmightyShogun.ConsoleCommands
```

```csharp
using AlmightyShogun.ConsoleCommands;

builder.Services
    .AddConsoleCommands()
    .RegisterConsoleCommands();
```

A command is a public class carrying [`ConsoleCommandAttribute`](/console-commands/attributes/console-command-attribute), inheriting [`ConsoleCommandBase`](/console-commands/types/console-command-base), and exposing exactly one public `ExecuteAsync` returning `Task`. The base takes no constructor arguments, so a command needing nothing declares no constructor at all.

```csharp
using AlmightyShogun.ConsoleCommands;

[ConsoleCommand("ping", "Writes a pong response.")]
public sealed class PingCommand : ConsoleCommandBase
{
    public Task ExecuteAsync()
    {
        Console.WriteLine("pong");

        return Task.CompletedTask;
    }
}
```

## Entity Framework Core

Use `AlmightyShogun.EntityFrameworkCore.ModelBuilding` when repeated relationship or index configuration starts to make `OnModelCreating` noisy. Every helper returns the `ModelBuilder`, so calls chain.

```sh
dotnet add package AlmightyShogun.EntityFrameworkCore.ModelBuilding
```

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> Sessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyOneToMany<User, UserSession>(
                user => user.Sessions,
                session => session.UserId,
                inverseNavigation: session => session.User
            )
            .ApplyIndex<UserSession>(session => session.UserId);
    }
}
```

```csharp [Entities.cs]
public sealed class User
{
    public int Id { get; set; }
    public List<UserSession> Sessions { get; set; } = [];
}

public sealed class UserSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}
```

:::

## Hangfire jobs

Use `AlmightyShogun.Hangfire.RecurringJobs` when a job's schedule should live on the job class instead of being repeated in startup code.

```sh
dotnet add package AlmightyShogun.Hangfire.RecurringJobs
```

```csharp
using AlmightyShogun.Hangfire.RecurringJobs;

builder.Services
    .AddCustomHangfire()
    .RegisterRecurringJobs(builder.Configuration);
```

## Remote commands

Use `AlmightyShogun.RemoteCommands` when an application should listen for length-prefixed JSON payloads over TCP and dispatch them to typed handlers.

```sh
dotnet add package AlmightyShogun.RemoteCommands
```

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands();
```

## Logging

Use `AlmightyShogun.Serilog` for Serilog registration with a console formatter that colors output by level and by property. Configuration is optional.

```sh
dotnet add package AlmightyShogun.Serilog
```

```csharp
using AlmightyShogun.Serilog;

builder.Services.AddCustomLogging(builder.Configuration);
```

## Email

Use `AlmightyShogun.Mail.Resend` when an application sends reusable HTML and plain-text templates through Resend.

```sh
dotnet add package AlmightyShogun.Mail.Resend
```

```csharp
using AlmightyShogun.Mail.Resend;

builder.Services.AddResendEmail(builder.Configuration);
```
