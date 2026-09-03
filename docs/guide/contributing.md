# Contributing

Contributions should stay focused on one package or one documentation area at a time. The source code is authoritative, so public API changes should be made in the package first and then reflected in the docs.

## Local setup

Clone the repository, install the documentation dependencies, and build the package solution once before making changes.

```sh
git clone https://github.com/Almighty-Shogun/nuget-packages.git
cd nuget-packages
bun install
dotnet build packages.sln
```

Use .NET 10 for package work. Bun is used for the VitePress documentation site.

## Package changes

Work package by package and keep namespaces aligned with the package name. Do not expose implementation-only classes or members just because they are useful internally.

When adding or changing public APIs:

- Add or update XML documentation on the source API.
- Use `<since>Unreleased</since>` for new XML documentation during normal development.
- Keep `/// <inheritdoc />` as the only XML documentation block when inherited docs are used.
- Update the matching package docs under `docs/{package-slug}/`.
- Build the solution before handing off the change.

```sh
dotnet build packages.sln
```

For a single-package check during active work, build that package directly:

```sh
dotnet build packages/AlmightyShogun.AspNet.Auth/AlmightyShogun.AspNet.Auth.csproj
```

## Documentation changes

Documentation is written manually after inspecting the current package source. Do not generate API docs from scripts or document private/internal implementation details.

Run the docs site locally while editing:

```sh
bun run docs:dev
```

Build the static VitePress site before handing off documentation changes:

```sh
bun run docs:build
```

When a public package API changes, update its page, sidebar entry, examples, type signatures, configuration pages, and dependency notes where relevant.

## Pull requests

Keep pull requests scoped and easy to review. Prefer more focused commits over one large commit that mixes package source, docs, project metadata, and workflow changes.

Before opening a pull request:

- Build the package solution when source or project files changed.
- Build the docs when documentation, VitePress config, or docs theme files changed.
- Keep unrelated dirty work out of the branch.
- Do not publish NuGet packages locally.
- Do not edit release metadata unless preparing an approved release.

## Release flow

Publishing is CI-driven. A GitHub release triggers `.github/workflows/release.yml`, which builds the packages, packs them with the release tag as the package version, pushes the `.nupkg` files to NuGet, then builds and deploys the VitePress documentation.

Release preparation replaces `<since>Unreleased</since>` markers in package XML documentation with the release version. That metadata change should be committed separately from normal feature or fix work.
