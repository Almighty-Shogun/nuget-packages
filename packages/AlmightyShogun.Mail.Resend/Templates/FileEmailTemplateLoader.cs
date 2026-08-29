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
    /// The completed template reads. Only a finished read is stored, never the task producing it, so a failure or a
    /// cancellation leaves nothing behind and the next send retries the file instead of replaying the first outcome.
    /// </summary>
    ///
    /// <remarks>
    /// Caching the task would let concurrent first-time loads share one read, at the cost of caching a faulted or canceled
    /// one forever: a template deleted after startup would then fail every later send, and one canceled send would poison
    /// the entry for every other caller. Racing to read the same small file twice is the cheaper mistake.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// Reads one template from disk, refusing a name that would climb out of the templates' directory.
    /// </summary>
    ///
    /// <param name="templateName">The file name to read, combined with the templates directory and then checked.</param>
    /// <param name="cancellationToken">The token cancelling this read alone, since nothing is cached until it completes.</param>
    ///
    /// <returns>The template contents.</returns>
    ///
    /// <exception cref="ArgumentException">The name would resolve outside the templates' directory.</exception>
    /// <exception cref="IOException">The file is missing, locked, or otherwise unreadable.</exception>
    ///
    /// <remarks>
    /// The check compares resolved paths rather than scanning for <c>..</c>, so it also covers an absolute path and a name
    /// that only escapes once the platform has normalized it.
    ///
    /// Containment is decided by <see cref="Path.GetRelativePath"/> rather than by a string prefix. A prefix test passes any
    /// sibling directory whose name merely starts with the templates directory, so <c>../mail-hacked/secret</c> under a
    /// <c>mail</c> root resolves to <c>mail-hacked/secret</c> and reads clean. A relative path that is rooted or that starts
    /// with <c>..</c> is the only thing that escapes, and that is what is rejected here.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
