# Repository Instructions

These instructions apply to the whole repository. More specific instructions in subdirectories override this file.

## Repository

The `Almighty-Shogun/nuget-packages` monorepo: C#/.NET NuGet packages under `packages/`, and a VitePress documentation site under `docs/`. Packages target `.NET 10` and are versioned together, CI using the release tag as the version for every package.

## Package Workflow

- Source code is authoritative. Work package by package in the order shown by `find packages -maxdepth 2 -name '*.csproj' | sort`.
- Preserve unrelated user changes. The worktree may be dirty.
- Do not document or expose private, internal, or implementation-only APIs.
- Keep package namespaces aligned with the package name unless the user explicitly asks otherwise.
- `<since>` records when a member was **added**, never when it was last changed. A new member gets `<since>Unreleased</since>` until the release process replaces it; reordering parameters, changing a signature, or rewriting a body leaves it alone.
- A `<since>` below `2.0.0` predates this monorepo. Each package was its own repository until the `2.0.0` move, so those versions have no matching tag here and are correct as written.
- Do not add `<author>` or `<since>` below an `/// <inheritdoc />`.
- When moving experimental code out of a package, preserve it in a temporary directory instead of deleting it.

## XML Documentation Style

The **XML Documentation Style** section in my global instructions and the `/csharp-docs` skill are together the single standard for every package and every API; do not let a package drift its own way. The global section carries the rules that apply to any C# edit, `/csharp-docs` carries the tag mechanics and the audit. The only binding recorded here: `max_line_length` is **140 columns**, counting the indentation and the `///` prefix.

Check a package with:

```sh
dotnet build packages/<Package>/<Package>.csproj --no-incremental
awk 'length($0)>140 {print FILENAME":"FNR}' packages/<Package>/*.cs
```

The build covers completeness and cref resolution, since `GenerateDocumentationFile` is on with no `NoWarn`. Neither check can read a sentence, so a package whose build is clean may still be wrong on every claim it makes. Accuracy is checked by reading: run `/csharp-docs packages/<Package> --verify` for a full pass, which reads each block against the implementation and reports what it could not confirm. At minimum, whenever a body changes, re-read that member's whole doc block and correct what the change falsified.

## Build And Validation

```sh
dotnet build packages.sln
bun run docs:build
```

Run the solution build after source or project-file changes, and the docs build after documentation, VitePress config, or docs theme changes. Do not run publish commands locally; `.github/workflows/release.yml` publishes to NuGet when a GitHub release is published.

## Documentation

Documentation lives in `docs/` and uses VitePress. `docs/CLAUDE.md` owns all documentation structure, style, and validation rules. High-level:

- Pages live directly under `docs/{package-slug}/`, the slug being lowercase kebab case, so `AlmightyShogun.AspNet.Auth.Credentials` becomes `asp-net-auth-credentials`.
- Write documentation manually after inspecting the current package source. Do not use a generation script.
- Every documented public API page must be reachable from the VitePress sidebar.
- Do not silently change documentation conventions. Explain broad convention changes before applying them.

## Releasing

User-level release skills should use these repository overrides.

- Publishing and documentation deployment are CI-driven by `.github/workflows/release.yml`.
- Before releasing, replace exact `<since>Unreleased</since>` markers under `packages/` with `<since><version></since>`, and commit only those with `chore: prepare release metadata for <version>`.
- Release checks: `dotnet build packages.sln --configuration Release` and `bun run docs:build`.
