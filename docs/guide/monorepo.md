# Monorepo

This repository contains the `AlmightyShogun.*` NuGet packages and the VitePress documentation site that describes them. Package source lives under `packages/`, documentation under `docs/`, and `packages.sln` is the solution that builds every package together.

```text
nuget-packages/
    packages/
        AlmightyShogun.AspNet.Core/
        AlmightyShogun.AspNet.CredentialAuth/
        AlmightyShogun.AspNet.JwtAuth/
        AlmightyShogun.AspNet.Localization/
        AlmightyShogun.AspNet.MaintenanceMode/
        AlmightyShogun.AspNet.RequestValidation/
        AlmightyShogun.ConsoleCommands/
        AlmightyShogun.Utils/
        AlmightyShogun.EntityFrameworkCore.ModelBuilding/
        AlmightyShogun.Hangfire.RecurringJobs/
        AlmightyShogun.Hosting.ConsoleLifetime/
        AlmightyShogun.Mail.Resend/
        AlmightyShogun.RemoteCommands/
        AlmightyShogun.Serilog/
    docs/
        .vitepress/
        guide/
        index.md
    .github/workflows/release.yml
    Directory.Packages.props
    package.json
    packages.sln
    wrangler.toml
```

## Package groups

The packages are small on purpose. Each owns one application concern and can be installed on its own when an application needs only that concern.

- ASP.NET packages cover the shared web layer, authentication, credential login, localization, request validation, and maintenance mode.
- Operations packages cover console commands, recurring Hangfire jobs, host lifetime behavior, remote command handling, and logging.
- Data packages cover Entity Framework Core model building and Resend email.
- `AlmightyShogun.Utils` provides the shared configuration, dependency-injection, JSON, and reflection helpers the others build on.

## Package dependencies

Project references are explicit in each `.csproj`. The arrows point from a package to the packages in this repository it depends on.

```text
AlmightyShogun.AspNet.Core
    -> AlmightyShogun.AspNet.Localization

AlmightyShogun.AspNet.CredentialAuth
    -> AlmightyShogun.Utils
    -> AlmightyShogun.AspNet.Core
    -> AlmightyShogun.AspNet.JwtAuth
    -> AlmightyShogun.AspNet.Localization
    -> AlmightyShogun.AspNet.RequestValidation

AlmightyShogun.AspNet.JwtAuth
    -> AlmightyShogun.Utils
    -> AlmightyShogun.AspNet.Core
    -> AlmightyShogun.AspNet.Localization

AlmightyShogun.AspNet.Localization
    -> AlmightyShogun.Utils

AlmightyShogun.AspNet.MaintenanceMode
    -> AlmightyShogun.Utils
    -> AlmightyShogun.AspNet.Core
    -> AlmightyShogun.AspNet.Localization

AlmightyShogun.AspNet.RequestValidation
    -> AlmightyShogun.AspNet.Core
    -> AlmightyShogun.AspNet.Localization

AlmightyShogun.ConsoleCommands
    -> AlmightyShogun.Utils

AlmightyShogun.Hangfire.RecurringJobs
    -> AlmightyShogun.Utils

AlmightyShogun.Hosting.ConsoleLifetime
    -> AlmightyShogun.Utils

AlmightyShogun.Mail.Resend
    -> AlmightyShogun.Utils

AlmightyShogun.RemoteCommands
    -> AlmightyShogun.Utils
```

`AlmightyShogun.Utils`, `AlmightyShogun.EntityFrameworkCore.ModelBuilding`, and `AlmightyShogun.Serilog` reference no other package here; they depend only on framework and NuGet references.

## Building

Build the full package solution from the repository root:

```sh
dotnet build packages.sln
```

Build one package when working inside it:

```sh
dotnet build packages/AlmightyShogun.Utils/AlmightyShogun.Utils.csproj
```

NuGet package versions are declared centrally in `Directory.Packages.props`, so a `.csproj` lists a `PackageReference` without a version. The release workflow builds and packs in dependency order before publishing to NuGet; local work should build and validate, never publish.

## Documentation

The documentation site is a Bun workspace under `docs/`. Install dependencies from the repository root and run the docs scripts through the root package scripts. The root `wrangler.toml` holds the Cloudflare deployment configuration the release workflow uses.

```sh
bun install
bun run docs:dev
bun run docs:build
```

Package documentation lives directly under `docs/{package-slug}/`. For example, `AlmightyShogun.AspNet.JwtAuth` is documented under `docs/asp-net-jwt-auth/`.

## Versioning

Packages are versioned together. A GitHub release tag becomes the package version CI uses when it builds, packs, and publishes every package. New XML documentation added during normal development uses `<since>Unreleased</since>` until the release process replaces it with the release version.
