---
fields:
    - name: Label
      description: Visible text, HTML encoded when rendered, so markup in it is shown rather than applied. Blank is rejected.
      type: string

    - name: Url
      description: Destination the button points to. It must be an absolute `http`, `https`, or `mailto` URL; anything else is rejected.
      type: string
---

# MailButton

Represents a call-to-action button rendered by [`BaseMailTemplate`](../types/base-mail-template). Derived mail templates return `MailButton` values from the protected `Buttons` member when an email should include one or more prominent links.

The label becomes the visible text and the URL the target. Both are encoded in the HTML rendering; the plain-text rendering writes them as given, which is safe because the constructor has already rejected a URL that is not an absolute `http`, `https`, or `mailto` one.

::: warning
The constructor throws an `ArgumentException` when the label or URL is blank, or when the URL is not an absolute `http`, `https`, or `mailto` URL. Validating at construction rather than at render time is deliberate: the HTML and plain-text renderers are separate paths, and encoding only the HTML one would leave a `javascript:` URL visible verbatim in the plain-text alternative.
:::

## Usage

```csharp
using AlmightyShogun.Mail.Resend;

public sealed class ConfirmEmailTemplate(string url) : BaseMailTemplate
{
    public override string Subject => "Confirm your email address";

    protected override string Title => "Confirm your email";

    protected override string Greeting => "Hello,";

    protected override IReadOnlyList<string> Paragraphs =>
    [
        "Use the button below to confirm your email address."
    ];

    protected override IReadOnlyList<MailButton> Buttons =>
    [
        new("Confirm email", url)
    ];
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MailButton
{
    public MailButton(string label, string url);

    public string Label { get; }
    public string Url { get; }
}
```
