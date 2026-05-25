---
outline: deep

params:
    - name: question
      description: Question text written to the console prompt.
      type: string

    - name: defaultValue
      description: Value returned when the user submits an empty response.
      type: string?
      default: 'null'

returns: The entered answer, or the default value when one is provided and the input is empty.
---

# AskQuestionAsync

Writes a question prompt to the console and waits until a non-null answer is available. When `defaultValue` is provided, pressing enter without typing a value returns that default.

Use this helper inside command handlers that need a small interactive prompt. After reading input, the method clears the prompt line with `RemoveLastConsoleLine`.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

string environment = await ConsoleUtils.AskQuestionAsync(
    "Which environment should be deployed?",
    defaultValue: "development"
);
```

<FrontmatterDocs/>

## Type signature

```csharp
public static Task<string> AskQuestionAsync(
    string question,
    string? defaultValue = null
);
```

## Uses

- [RemoveLastConsoleLine](./remove-last-console-line)
