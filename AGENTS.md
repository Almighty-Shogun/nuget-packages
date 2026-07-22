# Repository Instructions

These instructions apply to the whole repository. More specific instructions in subdirectories override this file.

## Repository

This is the `Almighty-Shogun/nuget-packages` monorepo. It contains multiple C#/.NET NuGet packages under `packages/` and a VitePress documentation site under `docs/`.

Packages currently target `.NET 10` and are versioned together. A release tag is used by CI as the package version for every package.

## Package Workflow

- Treat source code as authoritative.
- Work package by package in the order shown by `find packages -maxdepth 2 -name '*.csproj' | sort`.
- Preserve unrelated user changes. The worktree may be dirty.
- Do not document or expose private, internal, or implementation-only APIs.
- Keep package namespaces aligned with the package name unless the user explicitly asks otherwise.
- When adding or updating XML code documentation during normal development, use `<since>Unreleased</since>` until the release process replaces it with the release version.
- When using `/// <inheritdoc />`, do not add `<author>` or `<since>` below it.
- When moving experimental code out of a package, preserve it in a temporary directory instead of deleting it.

## Build And Validation

Use these checks when relevant:

```sh
dotnet build packages.sln
bun run docs:build
```

Run `dotnet build packages.sln` after source or project-file changes. Run `bun run docs:build` after documentation, VitePress config, or docs theme changes.

Do not run publish commands locally. NuGet publishing is handled by `.github/workflows/release.yml` when a GitHub release is published.

## Documentation

Documentation lives in `docs/` and uses VitePress. Follow `docs/AGENTS.md` for all documentation structure, style, and validation rules.

Important high-level rules:

- Documentation lives directly under `docs/{package-slug}/`.
- Package slugs are lowercase kebab case, for example `AlmightyShogun.AspNet.JwtAuth` becomes `asp-net-jwt-auth`.
- Write documentation manually after inspecting the current package source. Do not use a generation script.
- Every documented public API page must be reachable from the VitePress sidebar.
- Do not silently change documentation conventions. Explain broad convention changes before applying them.

## Releasing

User-level release skills should use these repository overrides.

- Publishing and documentation deployment are CI-driven by `.github/workflows/release.yml`.
- Before releasing, replace exact `<since>Unreleased</since>` markers under `packages/` with `<since><version></since>`.
- Commit only release-marker changes with `chore: prepare release metadata for <version>`.
- Release checks: `dotnet build packages.sln --configuration Release` and `bun run docs:build`.
