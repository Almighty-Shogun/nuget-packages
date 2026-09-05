# Documentation Instructions

Applies to all VitePress documentation under `docs/`. Read the current package source and the existing pages before editing; the source is authoritative.

## Workflow

- Work package by package in the order under `packages/`. Write every page by hand after inspecting the source; never generate documentation with a script.
- Compare the current public surface against the existing documentation before editing. Do not rewrite an approved package unless the user asks for a global change or its source API changed.
- Preserve unrelated user changes.
- Document only APIs intended for package consumers. Never document private, internal, or implementation-only types and members, including public interfaces that exist only to support internal implementation.
- A public interface that is the DI contract is documented as the consumer-facing service, under the naming in **Structure** and **Navigation**.
- Never create both a class page and an interface page for one DI surface. When source changes are in scope, prefer making implementation-only concrete and base classes internal.
- Do not silently change these conventions. If the current files conflict with this file, or a broad convention change looks necessary, explain it to the user before applying it.

## Structure

Pages live directly under `docs/{package-slug}/`, never under `docs/packages/`. Slugs are lowercase kebab case: `AlmightyShogun.AspNet.Auth.Credentials` becomes `asp-net-auth-credentials`, `AlmightyShogun.EntityFrameworkCore.ModelBuilding` becomes `ef-core-model-building`, `AlmightyShogun.Mail.Resend` becomes `mail-resend`.

```text
docs/{package}/
  index.md
  installation.md
  configuration.md
  attributes/{attribute-name}.md
  validation-rules/{rule-family}.md
  constants/{constant-name}.md
  extensions/{api-name}.md
  handlers/{handler-name}.md
  records/{record-name}.md
  services/{service-name}.md
  utilities/{utility-name}.md
  types/{type-name}.md
```

Categories are `attributes`, `constants`, `extensions`, `handlers`, `records`, `services`, `types`, `utilities`, and package-specific ones such as `validation-rules`.

- `configuration` is not a category: a package's configuration is one page, never a directory.
- An `IExceptionHandler` implementation belongs under `handlers`, not `types`.
- `utilities` is for a static helper class that exposes only static members, is never instantiated, and is never injected, such as `ApplicationUtils`. `types` is for something a consumer holds an instance of, inherits from, or catches, and is the fallback for a public non-DI class, struct, record, or value that fits no more specific category.
- Never introduce `classes` or `interfaces` groups unless the user explicitly asks.
- Avoid duplicate pages for the same API.

Service pages document the DI contract and the registered implementation's behavior in one place. `IAppHostResolver` is documented at `services/app-host-resolver.md` under `# AppHostResolver`, with examples and type signatures still using `IAppHostResolver`. Methods stay on that page as `## MethodName` sections; split them into separate pages only when the page has grown too large to navigate.

Extension methods use one page per public extension method at `extensions/{extension-method-name}.md`, holding every overload. Do not document extension classes as pages unless a real method-name collision requires it and the user agrees. Which shape the one page takes depends on the call:

- **Same call shape, only the receiver changes.** One `## Usage` holding a `::: code-group` with a file per receiver, and one `## Type signature` listing every overload. `AddCustomLogging` is the model.
- **Genuinely different usage paths.** One `##` per overload family, named for the receiver or the family, each with its usage and `### Type signature`. `UseAspNetValidation` is the model, its `IApplicationBuilder`, `RouteHandlerBuilder`, and `RouteGroupBuilder` overloads being reached from different places.

A package exposing a very large set of closely related validation-rule APIs may use grouped family pages instead of one page per rule attribute or fluent rule method. Each public rule API is still documented exactly once, with usage, behavior notes, and type signatures. Use this only where separate pages would create repetitive, low-value navigation.

## Navigation

- Update the matching file in `docs/.vitepress/config/menu/`. Every package is reachable from the top navigation dropdown, and every API page from its package sidebar.
- Introduction, installation, `Configuration`, and package-specific guide pages such as Logging's `Formatter` stay in the first non-collapsible group, in that order. There is no collapsible `Configuration` category group.
- Group order after it: `Extensions`, package-specific groups such as `Validation Rules`, `Attributes`, `Handlers`, `Services`, `Utilities`, `Types`, `Records`, `Constants`.
- Category and API groups use `collapsed: false`, so they are collapsible and initially open.
- Use human-readable labels and slugified links. Attribute labels drop the `Attribute` suffix, so `AuthPermissionAttribute` appears as `AuthPermission`. DI service labels and routes drop the leading `I`, so `IAppHostResolver` appears as `AppHostResolver` at `services/app-host-resolver`.
- A service page is one sidebar item. Do not nest its methods as children unless the page has been intentionally split.

## Page Budgets

Hard limits. A page that exceeds one is wrong, not a judgement call.

| Limit | Value |
|---|---|
| Description, between the H1 and the first `##` | **1 to 3 sentences** |
| `::: tip` / `::: warning` / `::: danger` per page | **1** |
| Prose after the last code block in `## Usage` | **none** |
| Sections not on the page kind's list | **none** |
| Sections before `## Usage`, on a page that has one | **none** |

**Description.** One sentence on what the API does, and at most two more for a default, a constraint, or a failure the caller must handle. Never a fourth. `serilog/formatter.md` is the one exemption, for its colour syntax.

**Callouts.** One per page, for the single thing that costs the reader something if they get it wrong. When a page seems to need two, one of them is a fact for the frontmatter `params` description or prose for the page description.

**`## Usage` comes first** on every page that keeps one, directly after the description. A behavior section goes after it; everything else is a description, a callout, or a frontmatter entry.

**Drop `## Usage` when the member sections already show it.** This is the normal shape for a service page: `AuthUserService` opens on `## LoginAsync`. Keep it only when it shows something no member section does, as `types/auth-db-context.md` does by showing the context and user entity being derived. When removing it, fold the receiver's construction into the first member's example, leaving the later members as short, focused calls.

**`exceptions.md` has no `## Usage` either.** Catching guidance belongs in each exception's own `##` section.

**A record page has no `## Usage` at all**, covering `records/`, `requests/`, and `results/`. The page is its description, any single callout, `<FrontmatterDocs/>`, a `##` for each public method, then `## Type signature` last. A record that exposes public methods, as `UserAgent` and `ValidationRuleResult` do, gives each a `## MethodName` section with its own `### Type signature`, placed after `<FrontmatterDocs/>` and before the record's own type signature. A property, field, or constructor never gets a section this way.

**Never leave a duplicate behind under another name.** `## Example`, `## Getting started`, and `## Overview` are the same duplication under a forbidden name.

**No prose after the usage example.** Anything worth saying goes in the description, the frontmatter, or the one callout.

**Sections are a closed list per page kind:**

| Page | Sections, in this order |
|---|---|
| `index.md` | `Categories`, `Quick Example` |
| `configuration.md` | the JSON shape, then at most one cross-cutting section, then `<FrontmatterDocs/>`. The page ends there; it has no `Usage` |
| `installation.md` | `Dependencies`, then `Startup Registration` **or** `Usage`, then at most one install-specific section |
| `extensions/*.md` | `Usage`, `Type signature`, with at most one behavior section between them. Overloads with genuinely different usage paths use one `##` per family instead, each holding its usage and `### Type signature` |
| `records/*.md`, `requests/*.md`, `results/*.md` | one `##` per public method, then `Type signature` last. No `Usage`, and no `##` per field: `<FrontmatterDocs/>` renders them |
| `services/*.md`, `types/*.md`, `utilities/*.md`, `handlers/*.md`, `attributes/*.md`, `constants/*.md` | one `##` per public member, preceded by `Usage` only when it shows something no member section does |
| `validation-rules/*.md` | one `##` per rule |
| `exceptions.md` | the summary table, then one `##` per exception. No `Usage` |
| package guide pages | free, but every other rule still applies |

Never invent a section, and never give the one section a page is allowed a generic name. `What it registers`, `Behavior`, `Placement`, `Arguments`, `Overload families`, and `Why not the default configuration` have all appeared and are all wrong: either the content belongs in the description, a callout, or a frontmatter entry, or the section is the page's one behavior section and is named for what it covers, such as `Registered services`, `Pipeline order`, `Trusted networks`, `Shutdown behavior`. Never name a section after one of the page's own parameters.

## Documentation Voice

A page describes what the API is and does now.

- **No history.** No version history, migration notes, or comparisons with earlier behavior: no "previously", "now defaults to", "changed in", "before upgrading", or `::: danger Changed default`. A default that used to be different is documented as the default it is; if the old value is still a legitimate configuration, show how to pass it without saying it used to be the default. Version information belongs in `<since>` XML documentation and the release notes.
- **No rationale.** No `## Why this exists`, `## Why this matters`, or worked arguments about what would go wrong with a different design. State constraints, failure modes, and defaults as facts, in one or two sentences. "A unique index without a filter treats nulls as equal on some providers" is a fact a reader needs; three paragraphs building the case for the helper is not.
- **Same shape for every page of a kind.** Follow the page kind's schema exactly, in the order given, with no invented sections. Parameter behavior goes in the `params` or `fields` frontmatter description, never a `## ParameterName` section. Public records use `fields` frontmatter and `<FrontmatterDocs/>`, not a raw type signature block alone.
- **Callout placement follows one test: would the reader be worse off learning this only after copying the code?** A trap that makes the example dangerous or wrong to copy, such as a model that must never be bound on a public route, a secret that must never be logged, or a section the call requires, goes **above** the code block. Anything explaining a consequence of the code just shown goes **after** it. On a `configuration.md` the JSON shape is the code block the test applies to. Write a callout as a statement about current behavior.
- **Never remove content without listing it first.** A rewrite may reorganize, retitle, and reword freely, but may not silently drop a page, a section, a configuration key, a message-catalog entry, or a documented member. Compare the existing page list and documented API list against the replacement first, and report anything that would disappear to the user for a decision. This matters most for content that cannot be reconstructed from source, such as message catalogs and localization key lists.

## Writing Requirements

- Write specific descriptions covering what the API does, when to use it, relevant behavior, failure cases, defaults, and constraints.
- A list entry pairing a name with a gloss separates the two with `&mdash;`: the index `## Categories` list, the `## Dependencies` lists, and the guide page lists. That is the only place the entity is used, never in a sentence.
- No filler sections such as `Overview`, `Details`, or generic next steps. No `Importing` or namespace section. Do not start a page with package or category metadata.
- Use current C# terminology and syntax. Nullable types use `string?`, not `string | null`.
- Put a blank line after every heading.
- Wrap a literal keystroke, signal, or terminal token in backticks in prose: `Ctrl+C`, `SIGTERM`, `NO_COLOR`. Never `<kbd>` markup. A table cell may carry the bare token when every cell in that column does.
- A field or parameter description never says only "Required" or "Optional". Say when a value becomes required, as in "Must be set when `Hosts` is empty", or say nothing.
- Every fenced block declares a language. A `csharp` block holding a bare member declaration is not highlighted, so write the complete declaration including the access modifier: `public IReadOnlyList<Job> Jobs { get; }`.
- Call an assembly-scanning method with no argument. `RegisterConsoleCommands`, `RegisterRecurringJobs`, and `RegisterRemoteCommands` fall back to the calling assembly, so pass `typeof(Program).Assembly` only in an example whose point is that commands live in another project.
- Include practical, copy-paste-ready examples. Never placeholder comments such as `// Use XXX from application code after installing the package.`
- Include all required `using` statements, ordered from shortest line to longest.
- Chain extension-method registrations when the APIs return the same builder or service collection and chaining is applicable.
- Link references to documented APIs in prose: relative links within the same package, absolute docs-root links for other packages, and never a link from an API to its own page.
- Use `::: code-group` when an example needs multiple files, configuration plus code, or multiple valid setup forms, and give each file a meaningful name such as `[Program.cs]`, `[appsettings.json]`, or `[ExampleSettings.cs]`.
- Do not place unrelated classes in one code block. An interface and its implementation may share one only when that makes the specific example clearer.
- Examples must compile against themselves. Every member, enum member, navigation, and property used on a type shown in a code group must be declared on it: a group showing an `Order` class and then binding `order.Stage` is wrong even though each block reads correctly alone. Re-check the whole group after editing any block in it.
- Do not put blank lines between consecutive property or field declarations in an example type. Keep them between types, and between members that have bodies.
- Do not add a null-forgiving operator that is not needed; a `string?` passed where `object?` is expected converts cleanly. Equally, do not reshape a model to avoid a `!` that is genuinely needed: model the entity the way a consumer would really write it and suppress where the compiler requires it.
- Nullable flow analysis does not cross lambda boundaries, so a preceding `Where` never makes a later `Select` non-null and a filtering clause added only to try is wrong. Inside a single lambda a `!= null &&` guard does work. In an expression tree `is not null` does not compile, so use `!= null` in LINQ queries.
- Name an argument only when it skips an earlier optional parameter, or when a bare literal such as a `bool` would be unreadable at the call site. Write a call carrying named arguments one argument per line with the closing parenthesis on its own line, unless the whole call still fits comfortably on one.

## Package Pages

The H1 is the package name. The page must leave the reader knowing what the package does, what is in it, and what it costs them to adopt; a link list with a two-line intro is not sufficient. **The order is fixed: description, `## Categories`, `## Quick Example`,** with nothing between them and anything additional after.

- A description of two or three paragraphs: what the package does, the problem it is aimed at, and the shape of using it. Do not open with the package name.
- `## Categories`: category links followed by `&mdash;` and a concise description, in package sidebar order, which is `Configuration`, package-specific guide pages such as Logging's `Formatter`, `Extensions`, package-specific groups such as `Validation Rules`, `Attributes`, `Handlers`, `Services`, `Utilities`, `Types`, `Records`, `Constants`.
- `## Quick Example` that is realistic rather than minimal, using a code group when registration, application code, and configuration are all part of using the package.

**The introduction ends at the quick example.** No behavior section, operational notes, or surface map. Do not restate the sidebar as page content either.

Installation pages have a fixed order too: **description, `## Dependencies`, then `## Startup Registration` or `## Usage`, then anything else**, never between dependencies and it. Use `## Startup Registration` when the package must be registered at application startup, which is the usual case, and `## Usage` for the rare package that registers nothing, such as `ef-core-model-building` or `utils`, showing where its API is called from instead.

- Show only the `dotnet` CLI installation command, in one shell code block.
- Explain the target framework and runtime expectations, and name the project the package belongs in when it is not obvious.
- `## Dependencies` lists actual package, framework, and project dependencies with their current versions, split into `### Framework references`, `### Package references`, and `### Project references`, omitting groups that do not apply. Say what a dependency is there for, not just that it exists, and call out one that arrives transitively and that a consumer will notice.
- Read dependency information from the current `.csproj` files and central package management files. Do not reuse stale versions from existing docs.
- Do not document how this repository builds. Central package management, `Directory.Packages.props`, and solution layout mean nothing to a consumer installing from NuGet. Do not list what a consumer would obviously install anyway, such as a database provider for an Entity Framework Core package.
- **`## Startup Registration` is a short description, then any constraint as a callout, then the registration code.** The description says what each call gives the reader, in a sentence or two, and links to the method pages. A single ordering or pairing constraint, such as one middleware having to precede another, goes in a `::: warning` above the code block. The same shape applies to `## Usage` on a package that registers nothing.
- Do not pad that description with what the code already says: "Register the services, then add the middleware" above a block that plainly does both is filler. Do not expand it into a second example, a middleware-ordering discussion, or an account of everything the call registers; that belongs on the page for the method being called.
- Anything after that section must be specific to installing the package and owned by no other page, such as provider-specific behavior. Configuration shape belongs on the configuration page, runtime behavior on the page for the API that has it.
- Do not announce that a configuration section is required. `configuration.md` already says which section a package binds and what happens when one is absent.

## Configuration

A package that binds configuration from `appsettings.json` gets `docs/{package}/configuration.md`, the only configuration page it has. **There is no `configuration/` directory**: a configuration record is documented as a group on that page, never on its own.

- Show the complete JSON shape directly under the description, before anything else on the page.
- **One `fields` group per configuration record, named after the record.** A group is a `name`, a `description`, and its own nested `fields`. `Fields.vue` renders each group as `## GroupName`, the description, then `### Fields` and the group's entries, so the page needs no hand-written headings. A package binding a single record still uses one group. Document a nested record as its own group and include the property that holds it as a field on the parent group, so the JSON nesting and the field list agree.
- Include every configuration field with `name`, `description`, `type`, and `default` when a real default exists.
- Do not flatten a nested section into prefixed key names such as `Template:IgnoreText` or `Lockout:Enabled`, and do not carry one `##` section per key.
- **The page ends at `<FrontmatterDocs/>`.** There is no `## Usage` and nothing below the rendered fields. If some fact about reaching the values is genuinely non-obvious, such as a section that binds to no options type at all, it belongs in the group description.
- Say in the description that the section is optional when it is. Use the page's single callout for that only when there is nothing more important to warn about.
- Never point the reader at another page for fields, defaults, or the JSON shape.
- Link the package introduction to `./configuration`; do not add separate "Configuration" and "Configuration types" categories.
- **Only an `extensions/` page says how something is registered.** A service, type, handler, or record page never carries "registered by `AddX`".
- If a startup method accepts `builder.Configuration`, explain which section it requires and include the warning above.

```yaml
fields:
    - name: AuthCredentialsSettings
      description: The `AuthCredentials` section itself. Every value has a default, so the section may be absent.
      fields:
          - name: PasswordResetLifetime
            description: How long a password reset token stays usable after it is issued.
            type: TimeSpan
            default: 01:00:00

    - name: LockoutPolicy
      description: The nested `AuthCredentials:Lockout` section.
      fields:
          - name: Enabled
            description: Whether repeated login failures lock the account.
            type: bool
            default: 'false'
```

## Standalone API Page Schema

Use this order for method, extension, attribute, constructor-like record, and other API pages:

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

- Keep a blank line between the closing frontmatter delimiter and the H1.
- Omit `params` when the API has no parameters; omit `returns` only when the API truly returns `void`; always include `<FrontmatterDocs/>` when `params`, `returns`, or `fields` exists.
- Never write manual `## Parameters`, `## Returns`, or `## Fields` sections. `returns` stays a single descriptive string, not an object with name and type fields.
- An API whose consequences reach beyond the call site may add one behavior section between `## Usage` and `<FrontmatterDocs/>`, named for what it covers, such as `## Querying` on an API that changes how later queries behave. Use it when the reader has to know how the configured API behaves afterwards, not to restate a parameter. One per page.
- `default` carries actual C# default values. Quote `'null'`, `'true'`, `'false'`, `'[]'`, and `'0'` where YAML parsing requires it. Nullable does not automatically mean optional: document the actual method default.
- Write generic types literally in a quoted scalar: `type: 'IReadOnlyList<string>'`, `type: 'Expression<Func<TEntity, object?>>'`, keeping the commas in `Dictionary<TKey, TValue>`. Never HTML entities such as `&lt;` and `&gt;`; `type` is interpolated as text, so the entity is displayed to the reader literally.
- Descriptions render through the shared `renderInlineCode` utility, so inline backticks and markdown links work and nothing else does. Emphasis, lists, and raw HTML are escaped and shown literally. Link an API from a description the same way prose does.
- Quote any `description` containing a colon followed by a space. YAML reads `deliberately: locking` as a mapping and the build fails on the whole page.
- A record page uses `fields`, never `params`: the two render identically apart from the heading, so `params` silently labels its fields "Parameters". It carries no `## Usage`.

## Service, Type, And Utility Page Schema

`services/`, `types/`, and `utilities/` pages share one shape, covering DI contracts with their registered implementation behavior, public non-DI classes, structs, records, and static helper classes. They use no frontmatter-driven parameter or return tables: parameters, return behavior, defaults, failure cases, and constraints are explained in each method's prose.

````md
# AppHostResolver

Clear description of the service, what it resolves or controls, and where the package uses it.

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

- A service page uses `# ServiceName` without the leading `I`, even when the exported API is an interface, and the real interface type in examples and type signatures.
- Do not say how a service is registered, its lifetime included. That belongs on the `extensions/` page for the registration method.
- Put each method directly under `## MethodName`, with its signature under `### Type signature`. Never add a `## Methods` wrapper.
- Keep these pages concise but complete. Split only when one is too large to scan comfortably.
- Use a frontmatter-driven standalone page instead for a constructor-like record or simple value whose parameters, returns, or fields should be rendered once for the whole API.

## Type Signatures

- End signatures with `;`. Keep short signatures on one line and wrap long ones only to avoid horizontal scrolling.
- When a parameter or argument list is wrapped, the closing parenthesis goes on its own line, indented to match the line that opened it, never with the parameters. Anything that follows the list, such as a base list or generic constraints, stays on that closing line. This matches the C# source style enforced by `.editorconfig`.

```csharp
public AuthenticationBuilder AddJwtBearerAuthentication(
    IConfiguration configuration
);
```

```csharp
public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : DbContext(options)
```

```csharp
modelBuilder.ApplyOneToOne<Account, Profile>(
    account => account.Profile,
    profile => profile.AccountId
);
```

```csharp
public ModelBuilder ApplyAutoInclude<TEntity>(
    Expression<Func<TEntity, object?>> navigation
) where TEntity : class;
```

- Do not add type declarations to overview pages. On service pages, method signatures live in the relevant method's `### Type signature`.

## Validation Rule Documentation

`AlmightyShogun.AspNet.RequestValidation` uses grouped rule-family pages under `docs/asp-net-request-validation/validation-rules/`, because its validation attributes and fluent rule methods expose a large, repetitive public surface.

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

- Document every public validation attribute and every public fluent validation rule method exactly once, on the relevant family page, under one `## RuleName` section per rule.
- Document `CustomRule` on `docs/asp-net-request-validation/custom-rules.md` instead, because it needs implementation guidance for the DI-resolved rule type and the optional custom attribute wrapper.
- No shared rule tables and no shared `## Type signature` blocks on family pages.
- A no-argument attribute shows its real usage once, for example `[Required]`. An attribute with arguments shows the constructor-shaped form first, then a blank line, then a concrete real usage. Use `::: code-group` with `[Attribute.cs]` and `[FluentRule.cs]` when both APIs exist, and no label comments such as `// Real example` inside the block.
- If a rule is fluent-only or attribute-only, document the existing public API and raise the missing counterpart as a package parity question before release.
- `docs/asp-net-request-validation/fluent-validation.md` documents `Validator<TRequest>` and general fluent-rule behavior: one main request example near the top, no duplicate of the full rule catalog, and links to the family pages for concrete rule examples.

## Special Package Conventions

- **Logging.** Formatter documentation stays on its own page, with formatter colors in a table. Installation startup registration uses a code group for `[IServiceCollection.cs]` and `[IHostBuilder.cs]`.
- **Mail Resend.** The DI contract is `IResendMailService`. Examples commonly need separate template and caller files, so use code groups. The required `mail` template files are documented on the `AddResendEmail` page, which is what throws when they are missing, not on the installation page.
- **Remote Commands.** Command message and response records belong in separately named code-group blocks. `RemoteCommand<T>` exposes `HandleCommandAsync(T, ICommandResponse, CancellationToken)` and a protected `CommandName`; a command writes its reply through `ICommandResponse.WriteAsync`, never to a `NetworkStream`.
- **Console Commands.** Commands are class-based: a command class has `ConsoleCommandAttribute` and exactly one public `ExecuteAsync` method returning `Task`, defining a command with zero parameters or parameters. Examples reflect the current class-based runtime.

## Validation

After documentation changes:

1. Run `bun run docs:build` from the repository root, and `dotnet build packages.sln` when source or project files changed.
2. Confirm every sidebar link resolves and every API page is reachable.
3. Diff the page list against the one before the rewrite and confirm nothing was dropped.
4. Compare the documented API list against the current consumer-facing source APIs, and confirm no internal or private implementation API is documented.
5. Search the authored docs, excluding `docs/node_modules`, for violations of the rules above. Every rule in this file is a check the build cannot run; these four are stated nowhere else:
   - frontmatter links that do not resolve, including a `link:` on the home page's features, since VitePress dead-link checking only inspects markdown bodies and reports a build as clean with those broken;
   - old type names and namespaces;
   - broken or stale slugs;
   - `outline: deep` frontmatter.
