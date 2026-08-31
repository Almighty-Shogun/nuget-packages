namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Loads the shared HTML fragments the renderer assembles a message from. Internal, because the fragment names are the
/// package's own and an application customizes the output by supplying different files, not a different loader.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
internal interface IEmailTemplateLoader
{
    /// <summary>
    /// Loads one shared template by file name.
    /// </summary>
    ///
    /// <param name="templateName">The file name inside the mail directory, which may not escape it.</param>
    /// <param name="cancellationToken">The token cancelling the read, and only if the file has not already been cached.</param>
    ///
    /// <returns>The template contents, which an implementation may serve from a cache rather than from disk.</returns>
    ///
    /// <exception cref="IOException">The file could not be read.</exception>
    /// <exception cref="ArgumentException">The name resolves outside the mail directory.</exception>
    /// <exception cref="UnauthorizedAccessException">The process may not read the file.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was signaled during the read.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    Task<string> LoadAsync(string templateName, CancellationToken cancellationToken = default);
}
