---
name: release
description: Create a stable GitHub release for the Almighty-Shogun/nuget-packages monorepo and let CI publish NuGet packages and docs. Use when the user asks to cut, create, publish, or prepare a release. Resolves major, minor, patch, or explicit semver versions, prepares XML documentation and changelog release metadata, runs safeguards and build checks, generates release notes, requires explicit confirmation, then creates the GitHub release. Never manually packs, publishes, pushes tags, or changes package versions locally.
---

# Release

Create a stable GitHub release for `Almighty-Shogun/nuget-packages`.

Publishing is CI-driven:

- `.github/workflows/release.yml` builds, packs, and publishes all NuGet packages to NuGet.org.
- `.github/workflows/release-docs.yml` builds and deploys the VitePress documentation.

Do not run `dotnet nuget push`, manually create tags, push tags, or change package versions locally for a release. The release tag is passed to `dotnet pack` as `PackageVersion` by CI.

The current workflows support stable releases only. Do not create beta or pre-release releases unless the workflows are updated first.

## Resolve Version

Accept:

- `major`
- `minor`
- `patch`
- an explicit stable semver version, such as `1.21.0`

Refresh tags:

```bash
git fetch origin --tags --prune
```

For `major`, `minor`, or `patch`, compute from the highest stable semver tag:

```bash
git tag -l | grep -E '^[0-9]+\.[0-9]+\.[0-9]+$' | sort -V | tail -1
```

Bump:

- `major`: `X+1.0.0`
- `minor`: `X.Y+1.0`
- `patch`: `X.Y.Z+1`

For an explicit version, validate:

```text
^[0-9]+\.[0-9]+\.[0-9]+$
```

Show the resolved version before continuing.

## Safeguards

Run:

```bash
git rev-parse -q --verify "refs/tags/<version>"
gh release view "<version>"
git status --porcelain
git rev-parse origin/main
gh repo view --json nameWithOwner -q .nameWithOwner
```

Rules:

- If the tag or GitHub release already exists, stop.
- If the working tree is dirty, warn that local changes are not included in a release cut from `origin/main`. Continue only if the user accepts that.
- Capture the `origin/main` SHA. The release must target that SHA.

## Prepare Release Metadata

Before running the release build checks, replace pending release markers when they exist.

Inspect:

```bash
rg -n "<since>Unreleased</since>" packages || true
```

Rules:

- New unreleased XML documentation uses `<since>Unreleased</since>` during normal development.
- For a release, replace exact `<since>Unreleased</since>` occurrences under `packages/` with `<since><version></since>`.
- Do not modify existing released `<since>` tags.
- Do not edit `.csproj` version metadata or create local package version commits.

After applying the replacements, show:

```bash
git diff --stat
git diff -- packages
```

Ask:

```text
Do you approve committing and pushing these release metadata changes?
```

If there are metadata changes and the user does not approve, stop before build checks and before release creation.

After approval, verify and commit only the metadata files that changed:

```bash
git diff --check
dotnet build packages.sln --configuration Release
git diff --name-only -- packages
git add -- <changed-metadata-files>
git diff --cached --name-status
git commit -m "chore: prepare release metadata for <version>"
git push
git fetch origin --tags --prune
git rev-parse origin/main
```

Capture the new `origin/main` SHA after the push. The release must target this updated SHA.

If there are no `<since>Unreleased</since>` markers, continue without creating a metadata commit.

## Build Check

Run the same local validation surface used for this repo:

```bash
dotnet build packages.sln --configuration Release
bun run docs:build
```

If any build fails, stop and show the failing command/output. Do not create a release.

Do not run `dotnet pack` locally unless the user explicitly asks for a local package artifact. CI packs every package with the release tag as the version.

## Release Notes

Generate release notes using the `release-notes` skill workflow:

- Base: latest published GitHub release tag.
- Head: `origin/main`.
- Compare link should end with `<base>...<version>` in the release body because the tag will exist after release creation.

Write the raw markdown body to a temporary file, such as:

```text
/tmp/nuget-packages-release-<version>.md
```

Show the generated notes to the user.

## Review Gate

Before creating the release, state:

- version
- release title, which must exactly match the version
- stable release
- target SHA
- comparison base

Show the exact `gh release create` command that will be run, then ask:

```text
Do you want me to create the release <version>?
```

Do not run `gh release create` until the user clearly approves.

## Create Release

After approval:

```bash
gh release create "<version>" \
    --target "<origin-main-sha>" \
    --title "<version>" \
    --notes-file "<notes-file>" \
    --latest
```

After creating the release, report:

- Release URL from `gh release view <version> --json url -q .url`.
- CI is now publishing NuGet packages and deploying docs.
- No local version commits, tags, pack artifacts, or manual publish commands were created.
