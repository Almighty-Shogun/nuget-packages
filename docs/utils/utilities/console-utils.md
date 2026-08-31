# ConsoleUtils

Console primitives for a command-line application: naming the window, prompting for input, erasing a line, and taking over what `Ctrl+C` means. All members are static and none is registered in the container. Only the cursor rewrite checks for redirected output and skips itself. The window title, the prompt write, the colour change, and the cancellation guard all run either way, so a redirected stream still receives the prompt text.

## Usage

```csharp
using AlmightyShogun.Utils;

ConsoleUtils.Title("Importer");
ConsoleUtils.PreventCancellation();

string? environment = await ConsoleUtils.AskQuestionAsync(
    "What is the environment?",
    "staging"
);
```

## Title

Sets the console window title. A thin wrapper over `Console.Title`, provided so startup code reads consistently alongside the other helpers.

Setting the title is not supported on every platform and terminal. Where it is unsupported the value is ignored rather than throwing.

```csharp
using AlmightyShogun.Utils;

ConsoleUtils.Title("Importer");
```

### Type signature

```csharp
public static void Title(string title);
```

## RemoveLastLine

Erases the line above the cursor and parks the cursor at its start, so a prompt or a progress line can be replaced instead of scrolling away.

Does nothing when output is redirected or the cursor is already at the top. A host that refuses cursor movement even when output is not reported as redirected fails silently too, because erasing a line is cosmetic and never worth taking down a process for.

```csharp
using AlmightyShogun.Utils;

Console.WriteLine("Connecting...");
ConsoleUtils.RemoveLastLine();
Console.WriteLine("Connected.");
```

### Type signature

```csharp
public static void RemoveLastLine();
```

## AskQuestionAsync

Prompts on the console and waits for an answer, repeating the prompt until one is available. The typed input is coloured, and the prompt line is erased once answered, so a sequence of questions does not fill the screen with what was already asked.

Passing no `defaultValue` makes the question mandatory: an empty line re-asks rather than returning, and only a typed answer or a closed input stream ends the loop. The result is never empty, and is null only for a mandatory question whose input stream ended.

```csharp
using AlmightyShogun.Utils;

string? environment = await ConsoleUtils.AskQuestionAsync(
    "What is the environment?",
    "staging"
);
string? name = await ConsoleUtils.AskQuestionAsync(
    "Service name",
    cancellationToken: cancellationToken
);
```

::: tip
A redirected or closed input stream ends the prompt rather than looping on it. With a `defaultValue` the default is returned, and without one the result is null, so give a default to anything that might run outside an interactive terminal and treat null as "nobody was there to answer".
:::

::: warning
`Console.In` reads synchronously whatever is asked of it, so awaiting this yields no thread back while a reader is typing. The asynchronous shape is for composing with asynchronous callers, not for scaling. The cancellation token is observed between reads, so it takes effect once the pending line is submitted rather than interrupting someone part way through typing one.
:::

### Type signature

```csharp
public static Task<string?> AskQuestionAsync(
    string question,
    string? defaultValue = null,
    CancellationToken cancellationToken = default
);
```

## PreventCancellation

Stops `Ctrl+C` from terminating the process, so a console application can handle shutdown itself.

Safe to call from any thread and any number of times; only the first call attaches a handler, so calling this from several entry points does not stack them. There is no matching method to restore the default behavior, so once cancellation is prevented it stays prevented until the process exits.

```csharp
using AlmightyShogun.Utils;

ConsoleUtils.PreventCancellation();
```

### Type signature

```csharp
public static void PreventCancellation();
```
