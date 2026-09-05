# BaseMailTemplate

Base class for the emails sent through [`IResendMailService`](../services/resend-mail-service), describing a message as values rather than as a Resend message object. A subclass supplies the subject, title, greeting, paragraphs, and buttons, and the base class renders both the HTML body and the plain-text alternative sent alongside it. Every value interpolated into the HTML is encoded, while the plain-text body is written as given, since it has no markup to escape.

## Subject

The subject of the sent message, and the only value that is not rendered into either body. It is public because the mail service reads it while building the message, and it is abstract, so every template supplies one. [`PreviewAsync`](../services/resend-mail-service#previewasync) returns the two rendered bodies alone, so a preview never shows it.

```csharp
using AlmightyShogun.Mail.Resend;

public sealed class PasswordResetMailTemplate(string url) : BaseMailTemplate
{
    public override string Subject => "Reset your password";
    protected override string Title => "Password reset";
    protected override string Greeting => "Hello,";

    protected override IReadOnlyList<string> Paragraphs =>
    [
        "Use the button below to choose a new password.",
        "The link expires soon, request a new email if it no longer works."
    ];

    protected override IReadOnlyList<MailButton> Buttons =>
    [
        new("Reset password", url)
    ];
}
```

### Type signature

```csharp
public abstract string Subject { get; }
```

## Title

The heading of the message, substituted into the `{{DocumentTitle}}` and `{{Title}}` placeholders of `BaseEmailTemplate.html`, so where it lands in the HTML is decided by that file. The plain-text body opens with it, followed by a blank line, unless it is blank or whitespace, in which case it is left out there entirely. It is abstract, so a template supplies one even when it has no heading to show.

### Type signature

```csharp
protected abstract string Title { get; }
```

## Greeting

The opening line addressing the recipient, substituted into the `{{Greeting}}` placeholder and written into the plain-text body after the title. It is rendered unconditionally, so a template with nothing to greet returns an empty string and leaves a blank line behind rather than being skipped the way a blank `Title` is.

### Type signature

```csharp
protected abstract string Greeting { get; }
```

## Paragraphs

The body paragraphs, in order. Each one is rendered through `BaseEmailParagraph.html`, with `{{Paragraph}}` replaced by the encoded text, and repeated in the plain-text body on its own line followed by a blank line. Empty by default, which renders a message carrying only a greeting and buttons.

### Type signature

```csharp
protected virtual IReadOnlyList<string> Paragraphs { get; }
```

## Buttons

The call-to-action buttons rendered after the paragraphs, each through `BaseEmailButton.html` with `{{ButtonUrl}}` and `{{ButtonLabel}}` replaced, and repeated in the plain-text body as label and URL pairs so the destination survives for a client that shows only text. Empty by default. A [`MailButton`](../records/mail-button) rejects a blank or unsafe URL when it is constructed, so a bad destination throws while the list is being built rather than while the message renders.

### Type signature

```csharp
protected virtual IReadOnlyList<MailButton> Buttons { get; }
```

## AdditionalValues

Extra placeholder values for the template, written as `{{Key}}` in the template files. Override it to add template fields without changing the package or introducing a template engine. Empty by default, and every value is HTML encoded on the way in.

Subclass values are applied after the built-in placeholders, so a key naming a built-in placeholder has no effect. They are applied one after another over the accumulating text, and HTML encoding leaves braces alone, so a value containing `{{OtherKey}}` is itself substituted when that key is applied later. Enumeration order therefore decides the result.

```csharp
using AlmightyShogun.Mail.Resend;

public sealed class SupportRequestMailTemplate : BaseMailTemplate
{
    public override string Subject => "We received your request";
    protected override string Title => "Request received";
    protected override string Greeting => "Hello,";

    protected override IReadOnlyDictionary<string, string> AdditionalValues
        => new Dictionary<string, string>
        {
            ["SupportEmail"] = "support@example.com"
        };
}
```

::: warning
Additional values are applied to the HTML body only. The plain-text body has no markup to escape and no placeholders to fill, so a template that relies on an additional value for its wording renders it in the HTML alternative alone.
:::

### Type signature

```csharp
protected virtual IReadOnlyDictionary<string, string> AdditionalValues { get; }
```
