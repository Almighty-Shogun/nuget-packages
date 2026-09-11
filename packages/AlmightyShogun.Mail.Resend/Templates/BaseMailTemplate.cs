using System.Net;
using System.Text;
using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents a base class for email templates.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public abstract partial class BaseMailTemplate
{
    /// <summary>
    /// Gets the email subject.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public abstract string Subject { get; }

    /// <summary>
    /// Gets the email title.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    protected abstract string Title { get; }

    /// <summary>
    /// Gets the email greeting.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    protected abstract string Greeting { get; }

    /// <summary>
    /// Gets the email paragraphs.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    protected virtual IReadOnlyList<string> Paragraphs => [];

    /// <summary>
    /// Gets the email buttons.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    protected virtual IReadOnlyList<MailButton> Buttons => [];

    /// <summary>
    /// Gets additional template placeholder values.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    protected virtual IReadOnlyDictionary<string, string> AdditionalValues => FrozenDictionary<string, string>.Empty;

    /// <summary>
    /// Renders the template as HTML.
    /// </summary>
    ///
    /// <param name="templateHtml">The email template.</param>
    /// <param name="paragraphTemplateHtml">The paragraph template.</param>
    /// <param name="buttonTemplateHtml">The button template.</param>
    /// <param name="settings">The email settings.</param>
    ///
    /// <returns>The rendered HTML</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    internal string Render(string templateHtml, string paragraphTemplateHtml, string buttonTemplateHtml, EmailSettings settings)
    {
        string bodyHtml = string.Join(string.Empty,
            Paragraphs.Select(paragraph => paragraphTemplateHtml.Replace("{{Paragraph}}", Encode(paragraph), StringComparison.Ordinal)));

        string buttonsHtml = string.Join(string.Empty,
            Buttons.Select(button => buttonTemplateHtml
                .Replace("{{ButtonUrl}}", EncodeUrl(button.Url), StringComparison.Ordinal)
                .Replace("{{ButtonLabel}}", Encode(button.Label), StringComparison.Ordinal)));

        return ApplyAdditionalValues(templateHtml.Replace("{{DocumentTitle}}", Encode(Title), StringComparison.Ordinal)
            .Replace("{{LogoUrl}}", EncodeUrl(settings.LogoUrl), StringComparison.Ordinal)
            .Replace("{{BrandName}}", Encode(settings.BrandName), StringComparison.Ordinal)
            .Replace("{{Title}}", Encode(Title), StringComparison.Ordinal)
            .Replace("{{Greeting}}", Encode(Greeting), StringComparison.Ordinal)
            .Replace(
                "{{CopyrightText}}",
                Encode(ResolveTemplateValue(settings.Template.CopyrightText, settings)),
                StringComparison.Ordinal
            )
            .Replace("{{AppUrl}}", EncodeUrl(settings.AppUrl), StringComparison.Ordinal)
            .Replace(
                "{{FooterLinkText}}",
                Encode(ResolveTemplateValue(settings.Template.FooterLinkText, settings)),
                StringComparison.Ordinal
            )
            .Replace("{{IgnoreTextHtml}}", ResolveTemplateValue(settings.Template.IgnoreText, settings), StringComparison.Ordinal))
            .Replace("{{BodyHtml}}", bodyHtml, StringComparison.Ordinal)
            .Replace("{{ButtonsHtml}}", buttonsHtml, StringComparison.Ordinal);
    }

    /// <summary>
    /// Renders the template as plain text.
    /// </summary>
    ///
    /// <param name="settings">The email settings.</param>
    ///
    /// <returns>The rendered plain-text body.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    internal string RenderText(EmailSettings settings)
    {
        StringBuilder text = new();

        if (!string.IsNullOrWhiteSpace(Title))
            text.AppendLine(Title).AppendLine();

        text.AppendLine(Greeting).AppendLine();

        foreach (string paragraph in Paragraphs)
            text.AppendLine(paragraph).AppendLine();

        foreach (MailButton button in Buttons)
            text.AppendLine($"{button.Label}: {button.Url}");

        if (Buttons.Count > 0)
            text.AppendLine();

        return text.AppendLine(ResolveTemplateValue(settings.Template.CopyrightText, settings))
            .AppendLine(ResolveTemplateValue(settings.Template.FooterLinkText, settings))
            .AppendLine(ToPlainText(ResolveTemplateValue(settings.Template.IgnoreText, settings)))
            .ToString()
            .Trim();
    }

    /// <summary>
    /// Resolves placeholders in a configured template value.
    /// </summary>
    ///
    /// <param name="value">The template value.</param>
    /// <param name="settings">The email settings.</param>
    ///
    /// <returns>The resolved value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private static string ResolveTemplateValue(string value, EmailSettings settings) => value
        .Replace("{app_name}", settings.BrandName, StringComparison.OrdinalIgnoreCase)
        .Replace(
            "{app_url}",
            MailUrl.IsAllowed(settings.AppUrl) ? settings.AppUrl! : string.Empty,
            StringComparison.OrdinalIgnoreCase
        );

    /// <summary>
    /// Applies additional placeholder values to the HTML.
    /// </summary>
    ///
    /// <param name="html">The HTML to process.</param>
    ///
    /// <returns>The processed HTML.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private string ApplyAdditionalValues(string html) => AdditionalValues
        .Aggregate(html, (current, value) => current.Replace($"{{{{{value.Key}}}}}", Encode(value.Value), StringComparison.Ordinal));

    /// <summary>
    /// HTML-encodes the specified value.
    /// </summary>
    ///
    /// <param name="value">The value to encode.</param>
    ///
    /// <returns>The encoded value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private static string Encode(string value) => WebUtility.HtmlEncode(value);

    /// <summary>
    /// Converts HTML content to plain text.
    /// </summary>
    ///
    /// <param name="html">The HTML content.</param>
    ///
    /// <returns>The plain-text content.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private static string ToPlainText(string html)
        => WebUtility.HtmlDecode(HtmlRegex().Replace(NewLineRegex().Replace(html, "\n"), string.Empty));

    /// <summary>
    /// Encodes a URL for use in HTML.
    /// </summary>
    ///
    /// <param name="value">The URL to encode.</param>
    ///
    /// <returns>The encoded URL, or an empty string when the URL is not allowed.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private static string EncodeUrl(string? value)
        => MailUrl.IsAllowed(value) ? WebUtility.HtmlEncode(value) ?? string.Empty : string.Empty;

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex HtmlRegex();

    [GeneratedRegex("<br\\s*/?>", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex NewLineRegex();
}
