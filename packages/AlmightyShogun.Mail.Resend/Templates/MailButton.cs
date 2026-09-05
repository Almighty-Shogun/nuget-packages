namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents a call-to-action button rendered into both the HTML and the plain-text body, validated at construction so an
/// unsafe URL cannot reach either.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public sealed record MailButton
{
    /// <summary>
    /// Creates a button, rejecting a URL that cannot safely be rendered.
    /// </summary>
    ///
    /// <param name="label">The visible text, HTML encoded when rendered, so markup in it is shown rather than applied.</param>
    /// <param name="url">The absolute destination, which must use http, https, or mailto.</param>
    ///
    /// <exception cref="ArgumentException">The label is blank, or the URL is blank or not an accepted scheme.</exception>
    ///
    /// <remarks>
    /// Validated here rather than at render time, because the HTML and plain-text renderers are separate paths. Encoding
    /// the HTML one alone would leave a <c>javascript:</c> URL visible verbatim in the plain-text alternative.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public MailButton(string label, string url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        if (!MailUrl.IsAllowed(url))
        {
            throw new ArgumentException(
                $"Button URL '{url}' must be an absolute http, https, or mailto URL.",
                nameof(url)
            );
        }

        Url = url;
        Label = label;
    }

    /// <summary>
    /// Gets the visible text, encoded into the HTML body and written verbatim into the plain-text one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string Label { get; }

    /// <summary>
    /// Gets the destination, checked against the accepted schemes at construction, which is what keeps an unsafe one out of
    /// the plain-text body the renderer does not encode.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string Url { get; }
}
