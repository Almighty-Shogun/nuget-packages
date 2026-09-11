using System.Collections.Concurrent;

namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Loads and caches email templates from the mail directory.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
internal sealed class FileEmailTemplateLoader : IEmailTemplateLoader
{
    /// <summary>
    /// Gets the mail templates directory.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    internal static readonly string TemplatesDirectory = Path.Combine(AppContext.BaseDirectory, "mail");

    /// <summary>
    /// Stores cached templates by name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly ConcurrentDictionary<string, string> _templates = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public async Task<string> LoadAsync(string templateName, CancellationToken cancellationToken = default)
    {
        if (_templates.TryGetValue(templateName, out string? cached))
            return cached;

        string template = await ReadAsync(templateName, cancellationToken);

        return _templates.GetOrAdd(templateName, template);
    }

    /// <summary>
    /// Reads a template from disk.
    /// </summary>
    ///
    /// <param name="templateName">The template name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The template contents.</returns>
    ///
    /// <exception cref="ArgumentException">The template resolves outside the mail directory.</exception>
    /// <exception cref="IOException">The template could not be read.</exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The template could not be accessed.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// The operation was canceled.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static Task<string> ReadAsync(string templateName, CancellationToken cancellationToken)
    {
        string root = Path.GetFullPath(TemplatesDirectory);
        string templatePath = Path.GetFullPath(Path.Combine(root, templateName));
        string relative = Path.GetRelativePath(root, templatePath);

        bool isOutside = Path.IsPathRooted(relative)
                         || relative == ".."
                         || relative.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal);

        if (isOutside)
            throw new ArgumentException(
                $"Template name '{templateName}' resolves outside the mail template directory.",
                nameof(templateName)
            );

        return File.ReadAllTextAsync(templatePath, cancellationToken);
    }
}
