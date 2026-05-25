---
outline: deep
---

# RemoveLastConsoleLine

Clears the previous line in the console and moves the cursor back to the start of that cleared line. The command runtime uses this to remove entered command text or prompt text after it has been processed.

Use this helper when a command wants to keep the console output tidy after reading temporary input. It assumes the console cursor can be moved, so it is best suited for interactive terminals rather than redirected output.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

Console.WriteLine("Temporary status");
ConsoleUtils.RemoveLastConsoleLine();
```

<FrontmatterDocs/>

## Type signature

```csharp
public static void RemoveLastConsoleLine();
```
