using System.Net;
using System.Text;
using System.Collections.Frozen;

namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Base class for application email templates. A subclass supplies the content as values, and this class renders both the
/// HTML and the plain-text body from them, so a template never handles encoding or the shared chrome itself.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public abstract class BaseMailTemplate
{
    /// <summary>
    /// Gets the subject line. Public because the mail service reads it when building the message, and it is the one value
    /// not rendered into either body.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public abstract string Subject { get; }

    /// <summary>
    /// Gets the heading rendered above the greeting, and the document title in the HTML head. A blank value is omitted from
    /// the plain-text body rather than leaving a leading blank line.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    protected abstract string Title { get; }

    /// <summary>
    /// Gets the opening line addressing the recipient. It is always rendered, so a template with nothing to greet should
    /// return an empty string rather than leaving it unimplemented.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    protected abstract string Greeting { get; }

    /// <summary>
    /// Gets the body paragraphs, each wrapped in the shared paragraph fragment and HTML encoded. Empty by default, which
    /// renders a message with only a greeting and buttons.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    protected virtual IReadOnlyList<string> Paragraphs => [];

    /// <summary>
    /// Gets the buttons rendered after the paragraphs, and repeated in the plain-text body as label and URL pairs so the
    /// destination survives for a client that shows only text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    protected virtual IReadOnlyList<MailButton> Buttons => [];

    /// <summary>
    /// Gets extra placeholder values for the template, written as <c>{{Key}}</c> and replaced after the built-in
    /// placeholders. Values are HTML encoded.
    /// </summary>
    ///
    /// <remarks>
    /// Override this to add template fields without changing the package or introducing a template engine.
    ///
    /// The default is the shared empty <see cref="FrozenDictionary{TKey,TValue}"/> rather than a new dictionary, because this
    /// is read on every render and a template that adds no fields should allocate nothing to say so.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected virtual IReadOnlyDictionary<string, string> AdditionalValues => FrozenDictionary<string, string>.Empty;

    /// <summary>
    /// Assembles the HTML body from the shared fragments, filling the chrome from settings and the content from this
    /// template.
    /// </summary>
    ///
    /// <param name="templateHtml">The document fragment, holding the placeholders for the chrome and the assembled body.</param>
    /// <param name="paragraphTemplateHtml">The fragment repeated once per entry in <see cref="Paragraphs"/>.</param>
    /// <param name="buttonTemplateHtml">The fragment repeated once per entry in <see cref="Buttons"/>.</param>
    /// <param name="settings">The bound settings supplying the brand, logo, and footer values.</param>
    ///
    /// <returns>The rendered HTML body, with every interpolated value encoded.</returns>
    ///
    /// <remarks>
    /// Built-in placeholders are replaced before the subclass ones, so a value returned by <see cref="AdditionalValues"/>
    /// cannot inject a placeholder that then gets substituted. Every value is encoded on its way in, which is why the
    /// fragments are plain HTML rather than a template language.
    /// </remarks>
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
            .Replace("{{BodyHtml}}", bodyHtml, StringComparison.Ordinal)
            .Replace("{{ButtonsHtml}}", buttonsHtml, StringComparison.Ordinal)
            .Replace(
                "{{CopyrightText}}",
                Encode(ResolveTemplateValue(settings.Template.CopyrightTextTemplate, settings)),
                StringComparison.Ordinal
            )
            .Replace("{{AppUrl}}", EncodeUrl(settings.AppUrl), StringComparison.Ordinal)
            .Replace(
                "{{FooterLinkText}}",
                Encode(ResolveTemplateValue(settings.Template.FooterLinkText, settings)),
                StringComparison.Ordinal
            )
            .Replace("{{IgnoreTextHtml}}", Encode(ResolveTemplateValue(settings.Template.IgnoreText, settings)), StringComparison.Ordinal));
    }

    /// <summary>
    /// Renders the template as plain text, sent alongside the HTML for clients that will not display it.
    /// </summary>
    ///
    /// <param name="settings">The bound settings supplying the footer values.</param>
    ///
    /// <returns>The rendered plain-text body, trimmed of the trailing blank lines the footer would otherwise leave.</returns>
    ///
    /// <remarks>
    /// Nothing is encoded here, because there is no markup to escape. <see cref="AdditionalValues"/> is still not applied, so
    /// a template relying on an additional value for its wording renders it only in the HTML.
    ///
    /// The configurable footer text goes through the same resolution the HTML body applies, so <c>{app_name}</c> and
    /// <c>{app_url}</c> read the same in both bodies rather than reaching the reader unsubstituted here.
    /// </remarks>
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

        return text.AppendLine(ResolveTemplateValue(settings.Template.CopyrightTextTemplate, settings))
            .AppendLine(ResolveTemplateValue(settings.Template.FooterLinkText, settings))
            .AppendLine(ResolveTemplateValue(settings.Template.IgnoreText, settings))
            .ToString()
            .Trim();
    }

    /// <summary>
    /// Substitutes the <c>{app_name}</c> and <c>{app_url}</c> placeholders shared by the configurable footer text.
    /// </summary>
    ///
    /// <param name="value">The configured text, which may contain neither, either, nor both placeholders.</param>
    /// <param name="settings">The bound settings the replacements are read from.</param>
    ///
    /// <returns>The text with both placeholders substituted, an unset URL becoming an empty string.</returns>
    ///
    /// <remarks>
    /// Matched case-insensitively, so configuration written as <c>{App_Name}</c> still resolves. This runs before encoding,
    /// which is what keeps a brand name containing markup from reaching the document.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private static string ResolveTemplateValue(string value, EmailSettings settings) => value
        .Replace("{app_name}", settings.BrandName, StringComparison.OrdinalIgnoreCase)
        .Replace("{app_url}", settings.AppUrl ?? string.Empty, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Applies the subclass-supplied placeholder values, written as <c>{{Key}}</c> in the template.
    /// </summary>
    ///
    /// <param name="html">The HTML with the built-in placeholders already replaced.</param>
    ///
    /// <returns>The HTML with each additional value encoded and substituted, unmatched keys left untouched.</returns>
    ///
    /// <remarks>
    /// A key naming a built-in placeholder has no effect, since that one was already replaced. Enumeration order decides
    /// nothing, because each key is substituted independently.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string ApplyAdditionalValues(string html) => AdditionalValues
        .Aggregate(html, (current, value) => current.Replace($"{{{{{value.Key}}}}}", Encode(value.Value), StringComparison.Ordinal));

    /// <summary>
    /// Encodes text for safe HTML output, applied to every interpolated value without exception.
    /// </summary>
    ///
    /// <param name="value">The text to encode.</param>
    ///
    /// <returns>The text with the HTML-significant characters replaced by entities.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private static string Encode(string value) => WebUtility.HtmlEncode(value);

    /// <summary>
    /// Encodes a URL for safe use in an <c>href</c> or <c>src</c>, dropping anything that is not an accepted scheme.
    /// </summary>
    ///
    /// <param name="value">The URL to encode, which may be <c>null</c> when the setting behind it is unset.</param>
    ///
    /// <returns>The encoded URL, or an empty string when it is unset or uses a scheme that is not accepted.</returns>
    ///
    /// <remarks>
    /// Dropping the value rather than throwing keeps one bad configured URL from failing every send, at the cost of a logo or
    /// footer link silently disappearing. A button takes the opposite trade and throws at construction instead.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private static string EncodeUrl(string? value)
        => MailUrl.IsAllowed(value) ? WebUtility.HtmlEncode(value) ?? string.Empty : string.Empty;
}
