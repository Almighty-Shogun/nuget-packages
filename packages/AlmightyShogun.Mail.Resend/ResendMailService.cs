using Resend;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Renders mail templates and sends them through the configured Resend client, turning a provider failure into a result
/// rather than letting it escape.
/// </summary>
///
/// <param name="resend">The typed client the send request goes through, already wrapped in the standard resilience handler.</param>
/// <param name="emailTemplateLoader">The loader the shared HTML fragments are read from, caching each file after its first read.</param>
/// <param name="emailSettings">
/// The bound settings, read once at construction, so a message never sees a sender changed part-way through its own send.
/// </param>
/// <param name="logger">
/// The logger the exception behind a failed send request is recorded on, because the returned result carries only its
/// message and nothing forces a caller to look at that. A send refused for having no recipient logs nothing.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
internal sealed class ResendMailService(
    IResend resend,
    IEmailTemplateLoader emailTemplateLoader,
    IOptions<EmailSettings> emailSettings,
    ILogger<ResendMailService> logger
) : IResendMailService
{
    /// <summary>
    /// The settings read once at construction. The dependency is <see cref="IOptions{TOptions}"/> rather than
    /// <see cref="IOptionsSnapshot{TOptions}"/>, so the value is computed once for the process and a configuration reload
    /// never reaches a later message, whatever the service's lifetime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private readonly EmailSettings _settings = emailSettings.Value;

    /// <inheritdoc />
    public Task<MailSendResult> SendAsync(
        string recipientEmail,
        BaseMailTemplate mail,
        CancellationToken cancellationToken = default
    ) => SendAsync(mail, new MailOptions { To = [recipientEmail] }, cancellationToken);

    /// <inheritdoc />
    public async Task<MailSendResult> SendAsync(BaseMailTemplate mail, MailOptions options, CancellationToken cancellationToken = default)
    {
        if (options.To.Count == 0)
            return MailSendResult.Failure("No recipient was supplied.");

        MailPreview preview = await PreviewAsync(mail, cancellationToken);

        EmailMessage message = new()
        {
            From = _settings.From,
            Subject = mail.Subject,
            HtmlBody = preview.Html,
            TextBody = preview.Text
        };

        foreach (string recipient in options.To)
            message.To.Add(recipient);

        foreach (string recipient in options.Cc)
            (message.Cc ??= []).Add(recipient);

        foreach (string recipient in options.Bcc)
            (message.Bcc ??= []).Add(recipient);

        foreach (string recipient in options.ReplyTo)
            (message.ReplyTo ??= []).Add(recipient);

        foreach (MailAttachment attachment in options.Attachments)
            (message.Attachments ??= []).Add(new EmailAttachment
            {
                Filename = attachment.FileName,
                Content = attachment.Content,
                ContentType = attachment.ContentType
            });
        
        string idempotencyKey = options.IdempotencyKey ?? Guid.CreateVersion7().ToString();

        try
        {
            ResendResponse<Guid> response = await resend.EmailSendAsync(idempotencyKey, message, cancellationToken);

            return MailSendResult.Success(response.Content.ToString());
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to send Resend email to {Recipient}", options.To[0]);

            return MailSendResult.Failure(exception.Message);
        }
    }

    /// <inheritdoc />
    public async Task<MailPreview> PreviewAsync(BaseMailTemplate mail, CancellationToken cancellationToken = default)
    {
        string templateHtml = await emailTemplateLoader.LoadAsync("BaseEmailTemplate.html", cancellationToken);
        string paragraphTemplateHtml = await emailTemplateLoader.LoadAsync("BaseEmailParagraph.html", cancellationToken);
        string buttonTemplateHtml = await emailTemplateLoader.LoadAsync("BaseEmailButton.html", cancellationToken);

        return new MailPreview
        {
            Html = mail.Render(templateHtml, paragraphTemplateHtml, buttonTemplateHtml, _settings),
            Text = mail.RenderText(_settings)
        };
    }
}
