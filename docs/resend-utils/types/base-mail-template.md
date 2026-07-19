# BaseMailTemplate

Base class for emails that should be sent through [`IResendMailService`](../services/resend-mail-service). Application code inherits from this class to describe the message subject, title, greeting, paragraphs, and buttons without dealing with the Resend API message object directly.

The mail service renders every template twice: once as HTML by filling the base HTML template files and once as plain text for clients that do not display HTML. Text values are HTML encoded during rendering, while button URLs are encoded for attribute output.

## Usage

```csharp
using AlmightyShogun.Resend.Utils;

public sealed class PasswordResetMailTemplate(string resetUrl) : BaseMailTemplate
{
    public override string Subject => "Reset your password";

    protected override string Title => "Password reset";

    protected override string Greeting => "Hello,";

    protected override IReadOnlyList<string> Paragraphs =>
    [
        "Use the button below to choose a new password.",
        "The link expires soon, so request a new email if it no longer works."
    ];

    protected override IReadOnlyList<MailButton> Buttons =>
    [
        new("Reset password", resetUrl)
    ];
}
```

## Members

- `Subject` &mdash; public email subject used for the Resend message.
- `Title` &mdash; protected title rendered into the HTML document and visible heading.
- `Greeting` &mdash; protected opening line rendered before the message paragraphs.
- `Paragraphs` &mdash; protected optional body paragraphs rendered in order.
- `Buttons` &mdash; protected optional call-to-action buttons rendered in order.
