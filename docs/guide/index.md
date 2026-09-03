# Introduction

Welcome to NuGet Packages, a collection of small .NET packages used across ASP.NET Core APIs, hosted applications, console tooling, background jobs, logging, email, and Entity Framework Core. Each one is published independently and documented the same way: installation, configuration, usage, type signatures, and the dependencies a consumer will notice.

The repository is intentionally practical. Packages are small, .NET-first, dependency-injection-friendly, and aimed at infrastructure that is common enough to share but not large enough to become a framework. Several build on each other, so an API that installs one of the ASP.NET packages usually gets [Utils](/utils/) and [ASP.NET Core](/asp-net-core/) alongside it.

## ASP.NET

- [ASP.NET Core](/asp-net-core/) &mdash; the layer the other web packages build on: one standardized error body, exception mapping, request metadata, CORS, and forwarded headers for a proxied application.
- [ASP.NET Auth](/asp-net-auth/) &mdash; JWT bearer authentication, refresh-token cookies, host-to-application audience checks, and permission-based authorization.
- [ASP.NET Localization](/asp-net-localization/) &mdash; message text in JSON files on disk, resolved in the language the request asks for and reported back through `Content-Language`.
- [ASP.NET Maintenance Mode](/asp-net-maintenance-mode/) &mdash; file-backed maintenance windows that block traffic and survive a process restart without a database or coordinator.
- [ASP.NET Request Validation](/asp-net-request-validation/) &mdash; attribute and fluent request rules, collected per field and returned as one localized `422` rather than one failure at a time.
- [ASP.NET Auth Credentials](/asp-net-auth-credentials/) &mdash; first-party accounts: password login, refresh sessions, password reset, account lockout, and TOTP two-factor, stored in the application's own EF Core context.

## Operations

- [Console Commands](/console-commands/) &mdash; command classes discovered from assemblies, with string input converted into method parameters and dispatched from an input loop.
- [Hangfire Recurring Jobs](/hangfire-recurring-jobs/) &mdash; Hangfire setup where a job's schedule lives on the job class instead of being repeated in startup code.
- [Hosting Console Lifetime](/hosting-console-lifetime/) &mdash; a generic host that an accidental interrupt cannot kill, while an orchestrator's stop signal still shuts it down cleanly.
- [Remote Commands](/remote-commands/) &mdash; a TCP listener taking length-prefixed JSON and dispatching each payload to a typed handler.
- [Serilog](/serilog/) &mdash; Serilog registration with a console formatter that colors output by level and by property.

## Data

- [EF Core Model Building](/ef-core-model-building/) &mdash; `ModelBuilder` helpers for the shapes written on almost every model: relationships, indexes, enum storage, and eager loading.
- [Mail Resend](/mail-resend/) &mdash; Resend sending through a typed contract, with reusable HTML and plain-text templates that share one layout.

## Utils

- [Utils](/utils/) &mdash; the shared building blocks: assembly scanning for DI registration, typed configuration binding, JSON deserialization helpers, and small console utilities.
