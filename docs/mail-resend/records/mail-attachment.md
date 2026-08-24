---
fields:
    - name: FileName
      description: Name the recipient sees. Resend uses it to infer a content type when none is given.
      type: string
    - name: Content
      description: The file bytes. Resend base64-encodes them, so a large attachment costs memory and request size.
      type: 'byte[]'
    - name: ContentType
      description: MIME type. Leave it null to let Resend infer one from the file name.
      type: string?
      default: 'null'
---

# MailAttachment

One file attached to a send, supplied through [`MailOptions.Attachments`](./mail-options).

## Usage

```csharp
using AlmightyShogun.Mail.Resend;

MailSendResult result = await mailService.SendAsync(
    new InvoiceMail(invoice.Number),
    new MailOptions
    {
        To = [customer.Email],
        Attachments = [
            new MailAttachment(
                "invoice.pdf",
                pdfBytes,
                "application/pdf"
            )
        ]
    },
    cancellationToken
);
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MailAttachment(
    string FileName,
    byte[] Content,
    string? ContentType = null
);
```
