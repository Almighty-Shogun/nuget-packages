using Resend;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.Resend.Utils;

internal sealed class ResendMailService(IResend resend, IEmailTemplateLoader emailTemplateLoader, IOptions<EmailSettings> emailSettings, ILogger<ResendMailService> logger) : IResendMailService
{
    private readonly EmailSettings _settings = emailSettings.Value;

    /// <inheritdoc/>
    public async Task<bool> SendAsync(string recipientEmail, BaseMailTemplate mail)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiToken) || string.IsNullOrWhiteSpace(_settings.FromEmail))
            return false;
        
        string templateHtml = await emailTemplateLoader.LoadAsync("BaseEmailTemplate.html");
        string paragraphTemplateHtml = await emailTemplateLoader.LoadAsync("BaseEmailParagraph.html");
        string buttonTemplateHtml = await emailTemplateLoader.LoadAsync("BaseEmailButton.html");

        EmailMessage message = new()
        {
            From = $"{_settings.FromName} <{_settings.FromEmail}>",
            Subject = mail.Subject,
            HtmlBody = mail.Render(templateHtml, paragraphTemplateHtml, buttonTemplateHtml, _settings),
            TextBody = mail.RenderText(_settings)
        };

        message.To.Add(recipientEmail);

        try
        {
            await resend.EmailSendAsync(message);
            
            return true;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to send Resend email to {Recipient}", recipientEmail);
            
            return false;
        }
    }
}
