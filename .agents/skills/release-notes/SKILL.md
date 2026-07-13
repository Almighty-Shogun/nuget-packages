---
name: release-notes
description: Generate GitHub release notes or a changelog for the Almighty-Shogun/nuget-packages monorepo by diffing a base ref against origin/main. Use when the user asks for release notes, a changelog, or a summary of changes for an upcoming or specific release. This skill is read-only and never creates tags, releases, commits, packs, or publishes packages.
---

# Release Notes

Generate release notes for `Almighty-Shogun/nuget-packages`.

This workflow is read-only. Do not create tags, releases, commits, local package artifacts, or publish packages.

## Resolve Range

Refresh tags when needed:

```bash
git fetch origin --tags --prune
```

Use:

- Head: `origin/main`.
- Default base: latest published GitHub release tag.
- Explicit base: the tag or ref provided by the user.

Useful commands:

```bash
gh release view --json tagName -q .tagName
git rev-parse "<base>"
git rev-parse origin/main
gh repo view --json nameWithOwner -q .nameWithOwner
```

If `<base>` and `origin/main` point to the same commit, report that there are no changes and stop.

## Gather Changes

Run:

```bash
git log --oneline --no-merges <base>..origin/main
git diff --name-status <base>..origin/main -- 'packages/**' 'docs/**' '.github/**' 'AGENTS.md' '.agents/**'
git diff --stat <base>..origin/main
```

Read relevant diffs before describing behavior. Do not rely only on commit subjects when a change affects public APIs, package dependencies, package metadata, workflows, or documentation structure.

## Classify

Group changes by package or repo area first. Then classify the entries inside each group.

Package groups:

- `AlmightyShogun.AspNet.JwtAuth`: `packages/AlmightyShogun.AspNet.JwtAuth` and `docs/asp-net-jwt-auth`.
- `AlmightyShogun.AspNet.Utils`: `packages/AlmightyShogun.AspNet.Utils` and `docs/asp-net-utils`.
- `AlmightyShogun.ConsoleCommands`: `packages/AlmightyShogun.ConsoleCommands` and `docs/console-commands`.
- `AlmightyShogun.EntityFrameworkCore.Utils`: `packages/AlmightyShogun.EntityFrameworkCore.Utils` and `docs/ef-core-utils`.
- `AlmightyShogun.Hangfire.Utils`: `packages/AlmightyShogun.Hangfire.Utils` and `docs/hangfire-utils`.
- `AlmightyShogun.Hosting.Utils`: `packages/AlmightyShogun.Hosting.Utils` and `docs/hosting-utils`.
- `AlmightyShogun.Logging`: `packages/AlmightyShogun.Logging` and `docs/logging`.
- `AlmightyShogun.RemoteCommands`: `packages/AlmightyShogun.RemoteCommands` and `docs/remote-commands`.
- `AlmightyShogun.Resend.Utils`: `packages/AlmightyShogun.Resend.Utils` and `docs/resend-utils`.
- `AlmightyShogun.Utils`: `packages/AlmightyShogun.Utils` and `docs/utils`.
- `Documentation`: docs-wide changes that are not specific to one package.
- `Build and release`: workflows, publishing, package metadata, dependency wiring, and CI/deployment changes.
- `Internal`: repo maintenance that is worth mentioning but not tied to a public package API.

Use these subsection labels inside each group when they have content:

- Breaking changes: removed public APIs, renamed public APIs, changed signatures, changed setup requirements, removed package behavior, or workflow changes that alter publishing/deployment.
- New APIs: added public classes, interfaces, records, attributes, extension methods, methods, or packages.
- Features: new behavior in existing public APIs.
- Fixes: corrected behavior, package metadata, dependencies, documentation, publishing, or build issues.
- Documentation: meaningful documentation structure or content updates.
- Internal: dependency bumps, formatting, private implementation cleanup, or agent instruction changes. Omit these unless they materially affect users or maintainers.

## Render

Output one copy-paste-ready markdown block in English. Use package-first grouping:

```markdown
## `AlmightyShogun.ConsoleCommands`

### Breaking changes

- Changed command discovery to use class-level command attributes.

### Fixes

- Fixed service registration for command handlers.

## Documentation

### Documentation

- Updated API reference pages for the new command structure.

## Build and release

### Fixes

- Updated the release workflow to build every package before packing.

**Full Changelog:** https://github.com/Almighty-Shogun/nuget-packages/compare/<base>...origin/main
```

Rules:

- Only include package or repo-area groups that have content.
- Only include subsections with content.
- Put package-specific documentation changes under that package when possible.
- Put cross-package VitePress, guide, or agent documentation changes under `Documentation`.
- Put workflow, CI, package publishing, and repository release changes under `Build and release`.
- Put pure cleanup under `Internal` only when it is worth mentioning to users or maintainers.
- Do not use em dashes or en dashes.
- Use complete sentences.
- Build the compare link from the resolved repo slug and refs.
- After the markdown block, briefly state the resolved range and any important caveats.
