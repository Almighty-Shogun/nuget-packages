---
fields:
    - name: Html
      description: The rendered HTML body, exactly as it would be sent.
      type: string
    - name: Text
      description: The rendered plain-text body, for clients that do not display HTML.
      type: string
---

# MailPreview

Both rendered bodies for a template, returned by [`PreviewAsync`](../services/resend-mail-service#previewasync) without sending anything.

Writing them to a file is left to the caller, so the package takes on no path validation or overwrite semantics.

## Usage

```csharp
using AlmightyShogun.Mail.Resend;

WelcomeMail mail = new WelcomeMail("Ada");
MailPreview preview = await mailService.PreviewAsync(mail);

await File.WriteAllTextAsync("welcome.html", preview.Html);
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MailPreview(string Html, string Text);
```
