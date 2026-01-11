# Utils
A simple C# library including some common utils I use in various projects.

## 📃 Prerequisites
- **[.NET](https://dotnet.microsoft.com/)**: >= 10

# 📦 Semantic Versioning (SemVer)
This project follows Semantic Versioning (SemVer), which uses a version format of `MAJOR.MINOR.PATCH`.

- **Patch** — Increases when backward-compatible bug fixes are made. These are small changes that address incorrect behavior without adding new features or functionality.
- **Minor** — Increases when new features or functionality are added in a backward-compatible manner. This includes adding new methods, classes, or capabilities that do not break existing code.
- **Major** — Increases when breaking changes are introduced that are not backward-compatible, such as modifying method signatures, removing functionality, or changing existing behavior in a way that may break dependent code.

# 🪵 Git
All commit messages, branches, issues and/or pull requests will be in English.

### Branches
- **Main** — This is the `main` branch. This contains the latest stable release and is the exact source running in production.
- **Development** — This is the `development` branch. This contains the latest staging release that is marked for deployment and is the exact source running on staging.
- **Feature** — This is a `feature/*` branch. This contains a new feature that will be added. Any feature should have its own branch. Once completed the branch should be merged into the `development` branch.
- **Bugfix** — This is a `bugfix/*` branch. This contains a bugfix that will be added. Any bugfix should have its own branch. Once completed the branch should be merged into the `development` branch.

### Commit messages
Commit messages are bound to the following template:
- `<type>: <message>`
- `<type>(feature): <message>`
- `<type>(feature): <message> [<issue-number>]`
