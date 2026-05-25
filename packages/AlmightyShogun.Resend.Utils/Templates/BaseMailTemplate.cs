using System.Net;
using System.Text;

namespace AlmightyShogun.Resend.Utils;

public abstract class BaseMailTemplate
{
    public abstract string Subject { get; }

    protected abstract string Title { get; }

    protected abstract string Greeting { get; }

    protected virtual IReadOnlyList<string> Paragraphs => [];

    protected virtual IReadOnlyList<MailButton> Buttons => [];

    /// <summary>
    /// Renders the email template as HTML using the provided base templates.
    /// </summary>
    ///
    /// <param name="templateHtml">The base HTML template for the full email document.</param>
    /// <param name="paragraphTemplateHtml">The HTML template used for each paragraph in the body.</param>
    /// <param name="buttonTemplateHtml">The HTML template used for each button in the email.</param>
    /// <param name="settings">The configured email settings used to fill shared template values.</param>
    ///
    /// <returns>The rendered HTML email body.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    internal string Render(string templateHtml, string paragraphTemplateHtml, string buttonTemplateHtml, EmailSettings settings)
    {
        string bodyHtml = string.Join(string.Empty, Paragraphs.Select(paragraph =>
            paragraphTemplateHtml.Replace("{{Paragraph}}", Encode(paragraph), StringComparison.Ordinal)));

        string buttonsHtml = string.Join(string.Empty, Buttons.Select(button =>
            buttonTemplateHtml
                .Replace("{{ButtonUrl}}", EncodeAttribute(button.Url), StringComparison.Ordinal)
                .Replace("{{ButtonLabel}}", Encode(button.Label), StringComparison.Ordinal)));

        return templateHtml
            .Replace("{{DocumentTitle}}", Encode(Title), StringComparison.Ordinal)
            .Replace("{{LogoUrl}}", EncodeAttribute(settings.LogoUrl), StringComparison.Ordinal)
            .Replace("{{BrandName}}", Encode(settings.BrandName), StringComparison.Ordinal)
            .Replace("{{Title}}", Encode(Title), StringComparison.Ordinal)
            .Replace("{{Greeting}}", Encode(Greeting), StringComparison.Ordinal)
            .Replace("{{BodyHtml}}", bodyHtml, StringComparison.Ordinal)
            .Replace("{{ButtonsHtml}}", buttonsHtml, StringComparison.Ordinal)
            .Replace("{{CopyrightText}}", Encode(ResolveTemplateValue(settings.Template.CopyrightTextTemplate, settings)), StringComparison.Ordinal)
            .Replace("{{AppUrl}}", EncodeAttribute(settings.AppUrl), StringComparison.Ordinal)
            .Replace("{{FooterLinkText}}", Encode(ResolveTemplateValue(settings.Template.FooterLinkText, settings)), StringComparison.Ordinal)
            .Replace("{{IgnoreTextHtml}}", ResolveTemplateValue(settings.Template.IgnoreText, settings), StringComparison.Ordinal);
    }

    /// <summary>
    /// Renders the email template as plain text.
    /// </summary>
    ///
    /// <returns>The rendered plain text email body.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    internal string RenderText(EmailSettings settings)
    {
        StringBuilder text = new();
        
        text.AppendLine(Greeting)
            .AppendLine();

        foreach (string paragraph in Paragraphs)
        {
            text.AppendLine(paragraph)
                .AppendLine();
        }

        foreach (MailButton button in Buttons)
        {
            text.AppendLine($"{button.Label}: {button.Url}");
        }

        if (Buttons.Count > 0)
            text.AppendLine();

        return text.AppendLine(ResolveTemplateValue(settings.Template.CopyrightTextTemplate, settings))
            .AppendLine(ResolveTemplateValue(settings.Template.FooterLinkText, settings))
            .AppendLine(settings.Template.IgnoreText)
            .ToString().Trim();
    }

    /// <summary>
    /// Replaces template placeholders with values from the configured email settings.
    /// </summary>
    ///
    /// <param name="value">The template text containing placeholders to resolve.</param>
    /// <param name="settings">The configured email settings used for placeholder replacement.</param>
    ///
    /// <returns>The resolved template value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private static string ResolveTemplateValue(string value, EmailSettings settings) => value
        .Replace("{app_name}", settings.BrandName, StringComparison.OrdinalIgnoreCase)
        .Replace("{app_url}", settings.AppUrl, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Encodes text for safe HTML output.
    /// </summary>
    ///
    /// <param name="value">The text value to encode.</param>
    ///
    /// <returns>The encoded text value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private static string Encode(string value) => WebUtility.HtmlEncode(value);

    /// <summary>
    /// Encodes an attribute value for safe HTML output.
    /// </summary>
    ///
    /// <param name="value">The attribute value to encode.</param>
    ///
    /// <returns>The encoded attribute value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private static string EncodeAttribute(string value) => WebUtility.HtmlEncode(value);
}
