---
params:
    - name: Label
      description: Text shown inside the rendered email button.
      type: string

    - name: Url
      description: Destination URL opened when the recipient clicks the button.
      type: string
---

# MailButton

Represents a call-to-action button rendered by `BaseMailTemplate`. Derived mail templates return `MailButton` values from the protected `Buttons` member when an email should include one or more prominent links.

The renderer uses the label as visible button text and the URL as the button target. Both values are encoded when rendered into the HTML template.

## Usage

```csharp
using AlmightyShogun.Resend.Utils;

public sealed class ConfirmEmailTemplate(string confirmationUrl) : BaseMailTemplate
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
        new("Confirm email", confirmationUrl)
    ];
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MailButton(
    string Label,
    string Url
);
```
