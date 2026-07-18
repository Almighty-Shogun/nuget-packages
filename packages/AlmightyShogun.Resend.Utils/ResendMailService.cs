using Resend;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.Resend.Utils;

/// <summary>
/// Renders mail templates and sends them through the configured Resend client.
/// </summary>
///
/// <param name="resend">The Resend client used to send email messages.</param>
/// <param name="emailTemplateLoader">The template loader used to read shared HTML template fragments.</param>
/// <param name="emailSettings">The bound email settings used for sender details and template values.</param>
/// <param name="logger">The logger used to report send failures.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
internal sealed class ResendMailService(IResend resend, IEmailTemplateLoader emailTemplateLoader, IOptions<EmailSettings> emailSettings, ILogger<ResendMailService> logger) : IResendMailService
{
    /// <summary>
    /// Stores the current email settings snapshot used when rendering and sending messages.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private readonly EmailSettings _settings = emailSettings.Value;

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
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
