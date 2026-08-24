using System.Collections.Concurrent;

namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Loads the shared mail templates from the application's <c>mail</c> output directory, caching each file so a send after
/// the first touches no disk.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
internal sealed class FileEmailTemplateLoader : IEmailTemplateLoader
{
    /// <summary>
    /// The directory the templates are read from, resolved against the output directory rather than the working directory so
    /// it holds wherever the process was launched from.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    internal static readonly string TemplatesDirectory = Path.Combine(AppContext.BaseDirectory, "mail");

    /// <summary>
    /// The cached template reads. The task itself is cached, so concurrent first-time loads share one read rather than racing
    /// to perform the same file access. Nothing evicts an entry, so an edited template needs a restart to take effect.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ConcurrentDictionary<string, Task<string>> _templates = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public Task<string> LoadAsync(string templateName, CancellationToken cancellationToken = default)
        => _templates.GetOrAdd(templateName, name => ReadAsync(name, cancellationToken));

    /// <summary>
    /// Reads one template from disk, refusing a name that would climb out of the templates directory.
    /// </summary>
    ///
    /// <param name="templateName">The file name to read, combined with the templates directory and then checked.</param>
    /// <param name="cancellationToken">The token cancelling this read, which is the one baked into the cached task.</param>
    ///
    /// <returns>The template contents.</returns>
    ///
    /// <exception cref="ArgumentException">The name would resolve outside the templates directory.</exception>
    /// <exception cref="IOException">The file is missing, locked, or otherwise unreadable.</exception>
    ///
    /// <remarks>
    /// The check compares resolved paths rather than scanning for <c>..</c>, so it also covers an absolute path and a name
    /// that only escapes once the platform has normalized it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static Task<string> ReadAsync(string templateName, CancellationToken cancellationToken)
    {
        string templatePath = Path.GetFullPath(Path.Combine(TemplatesDirectory, templateName));

        if (!templatePath.StartsWith(TemplatesDirectory, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Template name '{templateName}' resolves outside the mail template directory.",
                nameof(templateName)
            );
        }

        return File.ReadAllTextAsync(templatePath, cancellationToken);
    }
}
