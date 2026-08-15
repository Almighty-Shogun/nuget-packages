# Monorepo

This repository contains the `AlmightyShogun.*` NuGet packages and the VitePress documentation site that describes them. The package source lives under `packages/`, the documentation lives under `docs/`, and `packages.sln` is the solution used to build every package together.

```text
nuget-packages/
    packages/
        AlmightyShogun.AspNet.CredentialAuth/
        AlmightyShogun.AspNet.JwtAuth/
        AlmightyShogun.AspNet.Maintenance/
        AlmightyShogun.AspNet.Utils/
        AlmightyShogun.AspNet.Validation/
        AlmightyShogun.ConsoleCommands/
        AlmightyShogun.EntityFrameworkCore.Utils/
        AlmightyShogun.Hangfire.Utils/
        AlmightyShogun.Hosting.Utils/
        AlmightyShogun.Logging/
        AlmightyShogun.RemoteCommands/
        AlmightyShogun.Resend.Utils/
        AlmightyShogun.Utils/
    docs/
        .vitepress/
        guide/
        index.md
    .github/workflows/release.yml
    package.json
    packages.sln
    wrangler.toml
```

## Package groups

The packages are small on purpose. Each one owns a specific application concern and can be installed independently when an application only needs that concern.

- ASP.NET packages cover authentication, login, request helpers, validation, and maintenance mode.
- Operations packages cover console commands, recurring Hangfire jobs, hosting helpers, logging, and remote command handling.
- Data and messaging packages cover Entity Framework Core model-building helpers and Resend email helpers.
- Core utilities provide shared configuration, dependency-injection, JSON, reflection, and console helpers used by other packages.

## Package dependencies

Project references are kept explicit in each `.csproj`. The arrows below point from a package to the internal packages it depends on.

```text
AlmightyShogun.AspNet.CredentialAuth
    -> AlmightyShogun.AspNet.JwtAuth
    -> AlmightyShogun.AspNet.Utils
    -> AlmightyShogun.AspNet.Validation

AlmightyShogun.AspNet.JwtAuth
    -> AlmightyShogun.AspNet.Utils
    -> AlmightyShogun.Utils

AlmightyShogun.AspNet.Maintenance
    -> AlmightyShogun.Utils

AlmightyShogun.AspNet.Validation
    -> AlmightyShogun.AspNet.Utils

AlmightyShogun.ConsoleCommands
    -> AlmightyShogun.Logging
    -> AlmightyShogun.Utils

AlmightyShogun.Hangfire.Utils
    -> AlmightyShogun.Utils

AlmightyShogun.RemoteCommands
    -> AlmightyShogun.Logging
    -> AlmightyShogun.Utils

AlmightyShogun.Resend.Utils
    -> AlmightyShogun.Logging
    -> AlmightyShogun.Utils
```

Packages not listed in that map either depend only on framework/package references or are the base packages other projects reference.

## Building

Build the full package solution from the repository root:

```sh
dotnet build packages.sln
```

Build one package when you are only working inside that package:

```sh
dotnet build packages/AlmightyShogun.Utils/AlmightyShogun.Utils.csproj
```

The release workflow builds and packs packages in dependency order before publishing them to NuGet. Local development should build and validate, but publishing is handled by GitHub Actions.

## Documentation

The documentation site is a Bun workspace under `docs/`. Install dependencies from the repository root and run the docs scripts through the root package scripts. The root `wrangler.toml` contains the Cloudflare deployment configuration used by the release workflow.

```sh
bun install
bun run docs:dev
bun run docs:build
```

Package documentation lives directly under `docs/{package-slug}/`. For example, `AlmightyShogun.AspNet.JwtAuth` is documented under `docs/asp-net-jwt-auth/`.

## Versioning

Packages are versioned together. A GitHub release tag becomes the package version used by CI when it builds, packs, and publishes every package. New XML documentation added during normal development uses `<since>Unreleased</since>` until the release process replaces it with the release version.
