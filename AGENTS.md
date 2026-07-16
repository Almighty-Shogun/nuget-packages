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
- When adding a new method, add XML code documentation and mark it with `<since>Unreleased</since>` until the release process replaces it with the release version.
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

## Git And PR Safety

- Do not use `git add .`, `git add -A`, `git reset --hard`, force pushes, rebases, or destructive cleanup unless the user explicitly asks.
- Inspect untracked files before including them in a commit.
- Keep commits focused and use the repository's commit/PR skills when requested.
- If a command needs to write to `.git` and the sandbox blocks it, request escalation instead of working around Git metadata.
