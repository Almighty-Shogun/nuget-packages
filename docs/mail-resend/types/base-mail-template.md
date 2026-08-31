# BaseMailTemplate

Base class for emails that should be sent through [`IResendMailService`](../services/resend-mail-service). Application code inherits from this class to describe the message subject, title, greeting, paragraphs, and buttons without dealing with the Resend API message object directly.

Every template is rendered twice, as HTML from the base template files and as plain text. The HTML rendering encodes text values and encodes button URLs for attribute output. The plain-text rendering writes values as given, so anything placed in a template must be safe to show unencoded.

## Usage

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

## Members

- `Subject` &mdash; public email subject used for the Resend message.
- `Title` &mdash; protected title rendered into the HTML document and visible heading.
- `Greeting` &mdash; protected opening line rendered before the message paragraphs.
- `Paragraphs` &mdash; protected optional body paragraphs rendered in order.
- `Buttons` &mdash; protected optional call-to-action buttons rendered in order.
- `AdditionalValues` &mdash; protected optional extra placeholder values, written as `{{Key}}` in the template files.

## Additional values

Override `AdditionalValues` to add template fields without changing the package or introducing a template engine. Each pair replaces a `{{Key}}` placeholder in the base template files, and every value is HTML encoded on the way in.

```csharp
protected override IReadOnlyDictionary<string, string> AdditionalValues
    => new Dictionary<string, string>
    {
        ["SupportEmail"] = "support@example.com"
    };
```

Subclass values are applied after the built-in placeholders, so a key naming a built-in placeholder has no effect. They are applied one after another over the accumulating text, and HTML encoding leaves braces alone, so a value containing `{{OtherKey}}` is itself substituted when that key is applied later. Enumeration order therefore decides the result.

::: warning
Additional values are applied to the HTML body only. The plain-text body has no markup to escape and no placeholders to fill, so a template that relies on an additional value for its wording renders it in the HTML alternative alone.
:::
