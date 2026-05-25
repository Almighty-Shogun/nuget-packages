---
outline: deep

params:
    - name: title
      description: Text to assign to the console window title.
      type: string
---

# Title

Sets the current console window title through `Console.Title`. This is useful for console applications, background worker hosts running in a terminal, or local tooling where the window title should identify the current process.

The method only affects environments that expose a console title. In hosts without an interactive console, the runtime may ignore the title change or apply platform-specific behavior.

## Usage

```csharp
using AlmightyShogun.Utils;

ApplicationUtils.Title("Import Worker");
```

<FrontmatterDocs/>

## Type signature

```csharp
public static void Title(string title);
```
