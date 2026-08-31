# Documentation Instructions

These instructions apply to all VitePress documentation work under `docs/`. Read the current package source and existing documentation before making changes. The source code is authoritative.

## Instruction File Parity

`docs/AGENTS.md` and `docs/CLAUDE.md` are the same document under two names. Their content must stay identical, 1:1, at all times.

- Any edit to one must be applied to the other in the same change. Never update one and leave the other behind.
- The same rule applies to the repository root pair, `AGENTS.md` and `CLAUDE.md`.
- Root and `docs/` are separate pairs. Mirror within a pair only. Do not copy documentation instructions into the repository pair or the reverse.
- When asked to change instructions in either file, treat the change as covering both files by default and confirm both were written.

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
- `AlmightyShogun.EntityFrameworkCore.ModelBuilding` becomes `ef-core-model-building`
- `AlmightyShogun.Mail.Resend` becomes `mail-resend`

Each package normally contains:

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

Use meaningful categories such as `attributes`, `constants`, `extensions`, `handlers`, `records`, `services`, `types`, `utilities`, and package-specific categories such as `validation-rules`. `configuration` is not a category: a package's configuration is one page, never a directory. Put an `IExceptionHandler` implementation under `handlers`, not `types`. Do not introduce separate `classes` and `interfaces` groups for new or migrated documentation unless the user explicitly asks for that structure.

Service pages document consumer-facing DI contracts and the behavior of the registered implementation in one place. The route and page title use the service name without the interface prefix, for example `IAppHostResolver` is documented at `services/app-host-resolver.md` with `# AppHostResolver`. Examples and type signatures still use `IAppHostResolver`.

Service methods usually stay on the same service page as `## MethodName` sections. Do not create separate method pages for small or moderate services. Only split service methods into separate pages when the page becomes too large and separate pages clearly improve navigation.

Extension methods use one page per public extension method at `docs/{package}/extensions/{extension-method-name}.md`. Keep overloads of the same extension method on that method page. Do not document extension classes as their own pages unless a real method-name collision requires it and the user agrees.

When overloads have different receiver types, registration targets, or usage paths, keep them on one page and split the page into clear `## OverloadFamily` sections. Use the `AddCustomLogging` page style for this: one `# MethodName` page, one `##` section per overload family, usage under each section, and `### Type signature` under each section. Do not create separate pages for overloads of the same extension method.

Public non-DI classes, structs, records, and values can use focused categories such as `records`, `constants`, or `types`. Use `types` when a package exposes public types that do not fit a more specific category.

Use `utilities` for a static helper class that exposes only static members, is never instantiated, and is never injected, such as `ApplicationUtils`. `types` is for something a consumer holds an instance of, inherits from, or catches. A static helper filed under `types` is wrong: it is not a type the reader ever has one of.

Avoid duplicate pages for the same API. Overloads of the same method belong on one method page or one method section.

Packages that expose a very large set of closely related validation-rule APIs may use grouped family pages instead of one page per rule attribute or fluent rule method. Each public rule API must still be documented exactly once on the relevant family page, with clear usage, behavior notes, and type signatures. Use this only when separate pages would create repetitive, low-value navigation.

## Validation Rule Documentation

`AlmightyShogun.AspNet.RequestValidation` uses grouped rule-family pages because validation attributes and fluent rule methods expose a large, repetitive public surface.

Validation rule family pages under `docs/asp-net-request-validation/validation-rules/` use:

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
- Document `CustomRule` on `docs/asp-net-request-validation/custom-rules.md`, not inside a rule family, because it needs implementation guidance for the DI-resolved rule type and optional custom attribute wrapper.
- Use one `## RuleName` section per validation rule.
- Do not use shared rule tables or shared `## Type signature` blocks on validation rule family pages.
- For no-argument attributes, show only the real attribute usage once, for example `[Required]`.
- For attributes with arguments, show the constructor-shaped attribute first, then a blank line, then a concrete real usage.
- Use `::: code-group` with `[Attribute.cs]` and `[FluentRule.cs]` when both APIs exist.
- If a rule is fluent-only or attribute-only, document the existing public API and call out the missing counterpart as a package parity question before release.
- Do not add label comments such as `// Real example` inside the code block.

The `docs/asp-net-request-validation/fluent-validation.md` page documents `ValidatableRequest<TRequest>` and general fluent-rule behavior:

- Keep one main request example near the top.
- Do not duplicate the full rule catalog on this page.
- Link to the validation rule family pages for concrete rule examples.

## Navigation

- Update the matching file in `docs/.vitepress/config/menu/`.
- Every package must be available from the top navigation package dropdown.
- Every API page must be reachable from its package sidebar.
- Introduction, installation, and package-level configuration links stay in the first non-collapsible group.
- Sidebar groups use this order when present: package pages, `Extensions`, package-specific groups such as `Validation Rules`, `Attributes`, `Handlers`, `Services`, `Utilities`, `Types`, `Records`, `Constants`.
- `Configuration` is a package page in the first group. There is no collapsible `Configuration` category group, because no package has more than one configuration page.
- Package-specific guide pages, such as Logging's `Formatter`, stay in the first group after `Configuration`.
- Category and API groups use `collapsed: false` so they are collapsible and initially open.
- Use human-readable labels and slugified links.
- Attribute sidebar labels omit the `Attribute` suffix. For example, `AuthPermissionAttribute` appears as `AuthPermission` in the sidebar.
- DI service sidebar labels and routes omit the leading interface `I`. For example, `IAppHostResolver` appears as `AppHostResolver` and uses `services/app-host-resolver`.
- Service pages are usually a single sidebar item. Do not list every method as nested sidebar children unless the service page has been intentionally split.

## Page Budgets

Hard limits. A page that exceeds one is wrong, not a judgement call.

| Limit | Value |
|---|---|
| Description, between the H1 and the first `##` | **1 to 3 sentences** |
| `::: tip` / `::: warning` / `::: danger` per page | **1** |
| Prose after the last code block in `## Usage` | **none** |
| Sections not on the page kind's list | **none** |
| Sections before `## Usage`, on a page that has one | **none** |

**Description.** One sentence saying what the API does, and at most two more for a default, a constraint, or a failure the caller must handle. Never a fourth. If three sentences cannot carry it, the surplus is rationale and belongs nowhere. `serilog/formatter.md` is the one exemption, because its colour syntax has no analogue elsewhere.

**Callouts.** One per page, for the single thing that costs the reader something if they get it wrong. Two callouts means neither stands out. When a page seems to need two, one of them is either a fact for the frontmatter `params` description or prose for the description.

**`## Usage` comes first.** On every page that keeps one, `## Usage` is the first section, directly after the description. A reader arrives wanting the call, not a preamble about overloads or arguments. Nothing earns a place above it: a behavior section goes after it, and everything else is a description, a callout, or a frontmatter entry.

**`## Usage` is dropped when the member sections already show it.** On a page whose `##` sections are its members and where each carries its own example, a usage block is a second rendering of what the page is about to show anyway. Delete it and let the first member section open the page. This is the normal shape for a service page: `AuthUserService` opens on `## LoginAsync`, not on a controller that calls `LoginAsync` and is then shown again underneath.

Keep `## Usage` only when it shows something no member section does. `types/auth-db-context.md` keeps one because it shows the context and user entity being derived, which none of its `DbSet` sections cover. A page whose only example is its usage block, such as a request, result, or entity page, has no member sections to duplicate and always keeps it.

**Removing it must not lose the receiver.** On a service page the usage block is often the only place the service appears being injected, while the member examples call an already-resolved variable. Fold that construction into the first member's example rather than dropping it, so the reader still sees where the receiver comes from; the later members stay short, focused calls.

**Never leave a duplicate behind under another name.** Retitling the usage block `## Example`, `## Getting started`, or `## Overview` to keep it is the same duplication with a section name that is also forbidden.

**No prose after the usage example.** The example is the last thing in `## Usage`. A trailing sentence explaining what the example just showed is the most common form of the clutter these rules exist to stop. Anything worth saying goes in the description, in the frontmatter, or in the one callout.

**Sections are a closed list per page kind:**

| Page | Sections, in this order |
|---|---|
| `index.md` | `Categories`, `Quick Example` |
| `configuration.md` | the JSON shape, then at most one cross-cutting section, then `<FrontmatterDocs/>`, then `Usage` |
| `installation.md` | `Dependencies`, then `Startup Registration` **or** `Usage`, then at most one install-specific section |
| `extensions/*.md` | `Usage`, `Type signature`. An extension page may add one behavior section between them. |
| `services/*.md`, `types/*.md`, `utilities/*.md`, `handlers/*.md`, `records/*.md`, `attributes/*.md`, `constants/*.md` | one `##` per public member, preceded by `Usage` only when it shows something no member section does |
| `validation-rules/*.md` | one `##` per rule |
| package guide pages | free, but every other rule still applies |

Never invent a section, and never use a generic name for the one section a page is allowed. `What it registers`, `Behavior`, `Placement`, `Arguments`, `Overload families`, and `Why not the default configuration` have all appeared and are all wrong. Either the content belongs in the description, a callout, or a frontmatter entry, or the section is the page's one behavior section and must be named for what it covers: `Registered services`, `Pipeline order`, `Trusted networks`, `Shutdown behavior`.

Never name a section after one of the page's own parameters. The frontmatter renders them already.

## Documentation Voice

A page describes what the API is and does now. These four rules override any habit to the contrary and apply to every page in every package.

### Describe the current API, never its history

- No version history, migration notes, or comparisons with earlier behavior. No "previously", "now defaults to", "changed in", "before upgrading", or `::: danger Changed default` callouts.
- A default that used to be different is documented as the default it is. If the old value is still a legitimate configuration, show how to pass it, without saying it used to be the default.
- Version information belongs in `<since>` XML documentation and in the release notes, not on a documentation page.

### Rationale belongs in the audit files, not the page

- No `## Why this exists`, `## Why this matters`, or worked arguments about what would go wrong with a different design.
- State constraints, failure modes, and defaults as facts, in one or two sentences. "A unique index without a filter treats nulls as equal on some providers" is a fact a reader needs. Three paragraphs building the case for the helper is not.
- The reader wants to use the API correctly. They are not being persuaded that it should exist.

### Every page of a kind has the same shape

- Follow the schema for the page kind exactly, in the order given, with no invented sections.
- Do not add a `## ParameterName` section for a parameter. Parameter behavior goes in the `params` or `fields` frontmatter description, which is where every other page puts it.
- Content that must stand out uses `::: tip`, `::: warning`, or `::: danger` after the usage example, written as a statement about current behavior.
- Public records use `fields` frontmatter and `<FrontmatterDocs/>`. Do not document a record's members only inside a raw type signature block.

### Never remove content without listing it first

- A rewrite may reorganize, retitle, or reword freely. It may not silently drop a page, a section, a configuration key, a message-catalog entry, or a documented member.
- Before a rewrite, compare the existing page list and the documented API list against the replacement. Anything that would disappear is reported to the user for a decision first.
- This applies most to content that cannot be reconstructed from source, such as message catalogs and localization key lists.

## Writing Requirements

- Write specific descriptions that explain what the API does, when to use it, relevant behavior, failure cases, defaults, and constraints.
- Do not add filler sections such as `Overview`, `Details`, or generic next steps.
- Do not add an `Importing` or namespace section.
- Do not start pages with package/category metadata.
- Use current C# terminology and syntax. Nullable types use `string?`, not `string | null`.
- Put a blank line after every heading. A description that starts on the line directly below its `#` is a formatting bug, not a compact style.
- Wrap a literal keystroke, signal, or terminal token in backticks in prose: `` `Ctrl+C` ``, `` `SIGTERM` ``, `` `NO_COLOR` ``. Do not use `<kbd>` markup. A table cell may carry the bare token when every cell in that column does.
- A field or parameter description never says only "Required" or "Optional". The rendered type and the presence or absence of a default already carry that. Say when a value becomes required, as in "Must be set when `Hosts` is empty", or say nothing.
- Every fenced block declares a language. A `csharp` block holding a bare member declaration is not highlighted, so write the complete declaration including the access modifier: `public IReadOnlyList<Job> Jobs { get; }`, not `IReadOnlyList<Job> Jobs { get; }`.
- Call an assembly-scanning method with no argument in an example. `RegisterConsoleCommands`, `RegisterRecurringJobs`, and `RegisterRemoteCommands` fall back to the calling assembly, so `typeof(Program).Assembly` is noise that reads as if it were required. Pass an assembly only in an example whose point is that commands live in another project.
- Include practical, copy-paste-ready examples. Never use placeholder comments such as `// Use XXX from application code after installing the package.`
- Include all required `using` statements in examples and order them from shortest line to longest line.
- Chain extension-method registrations when the APIs return the same builder or service collection and chaining is applicable.
- Link references to documented APIs when they appear in prose. Use relative links within the same package, absolute docs-root links for other packages, and do not link an API to its own page.
- Use `::: code-group` when an example needs multiple files, configuration plus code, or multiple valid setup forms.
- Give code-group files meaningful names such as `[Program.cs]`, `[appsettings.json]`, or `[ExampleSettings.cs]`.
- Do not place unrelated classes in one code block. An interface and its implementation may share a block only when that makes the specific example clearer.
- Examples must compile against themselves. When a code group shows a type, every member used on that type anywhere in the same group must exist on it, and every enum member, navigation, and property referenced must be declared. A group that shows an `Order` class and then binds `order.Stage` is wrong even though each block reads correctly on its own.
- Check the whole group after editing any block in it. Changing one example to demonstrate a parameter is what usually leaves the shared entity block behind.
- Do not put blank lines between consecutive property or field declarations in an example type. They add nothing and push the rest of the example off the screen. Keep blank lines between types, and between members that have bodies.
- Do not add a null-forgiving operator that is not needed. Check whether the expression actually warns first: a `string?` passed where `object?` is expected converts cleanly, so a `!` there is noise.
- Do not reshape an example's model to avoid a `!` that is genuinely needed. Model the entity the way a consumer would really write it, then suppress where the compiler requires it. Turning an optional value into a required one to keep an example tidy teaches the wrong model, which is worse than the operator.
- Nullable flow analysis does not cross lambda boundaries. A preceding `Where` never makes a later `Select` non-null, so do not add a filtering clause that exists only to try. Inside a single lambda a `!= null &&` guard does work. In an expression tree, `is not null` does not compile at all, so use `!= null` in LINQ queries.
- Do not name an argument that does not need naming. Name one only when it skips an earlier optional parameter, or when a bare literal would be unreadable at the call site, such as a `bool`.
- Write a call that carries named arguments across multiple lines, one argument per line with the closing parenthesis on its own line, unless the whole call still fits comfortably on one.

## Package Pages

Package introductions use the package name as the H1. The page is the reader's first contact with the package and must leave them knowing what it does, what is in it, and what it costs them to adopt. A link list with a two-line intro is not sufficient.

**The order is fixed: description, then `## Categories`, then `## Quick Example`.** Nothing goes between them. Any additional section comes after the quick example.

- An opening description of two or three paragraphs: what the package does, the problem it is aimed at, and the shape of using it. Do not repeat the package name as the first words.
- A `## Categories` list. Category links followed by `&mdash;` and a concise description, in the same order as the package sidebar: `Configuration`, `Extensions`, package-specific groups such as `Validation Rules`, `Attributes`, `Handlers`, `Services`, `Utilities`, `Types`, `Records`, `Constants`. Package-specific guide pages, such as Logging's `Formatter`, appear after `Configuration`.
- A `## Quick Example` that is realistic rather than minimal. Use a code group when registration, application code, and configuration are all part of using the package.

**The introduction ends at the quick example.** Do not add a behavior section, operational notes, a surface map, or any other section. Everything a reader needs beyond the example already has a page that owns it, and repeating it here creates a second copy to keep in sync while burying the three things the page is for.

Do not restate the sidebar as page content either. A table or list that repeats the category's pages with a one-line gloss each tells the reader what the navigation already shows.

Installation pages follow a fixed order too: **description, then `## Dependencies`, then `## Startup Registration` or `## Usage`, then anything else.** A section such as provider support, configuration, or state-file behavior comes after the registration or usage section, never between dependencies and it.

Use `## Startup Registration` when the package must be registered at application startup, which is the usual case. Use `## Usage` instead for the rare package that registers nothing, such as `ef-core-model-building` or `utils`, and show where its API is called from instead.

- Show only the `dotnet` CLI installation command in one shell code block.
- Explain the target framework and runtime expectations, and name the project the package belongs in when it is not obvious.
- Add `## Dependencies` and list actual package, framework, and project dependencies with their current versions.
- Split dependencies into `### Framework references`, `### Package references`, and `### Project references`; omit groups that do not apply.
- Say what a dependency is there for, not just that it exists. Call out a dependency that arrives transitively and that a consumer will notice.
- Do not document how this repository builds. Central package management, `Directory.Packages.props`, and solution layout are internal to the monorepo and mean nothing to a consumer installing from NuGet.
- Do not list what a consumer would obviously install anyway, such as a database provider for an Entity Framework Core package.
- Read dependency information from the current `.csproj` files and central package management files when present. Do not reuse stale dependency versions from existing docs.
- **`## Startup Registration` is a short description, then any constraint as a callout, then the registration code.** The description says what each call gives the reader, in a sentence or two, and links to the method pages. A single ordering or pairing constraint, such as one middleware having to precede another, goes in a `::: warning` **above** the code block so it is read before the code is copied, not after. The same shape applies to `## Usage` on a package that registers nothing.
- Do not pad that description with what the code already says. "Register the services, then add the middleware" above a block that plainly does both is filler; what each call actually gives the reader is not.
- Do not expand it into a second example, a middleware-ordering discussion, or an explanation of everything the call registers. That belongs on the page for the method being called, where a reader looking it up will find it.
- Anything after that section must be specific to installing the package and owned by no other page, such as provider-specific behavior. Configuration shape belongs on the configuration page, and runtime behavior belongs on the page for the API that has it.
- When configuration is required, use this warning style with the actual section name:

```md
::: warning
Requires an `Example` section in application configuration, usually from `appsettings.json`.
:::
```

The Logging package is the only package that does not use this required-configuration warning because its configuration is optional.

## Configuration

When a package binds configuration from `appsettings.json`:

- Add `docs/{package}/configuration.md`. It is the only configuration page a package has.
- **There is no `configuration/` directory.** A configuration record is documented as a group on `configuration.md`, never on its own page. A reader who wants to know what a key does, what it defaults to, and how to read it back must never have to open a second page to find out.
- Show the complete JSON shape directly under the description, before anything else on the page. It is what a reader came for, and everything below it explains the keys they can now see.
- **One `fields` group per configuration record, named after the record.** A group is a `name`, a `description`, and its own nested `fields`. `Fields.vue` renders each group as `## GroupName`, the description, then `### Fields` and the group's entries, so the page needs no hand-written headings. A package binding a single record still uses one group, so the reader sees which type owns the keys.
- Close the page with a `## Usage` section below `<FrontmatterDocs/>`, showing how to read the bound settings through `IOptions<T>`. This is the only place `configuration.md` puts prose after the rendered fields, and it is where the old per-record pages' examples belong.
- Link the package introduction to `./configuration`; do not add separate “Configuration” and “Configuration types” categories.
- **Only an `extensions/` page says how something is registered.** A service, type, handler, or record page never carries "registered by `AddX`". The reader is on that page to use the type, and the sidebar already links the registration method.
- Include every configuration field with `name`, `description`, `type`, and `default` when a real default exists.
- Document a nested record as its own group, and include the property that holds it as a field on the parent group, so the JSON nesting and the field list agree.
- On `configuration.md`, say in the description that the section is optional when it is. Use the page's single callout for that only when there is nothing more important to warn about; a real trap always outranks it.
- Do not flatten a nested section into prefixed key names such as `Template:IgnoreText` or `Lockout:Enabled`. Use a group per type instead: the reader sees which record owns which key, and the JSON block above already shows the nesting.
- `configuration.md` carries no `##` section per key. A per-key section duplicates what the frontmatter already renders, which is the same mistake as a manual `## Parameters`.

Example:

```yaml
fields:
    - name: LocalhostApp
      description: Application audience used for `localhost` requests during development.
      type: string?
      default: 'null'
```

Grouped example, one group per bound record:

```yaml
fields:
    - name: CredentialAuthSettings
      description: The `CredentialAuth` section itself. Every value has a default, so the section may be absent.
      fields:
          - name: PasswordResetLifetime
            description: How long a password reset token stays usable after it is issued.
            type: TimeSpan
            default: 01:00:00

    - name: LockoutPolicy
      description: The nested `CredentialAuth:Lockout` section.
      fields:
          - name: Enabled
            description: Whether repeated login failures lock the account.
            type: bool
            default: 'false'
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
- An API whose consequences reach beyond the call site may add one behavior section between `## Usage` and `<FrontmatterDocs/>`, named for what it covers, such as `## Querying` on an API that changes how later queries behave. Use it when the reader has to know how the configured API behaves afterwards, not to restate a parameter. One such section per page.
- Omit `params` when the API has no parameters.
- Omit `returns` only when the API truly returns `void`.
- Always include `<FrontmatterDocs/>` when `params`, `returns`, or `fields` exists.
- Do not write manual `## Parameters`, `## Returns`, or `## Fields` sections.
- Returns remain a single descriptive string, not an object with name/type fields.
- Use `default` for actual C# default values. Use quoted `'null'`, `'true'`, `'false'`, `'[]'`, or `'0'` where YAML parsing requires it.
- Nullable does not automatically mean optional. Document the actual method default to show optional parameters.
- Write generic types literally and quote the YAML scalar: `type: 'IReadOnlyList<string>'`, `type: 'Expression<Func<TEntity, object?>>'`. Never use HTML entities such as `&lt;` and `&gt;`. The renderer interpolates `type` as text, so an entity is displayed to the reader as the literal characters `&lt;`.
- Keep generic type commas, for example `Dictionary<TKey, TValue>`.
- Inline backticks and markdown links in descriptions are rendered through the shared `renderInlineCode` utility. Link an API from a description the same way prose does: a relative link within the package, an absolute docs-root link across packages. Nothing else is markdown there. Emphasis, lists, and raw HTML are escaped and shown to the reader literally.
- Quote any `description` containing a colon followed by a space. YAML reads `deliberately: locking` as a mapping and the build fails on the whole page.
- A record page uses `fields`, never `params`. The two render identically apart from the heading, so `params` on a record silently labels its fields "Parameters".

## Service Page Schema

Use service pages for DI contracts and their registered implementation behavior. Service pages do not use frontmatter-driven parameter/return tables for each method. Explain parameters, return behavior, defaults, failure cases, and constraints in the method prose.

Use this order:

````md
# AppHostResolver

Clear description of the service, what it resolves or controls, and where the package uses it.

Application code should depend on `IAppHostResolver`. Explain what configuration or package behavior it relies on.

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
- Do not say how the service is registered, its lifetime included. That belongs on the `extensions/` page for the registration method, which the sidebar already links.
- Put each method directly under `## MethodName`; do not add an extra `## Methods` wrapper.
- Put each method signature under `### Type signature`.
- Keep service pages concise but complete. Split only when a service is too large to scan comfortably.

## Type Page Schema

This schema covers both `types/` and `utilities/` pages. Use it for public non-DI classes, structs, records, and static helper classes that are best understood as one small surface. These pages do not need frontmatter-driven parameter or return tables for each method when method sections are clearer.

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
- Wrap long signatures only when needed to avoid horizontal scrolling.
- When a parameter or argument list is wrapped, the closing parenthesis goes on its own line, indented to match the line that opened it, never indented with the parameters. Anything that follows the list, such as a base list or generic constraints, stays on that same closing line:

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

This matches the C# source style enforced by `.editorconfig`, so an example copied out of a package reads the same as the package.

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

### Mail Resend

- The DI contract is `IResendMailService`.
- Mail examples commonly need separate template and caller files; use code groups.
- Document the required `mail` template files on the `AddResendEmail` page, which is what throws when they are missing. The installation page does not carry them.

### Remote Commands

- Command message and response records belong in separately named code-group blocks.
- `RemoteCommand<T>` exposes `HandleCommandAsync(T, ICommandResponse, CancellationToken)` and a protected `CommandName`. A command writes its reply through `ICommandResponse.WriteAsync`, never to a `NetworkStream`.

### Console Commands

- Commands are class-based. A command class has `ConsoleCommandAttribute` and exactly one public `ExecuteAsync` method returning `Task`.
- One command class can define a command with zero parameters or parameters; examples should reflect the current class-based runtime.

## Validation

After documentation changes:

1. Run `bun run docs:build` from the repository root.
2. Run `dotnet build packages.sln` when source or project files changed.
3. Search authored docs, excluding `docs/node_modules`, for:
   - version history, migration notes, or changed-default callouts;
   - `## Why` sections or rationale essays;
   - `## ParameterName` sections duplicating frontmatter;
   - old type names and namespaces;
   - broken or stale slugs;
   - frontmatter links that do not resolve, including a `link:` on the home page's features, since VitePress dead-link checking only inspects markdown bodies and reports a build as clean with those broken;
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
   - placeholder or unusable examples;
   - members used in a code group that the types shown in that same group do not declare;
   - descriptions longer than three sentences;
   - pages with more than one callout;
   - prose after the last code block in `## Usage`;
   - a `## Usage` on a page whose member sections already show the same calls, or one retitled to `## Example` or `## Getting started` to survive that rule;
   - a member example calling a service through a variable the page never shows being injected;
   - sections not on the page kind's list in Page Budgets;
   - `registered by AddX` on a page outside `extensions/`;
   - an `IExceptionHandler` documented under `types/` instead of `handlers/`;
   - a static helper class documented under `types/` instead of `utilities/`;
   - introduction or installation pages carrying behavior, configuration, or operational detail that a dedicated page already owns;
   - a `configuration/` directory, or any configuration record documented on its own page instead of as a group on `configuration.md`;
   - a `configuration.md` whose fields are not grouped per record, or that has no `## Usage` section below `<FrontmatterDocs/>`;
   - `## Startup Registration` or `## Usage` on an installation page whose description restates the code, or whose ordering constraint sits after the code block instead of in a callout above it;
   - cross-reference lines pointing at another page for fields, defaults, or the JSON shape;
   - a section sitting above `## Usage`;
   - a heading with no blank line after it;
   - `<kbd>` markup, or a bare `Ctrl+C` or `SIGTERM` in prose;
   - a field or parameter description whose only content is "Required" or "Optional";
   - a `csharp` block whose only line is a member declaration with no access modifier;
   - `typeof(Program).Assembly` passed to a method that defaults to the calling assembly;
   - a record page using `params` instead of `fields`;
   - prefixed configuration key names such as `Lockout:Enabled` where a `fields` group belongs;
   - a `configuration.md` carrying one `##` section per key.
4. Confirm sidebar links resolve and every API page is reachable.
5. Diff the page list against the one before the rewrite and confirm nothing was dropped.
6. Compare the documented API list against current consumer-facing source APIs.
7. Confirm internal/private implementation APIs are not documented.

Do not silently change documentation conventions. If current files conflict with these instructions or a broad convention change appears necessary, explain it to the user before applying it.
