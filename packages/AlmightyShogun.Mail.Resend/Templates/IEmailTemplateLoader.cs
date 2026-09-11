namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Provides email template loading.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
internal interface IEmailTemplateLoader
{
    /// <summary>
    /// Loads an email template.
    /// </summary>
    ///
    /// <param name="templateName">The template name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The template contents.</returns>
    ///
    /// <exception cref="IOException">The template could not be read.</exception>
    /// <exception cref="ArgumentException">The template resolves outside the mail directory.</exception>
    /// <exception cref="UnauthorizedAccessException">The template could not be accessed.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    Task<string> LoadAsync(string templateName, CancellationToken cancellationToken = default);
}
