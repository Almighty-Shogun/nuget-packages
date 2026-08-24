namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Checks that a URL is safe to place in an <c>href</c> or <c>src</c>. Shared, because the check has to agree between the
/// point a button is constructed and the point a settings URL is rendered.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class MailUrl
{
    /// <summary>
    /// Determines whether a URL is absolute and uses a scheme that cannot execute script when a mail client follows it.
    /// </summary>
    ///
    /// <param name="url">The URL to check. <c>null</c> and blank are rejected rather than treated as absent.</param>
    ///
    /// <returns>
    /// <c>true</c> when the URL is absolute and uses http, https, or mailto. A relative URL is rejected, since an email has
    /// no base to resolve one against.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static bool IsAllowed(string? url)
        => !string.IsNullOrWhiteSpace(url)
           && Uri.TryCreate(url, UriKind.Absolute, out Uri? parsed)
           && (parsed.Scheme == Uri.UriSchemeHttp || parsed.Scheme == Uri.UriSchemeHttps || parsed.Scheme == Uri.UriSchemeMailto);
}
