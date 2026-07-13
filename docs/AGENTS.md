# Documentation Instructions

These instructions apply to all VitePress documentation work under `docs/`. Read the current package source and existing documentation before making changes. The source code is authoritative.

## Workflow

- Work package by package in the order under `packages/`.
- Do not generate documentation with a generation script. Inspect and write each page deliberately.
- Do not rewrite an already approved package unless the user asks for a global change or its source API changed.
- Compare the current public package surface with its documentation before editing.
- Document only APIs intended for package consumers. Do not document private, internal, or implementation-only types and members.
- Public interfaces that only support internal implementation do not need separate pages. When a public interface is the dependency-injection contract, document the consumer-facing interface and explain that users should inject it instead of the implementation.
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
  configuration/{configuration-name}/index.md
  {category}/{api-name}.md
  classes/{class-name}/index.md
  classes/{class-name}/{method}.md
```

Use meaningful categories such as `attributes`, `classes`, `configuration`, `constants`, `extensions`, `interfaces`, and `records`.

Class pages explain the class, intended use, construction/inheritance requirements, and list its documented methods. Every consumer-facing method gets its own page under the class directory. Do not add a type-signature section to class overview pages.

Avoid duplicate pages for the same API. Overloads of the same method belong on one method page.

## Navigation

- Update the matching file in `docs/.vitepress/config/menu/`.
- Every package must be available from the top navigation package dropdown.
- Every API page must be reachable from its package sidebar.
- Introduction, installation, and package-level configuration links stay in the first non-collapsible group.
- Category and API groups use `collapsed: true` so they are collapsed by default.
- Use human-readable labels and slugified links.

## Writing Requirements

- Write specific descriptions that explain what the API does, when to use it, relevant behavior, failure cases, defaults, and constraints.
- Do not add filler sections such as `Overview`, `Details`, or generic next steps.
- Do not add an `Importing` or namespace section.
- Do not start pages with package/category metadata.
- Use current C# terminology and syntax. Nullable types use `string?`, not `string | null`.
- Include practical, copy-paste-ready examples. Never use placeholder comments such as `// Use XXX from application code after installing the package.`
- Include all required `using` statements in examples and order them from shortest line to longest line.
- Chain extension-method registrations when the APIs return the same builder or service collection and chaining is applicable.
- Use `::: code-group` when an example needs multiple files, configuration plus code, or multiple valid setup forms.
- Give code-group files meaningful names such as `[Program.cs]`, `[appsettings.json]`, or `[ExampleSettings.cs]`.
- Do not place unrelated classes in one code block. An interface and its implementation may share a block only when that makes the specific example clearer.

## Package Pages

Package introductions use the package name as the H1 and contain:

- A clear description of the package.
- A `## Categories` list.
- Category links followed by `&mdash;` and a concise description.
- A short practical example.
- Dependency/runtime notes where useful.

Do not repeat the package name as the first words of the introduction paragraph.

Installation pages:

- Show only the `dotnet` CLI installation command in one shell code block.
- Explain the target framework and runtime expectations.
- Add `## Dependencies` and list actual package, framework, and project dependencies with their current versions.
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
- Add each configuration record under `docs/{package}/configuration/{configuration-name}/index.md`.
- Link the package introduction directly to the configuration type category, for example `./configuration/auth-settings`; do not add separate “Configuration” and “Configuration types” categories.
- Use `fields` frontmatter on the package configuration page and configuration type pages.
- Include every configuration field with `name`, `description`, `type`, and `default` when a real default exists.
- Do not repeat the full JSON block on individual configuration type pages; link to the package configuration page instead.

Example:

```yaml
fields:
    - name: LocalhostApp
      description: Application audience used for `localhost` requests during development.
      type: string?
      default: 'null'
```

If a startup method accepts `builder.Configuration`, explain which section it requires and include the warning shown above.

## API Page Schema

Use this order for method, extension, attribute, constructor-like record, and other API pages:

````md
---
outline: deep

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

## Uses

- [OtherApi](../category/other-api)
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
public void ApplyAutoInclude<TEntity>(
    Expression<Func<TEntity, object?>> navigation
) where TEntity : class;
```

- Do not add type declarations to class overview pages.

## Uses Sections

`## Uses` means APIs used internally by the documented API, not APIs that call it.

- Add it only when the implementation directly uses another documented public API.
- Link to the relevant public API page.
- Do not list private/internal implementation details.
- Verify implementations rather than inferring uses from examples.

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

1. Run `npm run build` from `docs/`.
2. Run `dotnet build packages.sln` when source or project files changed.
3. Search authored docs, excluding `docs/node_modules`, for:
   - old type names and namespaces;
   - broken or stale slugs;
   - manual Parameters/Returns sections;
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
