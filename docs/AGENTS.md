# Documentation Instructions

These instructions apply to all VitePress documentation work under `docs/`. Read the current package source and existing documentation before making changes. The source code is authoritative.

## Workflow

- Work package by package in the order under `packages/`.
- Do not generate documentation with a generation script. Inspect and write each page deliberately.
- Do not rewrite an already approved package unless the user asks for a global change or its source API changed.
- Compare the current public package surface with its documentation before editing.
- Document only APIs intended for package consumers. Do not document private, internal, or implementation-only types and members.
- Public interfaces that only support internal implementation do not need separate pages.
- When a public interface is the dependency-injection contract, document it as the consumer-facing service. Use a clean service name without the leading `I` in the page title, sidebar label, and file name, but keep examples and type signatures using the real C# interface type.
- Do not create duplicate class and interface pages for the same DI surface. If a concrete class only exists as the implementation behind a DI contract, document the service contract instead. When source changes are in scope, prefer making implementation-only concrete/base classes internal.
- Preserve unrelated user changes.

## Documentation Structure

Documentation lives directly under `docs/{package-slug}/`, never under `docs/packages/`.

Package slugs use lowercase kebab case:

- `AlmightyShogun.AspNet.JwtAuth` becomes `asp-net-jwt-auth`
- `AlmightyShogun.EntityFrameworkCore.Utils` becomes `ef-core-utils`
- `AlmightyShogun.Resend.Utils` becomes `resend-utils`

Each package normally contains:

```text
docs/{package}/
  index.md
  installation.md
  configuration.md
  configuration/{configuration-name}.md
  attributes/{attribute-name}.md
  validation-rules/{rule-family}.md
  constants/{constant-name}.md
  extensions/{api-name}.md
  records/{record-name}.md
  services/{service-name}.md
  types/{type-name}.md
```

Use meaningful categories such as `attributes`, `configuration`, `constants`, `extensions`, `records`, `services`, `types`, and package-specific categories such as `validation-rules`. Do not introduce separate `classes` and `interfaces` groups for new or migrated documentation unless the user explicitly asks for that structure.

Service pages document consumer-facing DI contracts and the behavior of the registered implementation in one place. The route and page title use the service name without the interface prefix, for example `IAppHostResolver` is documented at `services/app-host-resolver.md` with `# AppHostResolver`. Examples and type signatures still use `IAppHostResolver`.

Service methods usually stay on the same service page as `## MethodName` sections. Do not create separate method pages for small or moderate services. Only split service methods into separate pages when the page becomes too large and separate pages clearly improve navigation.

Extension methods use one page per public extension method at `docs/{package}/extensions/{extension-method-name}.md`. Keep overloads of the same extension method on that method page. Do not document extension classes as their own pages unless a real method-name collision requires it and the user agrees.

When overloads have different receiver types, registration targets, or usage paths, keep them on one page and split the page into clear `## OverloadFamily` sections. Use the `AddCustomLogging` page style for this: one `# MethodName` page, one `##` section per overload family, usage under each section, and `### Type signature` under each section. Do not create separate pages for overloads of the same extension method.

Public non-DI classes, structs, records, and values can use focused categories such as `records`, `constants`, or `types`. Use `types` when a package exposes public types that do not fit a more specific category.

Avoid duplicate pages for the same API. Overloads of the same method belong on one method page or one method section.

Packages that expose a very large set of closely related validation-rule APIs may use grouped family pages instead of one page per rule attribute or fluent rule method. Each public rule API must still be documented exactly once on the relevant family page, with clear usage, behavior notes, and type signatures. Use this only when separate pages would create repetitive, low-value navigation.

## Validation Rule Documentation

`AlmightyShogun.AspNet.Validation` uses grouped rule-family pages because validation attributes and fluent rule methods expose a large, repetitive public surface.

Validation rule family pages under `docs/asp-net-validation/validation-rules/` use:

````md
# Rule Family

Family description.

## RuleName

Rule behavior, when to use it, and important constraints.

::: code-group

```csharp [Attribute.cs]
[AttributeName(parameter shape)]

[AttributeName(real, values)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Value)
    .RuleName(real, values);
```

:::
````

Rules:

- Document every public validation attribute exactly once on the relevant family page.
- Document every public fluent validation rule method exactly once on the relevant family page.
- Document `CustomRule` on `docs/asp-net-validation/custom-rules.md`, not inside a rule family, because it needs implementation guidance for the DI-resolved rule type and optional custom attribute wrapper.
- Use one `## RuleName` section per validation rule.
- Do not use shared rule tables or shared `## Type signature` blocks on validation rule family pages.
- For no-argument attributes, show only the real attribute usage once, for example `[Required]`.
- For attributes with arguments, show the constructor-shaped attribute first, then a blank line, then a concrete real usage.
- Use `::: code-group` with `[Attribute.cs]` and `[FluentRule.cs]` when both APIs exist.
- If a rule is fluent-only or attribute-only, document the existing public API and call out the missing counterpart as a package parity question before release.
- Do not add label comments such as `// Real example` inside the code block.

The `docs/asp-net-validation/fluent-validation.md` page documents `ValidatableRequest<TRequest>` and general fluent-rule behavior:

- Keep one main request example near the top.
- Do not duplicate the full rule catalog on this page.
- Link to the validation rule family pages for concrete rule examples.

## Navigation

- Update the matching file in `docs/.vitepress/config/menu/`.
- Every package must be available from the top navigation package dropdown.
- Every API page must be reachable from its package sidebar.
- Introduction, installation, and package-level configuration links stay in the first non-collapsible group.
- Sidebar groups use this order when present: package pages, `Configuration`, `Extensions`, package-specific groups such as `Validation Rules`, `Attributes`, `Services`, `Types`, `Records`, `Constants`.
- Package-specific guide pages, such as Logging's `Formatter`, stay in the first group after `Configuration`.
- Category and API groups use `collapsed: false` so they are collapsible and initially open.
- Use human-readable labels and slugified links.
- Attribute sidebar labels omit the `Attribute` suffix. For example, `AuthPermissionAttribute` appears as `AuthPermission` in the sidebar.
- DI service sidebar labels and routes omit the leading interface `I`. For example, `IAppHostResolver` appears as `AppHostResolver` and uses `services/app-host-resolver`.
- Service pages are usually a single sidebar item. Do not list every method as nested sidebar children unless the service page has been intentionally split.

## Writing Requirements

- Write specific descriptions that explain what the API does, when to use it, relevant behavior, failure cases, defaults, and constraints.
- Do not add filler sections such as `Overview`, `Details`, or generic next steps.
- Do not add an `Importing` or namespace section.
- Do not start pages with package/category metadata.
- Use current C# terminology and syntax. Nullable types use `string?`, not `string | null`.
- Include practical, copy-paste-ready examples. Never use placeholder comments such as `// Use XXX from application code after installing the package.`
- Include all required `using` statements in examples and order them from shortest line to longest line.
- Chain extension-method registrations when the APIs return the same builder or service collection and chaining is applicable.
- Link references to documented APIs when they appear in prose. Use relative links within the same package, absolute docs-root links for other packages, and do not link an API to its own page.
- Use `::: code-group` when an example needs multiple files, configuration plus code, or multiple valid setup forms.
- Give code-group files meaningful names such as `[Program.cs]`, `[appsettings.json]`, or `[ExampleSettings.cs]`.
- Do not place unrelated classes in one code block. An interface and its implementation may share a block only when that makes the specific example clearer.

## Package Pages

Package introductions use the package name as the H1 and contain:

- A clear description of the package.
- A `## Categories` list.
- Category links followed by `&mdash;` and a concise description.
- Categories use the same order as the package sidebar: `Configuration`, `Extensions`, package-specific groups such as `Validation Rules`, `Attributes`, `Services`, `Types`, `Records`, `Constants`.
- Package-specific guide pages, such as Logging's `Formatter`, appear after `Configuration` in the package introduction category list when present.
- A short practical example.
- Dependency/runtime notes where useful.

Do not repeat the package name as the first words of the introduction paragraph.

Installation pages:

- Show only the `dotnet` CLI installation command in one shell code block.
- Explain the target framework and runtime expectations.
- Add `## Dependencies` and list actual package, framework, and project dependencies with their current versions.
- Split dependencies into `### Framework references`, `### Package references`, and `### Project references`; omit groups that do not apply.
- Read dependency information from the current `.csproj` files and central package management files when present. Do not reuse stale dependency versions from existing docs.
- Show startup registration once when required.
- When configuration is required, use this warning style with the actual section name:

```md
::: warning
Requires an `Example` section in application configuration, usually from `appsettings.json`.
:::
```

The Logging package is the only package that does not use this required-configuration warning because its configuration is optional.

## Configuration

When a package binds configuration from `appsettings.json`:

- Add `docs/{package}/configuration.md`.
- Show the complete JSON shape.
- Add each configuration record under `docs/{package}/configuration/{configuration-name}.md`.
- Link the package introduction directly to the configuration type category, for example `./configuration/auth-settings`; do not add separate “Configuration” and “Configuration types” categories.
- Use `fields` frontmatter on dedicated configuration type pages.
- Do not repeat the same field table on the package-level `configuration.md` page when a dedicated configuration type page exists. The package-level page should show the JSON shape and any package-level behavior notes, but do not add generic cross-reference lines such as “See ConfigName for field descriptions and defaults.”
- If a package has a package-level configuration page but no dedicated configuration type page, `fields` may live on `configuration.md`.
- Include every configuration field with `name`, `description`, `type`, and `default` when a real default exists.
- Do not repeat the full JSON block on individual configuration type pages. Under `## Usage`, place the configuration-page note in a `::: tip` before the code example, for example:

```md
::: tip
The JSON shape is documented on the [configuration page](../configuration). The example below shows how application services can consume the already-bound options.
:::
```

Example:

```yaml
fields:
    - name: LocalhostApp
      description: Application audience used for `localhost` requests during development.
      type: string?
      default: 'null'
```

If a startup method accepts `builder.Configuration`, explain which section it requires and include the warning shown above.

## Standalone API Page Schema

Use this order for standalone method, extension, attribute, constructor-like record, and other API pages:

````md
---
params:
    - name: value
      description: Value to process.
      type: string
      default: 'null'

returns: The transformed value.
---

# ApiName

Useful description.

## Usage

```csharp
using Company.Package;

var result = ApiName.Run();
```

<FrontmatterDocs/>

## Type signature

```csharp
public string ApiName(string value);
```
````

Rules:

- Keep a blank line between the closing frontmatter delimiter and the H1.
- Omit `params` when the API has no parameters.
- Omit `returns` only when the API truly returns `void`.
- Always include `<FrontmatterDocs/>` when `params`, `returns`, or `fields` exists.
- Do not write manual `## Parameters`, `## Returns`, or `## Fields` sections.
- Returns remain a single descriptive string, not an object with name/type fields.
- Use `default` for actual C# default values. Use quoted `'null'`, `'true'`, `'false'`, `'[]'`, or `'0'` where YAML parsing requires it.
- Nullable does not automatically mean optional. Document the actual method default to show optional parameters.
- Keep generic type commas, for example `Dictionary<TKey, TValue>`.
- Inline backticks in descriptions are expected and rendered through the shared `renderInlineCode` utility.

## Service Page Schema

Use service pages for DI contracts and their registered implementation behavior. Service pages do not use frontmatter-driven parameter/return tables for each method. Explain parameters, return behavior, defaults, failure cases, and constraints in the method prose.

Use this order:

````md
# AppHostResolver

Clear description of the service, what it resolves or controls, and where the package uses it.

Application code should depend on `IAppHostResolver`. Explain how it is registered and what configuration or package behavior it relies on.

## Usage

```csharp
using Company.Package;

public sealed class CurrentAppService(IAppHostResolver appHostResolver)
{
    public string GetCurrentApp(string host)
        => appHostResolver.ResolveAppFromHost(host);
}
```

## ResolveAppFromHost

Explain what the method does, when to use it, what it returns, and what it throws or rejects.

```csharp
using Company.Package;

public sealed class AppScopedService(IAppHostResolver appHostResolver)
{
    public string GetAppForRequestHost(string host)
        => appHostResolver.ResolveAppFromHost(host);
}
```

### Type signature

```csharp
public string ResolveAppFromHost(string? host);
```
````

Rules:

- Use `# ServiceName` without the leading `I`, even when the exported API is an interface.
- Use the real interface type in examples and type signatures.
- Put each method directly under `## MethodName`; do not add an extra `## Methods` wrapper.
- Put each method signature under `### Type signature`.
- Keep service pages concise but complete. Split only when a service is too large to scan comfortably.

## Type Page Schema

Use type pages for public non-DI classes, structs, records, and utility types that are best understood as one small surface. These pages do not need frontmatter-driven parameter or return tables for each method when method sections are clearer.

Use this order:

````md
# ConsoleUtils

Clear description of the type, what it groups together, and when application code should use it.

## Usage

```csharp
using Company.Package;

ConsoleUtils.RemoveLastLine();
```

## MethodName

Explain what the method does, when to use it, return behavior, defaults, failure cases, and constraints.

```csharp
using Company.Package;

string answer = await ConsoleUtils.AskQuestionAsync("Name?", "Worker");
```

### Type signature

```csharp
public static Task<string> AskQuestionAsync(
    string question,
    string? defaultValue = null
);
```
````

Rules:

- Keep small type method sections on the type page when that is easier to scan than separate method pages.
- Put each method directly under `## MethodName`; do not add an extra `## Methods` wrapper.
- Put each method signature under `### Type signature`.
- Use frontmatter-driven standalone API pages for constructor-like records or simple values when parameters/returns/fields should be rendered once for the whole API.
- Extension methods are still documented as one page per public extension method under `extensions/`.

## Type Signatures

- End signatures with `;`.
- Keep short signatures on one line.
- Wrap long signatures only when needed to avoid horizontal scrolling:

```csharp
public AuthenticationBuilder AddJwtBearerAuthentication(
    IConfiguration configuration
);
```

- Keep generic constraints on the closing parameter line:

```csharp
public ModelBuilder ApplyAutoInclude<TEntity>(
    Expression<Func<TEntity, object?>> navigation
) where TEntity : class;
```

- Do not add type declarations to overview pages. On service pages, put method signatures inside the relevant method section under `### Type signature`.

## Special Package Conventions

### Logging

- Configuration is optional; do not show the required-configuration warning.
- Keep formatter documentation on its own page.
- Document formatter colors in a table.
- Installation startup registration uses a code group for `[IServiceCollection.cs]` and `[IHostBuilder.cs]`.

### Resend Utils

- The DI contract is `IResendMailService`.
- Mail examples commonly need separate template and caller files; use code groups.
- Document the required `mail` template files on installation.

### Remote Commands

- Command message and response records belong in separately named code-group blocks.
- `RemoteCommand<T>` methods include `HandleCommandAsync` and protected `WriteResponseAsync`.

### Console Commands

- Commands are class-based. A command class has `ConsoleCommandAttribute` and exactly one public `ExecuteAsync` method returning `Task`.
- One command class can define a command with zero parameters or parameters; examples should reflect the current class-based runtime.

## Validation

After documentation changes:

1. Run `bun run docs:build` from the repository root.
2. Run `dotnet build packages.sln` when source or project files changed.
3. Search authored docs, excluding `docs/node_modules`, for:
   - old type names and namespaces;
   - broken or stale slugs;
   - duplicate class/interface pages for the same DI surface;
   - `classes` or `interfaces` groups introduced where `services` should be used;
   - attribute sidebar labels that still include the `Attribute` suffix;
   - manual Parameters/Returns sections;
   - `outline: deep` frontmatter;
   - API pages with frontmatter but no `<FrontmatterDocs/>`;
   - missing blank lines after frontmatter;
   - TypeScript-style nullable/union types;
   - signatures missing semicolons;
   - generic constraints placed on a separate line;
   - placeholder or unusable examples.
4. Confirm sidebar links resolve and every API page is reachable.
5. Compare the documented API list against current consumer-facing source APIs.
6. Confirm internal/private implementation APIs are not documented.

Do not silently change documentation conventions. If current files conflict with these instructions or a broad convention change appears necessary, explain it to the user before applying it.
