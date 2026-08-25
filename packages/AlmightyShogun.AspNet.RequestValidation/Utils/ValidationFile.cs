using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.StaticFiles;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Reads uploads and checks their properties. A file property may be bound as one file or as several, so every helper accepts both and the
/// rules above never branch on which.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[SuppressMessage("Performance", "CA1865:Use char overload")]
internal static class ValidationFile
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    /// <summary>
    /// Reads a value as a set of uploads, so a single-file property and a multi-file one are checked by the same rule.
    /// </summary>
    ///
    /// <param name="value">The bound value to convert, which may already be the target type or may be text that has to be parsed.</param>
    /// <param name="files">The resolved uploaded files.</param>
    ///
    /// <returns><c>true</c> when files can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetFiles(object? value, out IReadOnlyList<IFormFile> files)
    {
        (bool isValid, IReadOnlyList<IFormFile> resolvedFiles) = value switch
        {
            IFormFile typed => (true, [typed]),
            IEnumerable<IFormFile> typed => (true, typed.ToArray()),
            _ => (false, [])
        };

        files = resolvedFiles;

        return isValid;
    }

    /// <summary>
    /// Checks an upload's extension against the permitted set, comparing case-insensitively and ignoring a leading dot on either side.
    /// </summary>
    ///
    /// <param name="file">The upload to inspect. Its metadata is read rather than its bytes, except where dimensions are involved.</param>
    /// <param name="allowedExtensions">
    /// The permitted extensions, matched case-insensitively and with a leading dot optional on either side.
    /// </param>
    ///
    /// <returns><c>true</c> when the extension is allowed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool HasExtension(IFormFile file, IReadOnlySet<string> allowedExtensions)
    {
        string extension = Path.GetExtension(file.FileName);

        if (extension.StartsWith(".", StringComparison.Ordinal))
            extension = extension[1..];

        return allowedExtensions.Contains(extension);
    }

    /// <summary>
    /// Checks an upload's content type against the permitted set, which is the client's claim rather than an inspection of the bytes.
    /// </summary>
    ///
    /// <param name="file">The upload to inspect. Its metadata is read rather than its bytes, except where dimensions are involved.</param>
    /// <param name="allowedMimeTypes">
    /// The permitted content types, compared against what the client claimed rather than the bytes sent.
    /// </param>
    ///
    /// <returns><c>true</c> when the MIME type is allowed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool HasMimeType(IFormFile file, IReadOnlySet<string> allowedMimeTypes)
        => file.ContentType.Length > 0 && allowedMimeTypes.Contains(file.ContentType);

    /// <summary>
    /// Checks whether an upload claims an image content type, which is a claim and not proof; the dimension rules read the header instead.
    /// </summary>
    ///
    /// <param name="file">The upload to inspect. Its metadata is read rather than its bytes, except where dimensions are involved.</param>
    ///
    /// <returns><c>true</c> when the file content type or extension indicates an image; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsImage(IFormFile file)
    {
        if (file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return true;

        string extension = Path.GetExtension(file.FileName);

        if (extension.Length == 0)
            return false;

        return ContentTypeProvider.TryGetContentType("file" + extension, out string? contentType)
               && contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Reduces extensions to a comparable form, so a set written with or without leading dots and in any case still matches.
    /// </summary>
    ///
    /// <param name="extensions">The extensions to normalize.</param>
    ///
    /// <returns>The normalized extension set.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlySet<string> NormalizeExtensions(IEnumerable<string> extensions) => extensions
        .Select(extension => extension.StartsWith(".", StringComparison.Ordinal) ? extension[1..] : extension)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Reduces content types to a comparable form, trimming and lowering so a header's spelling does not decide the outcome.
    /// </summary>
    ///
    /// <param name="mimeTypes">The MIME types to normalize.</param>
    ///
    /// <returns>The normalized MIME type set.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlySet<string> NormalizeMimeTypes(IEnumerable<string> mimeTypes)
        => mimeTypes.ToHashSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Accepts either a content type or an extension alias in the permitted set, so a rule can be written in whichever the caller thinks
    /// in.
    /// </summary>
    ///
    /// <param name="mimeTypesOrExtensions">The MIME values or extensions to resolve.</param>
    ///
    /// <returns>The resolved MIME type set.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlySet<string> ResolveMimeTypes(IEnumerable<string> mimeTypesOrExtensions)
    {
        HashSet<string> mimeTypes = new(StringComparer.OrdinalIgnoreCase);

        foreach (string value in mimeTypesOrExtensions)
        {
            if (value.Contains('/', StringComparison.Ordinal))
            {
                mimeTypes.Add(value);

                continue;
            }

            string extension = value.StartsWith(".", StringComparison.Ordinal) ? value : "." + value;

            if (ContentTypeProvider.TryGetContentType("file" + extension, out string? contentType))
                mimeTypes.Add(contentType);
        }

        return mimeTypes;
    }
}
