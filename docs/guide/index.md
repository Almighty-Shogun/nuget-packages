# Introduction

Welcome to NuGet Packages, a small collection of .NET packages used across ASP.NET Core APIs, hosted applications, console tooling, background jobs, logging, email, Entity Framework Core, and shared application utilities. Each package is published independently and documented with the same structure: installation, configuration, usage examples, parameters, returns, type signatures, and dependency notes where relevant.

The repository is intentionally practical. Packages are small, .NET-first, dependency-injection-friendly, and focused on reusable application infrastructure that is common enough to share but not large enough to become a framework.

## What packages are there?

- [ASP.NET JWT Auth](/asp-net-jwt-auth/) &mdash; JWT bearer authentication, refresh-token cookies, host-to-application audience checks, and permission-based authorization helpers.
- [ASP.NET Maintenance](/asp-net-maintenance/) &mdash; file-backed maintenance mode middleware and services for ASP.NET Core applications.
- [ASP.NET Utils](/asp-net-utils/) &mdash; ASP.NET Core request helpers for CORS, MVC filters, session context, cookies, and User-Agent parsing.
- [Console Commands](/console-commands/) &mdash; Attribute-based console command discovery and execution for dependency-injected console applications.
- [Entity Framework Core Utils](/ef-core-utils/) &mdash; `ModelBuilder` helpers for relationships, navigation, and indexes.
- [Hangfire Utils](/hangfire-utils/) &mdash; Hangfire registration helpers and recurring-job discovery through attributes.
- [Hosting Utils](/hosting-utils/) &mdash; host option and console lifetime helpers for .NET hosted applications.
- [Logging](/logging/) &mdash; Serilog registration helpers with a compact console formatter.
- [Remote Commands](/remote-commands/) &mdash; TCP-based remote command handling with typed JSON payloads.
- [Resend Utils](/resend-utils/) &mdash; Resend email registration, reusable templates, and a mail sending contract.
- [Utils](/utils/) &mdash; shared helpers for options binding, service modules, JSON serialization, type discovery, and console behavior.
