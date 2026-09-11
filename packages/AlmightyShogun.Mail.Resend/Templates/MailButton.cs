namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents a call-to-action button in an email.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public sealed record MailButton
{
    /// <summary>
    /// Initializes a new <see cref="MailButton"/>.
    /// </summary>
    ///
    /// <param name="label">The button label.</param>
    /// <param name="url">The button URL.</param>
    ///
    /// <exception cref="ArgumentException"><paramref name="label"/> or <paramref name="url"/> is empty or whitespace,
    /// or <paramref name="url"/> uses an unsupported scheme.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public MailButton(string label, string url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        if (!MailUrl.IsAllowed(url))
            throw new ArgumentException(
                $"Button URL '{url}' must be an absolute http, https, or mailto URL.",
                nameof(url)
            );

        Url = url;
        Label = label;
    }

    /// <summary>
    /// Gets the button label.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string Label { get; }

    /// <summary>
    /// Gets the button URL.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public string Url { get; }
}
