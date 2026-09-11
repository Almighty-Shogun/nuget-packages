namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Provides validation for email URLs.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class MailUrl
{
    /// <summary>
    /// Determines whether the specified URL is allowed.
    /// </summary>
    ///
    /// <param name="url">The URL to validate.</param>
    ///
    /// <returns>
    /// <c>true</c> if the URL is absolute and uses a supported scheme; otherwise, <c>false</c>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static bool IsAllowed(string? url)
        => !string.IsNullOrWhiteSpace(url)
           && Uri.TryCreate(url, UriKind.Absolute, out Uri? parsed)
           && (parsed.Scheme == Uri.UriSchemeHttp || parsed.Scheme == Uri.UriSchemeHttps || parsed.Scheme == Uri.UriSchemeMailto);
}
