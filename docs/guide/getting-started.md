# Getting started

This guide shows how to install one or more `AlmightyShogun.*` packages and use them in a .NET application.

## Prerequisites

- .NET 10 SDK.
- ASP.NET Core when using `AlmightyShogun.AspNet.*` packages.
- Entity Framework Core when using `AlmightyShogun.EntityFrameworkCore.Utils`.
- Hangfire when using `AlmightyShogun.Hangfire.Utils`.
- Resend account and API key when using `AlmightyShogun.Resend.Utils`.
- Application configuration from `appsettings.json` when a package reads options through `builder.Configuration`.

## Install your first package

Most ASP.NET Core APIs can start with `AlmightyShogun.AspNet.JwtAuth`. It registers JWT bearer authentication, permission authorization, refresh-token cookie helpers, and host-to-application audience validation.

```sh
dotnet add package AlmightyShogun.AspNet.JwtAuth
```

```csharp
using AlmightyShogun.AspNet.JwtAuth;

builder.Services.AddApiAuth(builder.Configuration);
```

The package expects an `Auth` section in `appsettings.json`. See the [ASP.NET JWT Auth configuration page](/asp-net-jwt-auth/configuration) for the full JSON shape and field descriptions.

## Common ASP.NET setup

An ASP.NET Core API usually combines authentication with request helpers. Use `AlmightyShogun.AspNet.JwtAuth` for authentication and `AlmightyShogun.AspNet.Utils` for CORS, MVC filters, session context, cookie helpers, and User-Agent parsing.

```sh
dotnet add package AlmightyShogun.AspNet.JwtAuth
dotnet add package AlmightyShogun.AspNet.Utils
```

```csharp
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.JwtAuth;

builder.Services
    .AddApiAuth(builder.Configuration)
    .AddActionFilters()
    .AddAllowedOrigins("DefaultCors", builder.Configuration);
```

Controllers can then use attributes and helpers from the installed packages:

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;

[ApiController]
[Route("admin/users")]
public sealed class AdminUsersController : ControllerBase
{
    [HttpGet]
    [AuthPermission("users.read")]
    public IActionResult ListUsers() => Ok();
}
```

## Console commands

Use `AlmightyShogun.ConsoleCommands` when a hosted console application should discover command classes from application assemblies and execute them from an input loop.

```sh
dotnet add package AlmightyShogun.ConsoleCommands
```

```csharp
using AlmightyShogun.ConsoleCommands;

builder.Services
    .AddConsoleCommands()
    .RegisterConsoleCommands(typeof(Program).Assembly);
```

Commands are public classes that derive from the package command base, add `ConsoleCommandAttribute` to the class, and expose exactly one public `ExecuteAsync` method returning `Task`.

```csharp
using Microsoft.Extensions.Logging;
using AlmightyShogun.ConsoleCommands;

[ConsoleCommand("ping", "Writes a pong response.")]
public sealed class PingCommand(ILogger<ConsoleCommandBase> logger) : ConsoleCommandBase(logger)
{
    public Task ExecuteAsync()
    {
        Console.WriteLine("pong");

        return Task.CompletedTask;
    }
}
```

## Entity Framework Core

Use `AlmightyShogun.EntityFrameworkCore.Utils` when repeated relationship, navigation, or index configuration starts to make `OnModelCreating` noisy. The package adds chainable extension methods to `ModelBuilder` for common one-to-one, one-to-many, many-to-one, auto-include, and index configuration.

```sh
dotnet add package AlmightyShogun.EntityFrameworkCore.Utils
```

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
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

Use `AlmightyShogun.Hangfire.Utils` when recurring jobs should be discovered from attributes instead of registered one by one in startup code.

```sh
dotnet add package AlmightyShogun.Hangfire.Utils
```

```csharp
using AlmightyShogun.Hangfire.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddHangfire()
    .RegisterRecurringJobs(typeof(Program).Assembly);
```

## Remote commands

Use `AlmightyShogun.RemoteCommands` when an application should listen for length-prefixed JSON command payloads over TCP and dispatch them to typed command handlers discovered from assemblies.

```sh
dotnet add package AlmightyShogun.RemoteCommands
```

```csharp
using AlmightyShogun.RemoteCommands;

builder.Services
    .AddRemoteCommands(builder.Configuration)
    .RegisterRemoteCommands(typeof(Program).Assembly);
```

## Email

Use `AlmightyShogun.Resend.Utils` when an application sends reusable HTML and plain-text email templates through Resend.

```sh
dotnet add package AlmightyShogun.Resend.Utils
```

```csharp
using AlmightyShogun.Resend.Utils;

builder.Services.AddResendEmail(builder.Configuration);
```
