---
outline: deep
---

# PreventCancellation

Prevents Ctrl+C from ending the current console process by subscribing to `Console.CancelKeyPress` and setting `Cancel` to `true`. This is useful for interactive command loops or local development tools that should decide for themselves when to stop.

Call this once during startup before entering a long-running console loop. The method does not stop the event from being raised; it only marks the default cancellation behavior as handled.

## Usage

```csharp
using AlmightyShogun.Utils;

ApplicationUtils.PreventCancellation();

while (true)
{
    string? input = Console.ReadLine();

    if (input == "exit")
        break;
}
```

## Type signature

```csharp
public static void PreventCancellation();
```
