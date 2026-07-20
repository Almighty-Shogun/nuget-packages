using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Provides file helpers used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[SuppressMessage("Performance", "CA1865:Use char overload")]
internal static class ValidationFile
{
    private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    /// <summary>
    /// Attempts to read a value as one or more uploaded files.
    /// </summary>
    ///
    /// <param name="value">The value to read.</param>
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
    /// Checks whether an uploaded file has one of the allowed extensions.
    /// </summary>
    ///
    /// <param name="file">The uploaded file.</param>
    /// <param name="allowedExtensions">The allowed extensions.</param>
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
    /// Checks whether an uploaded file has one of the allowed MIME types.
    /// </summary>
    ///
    /// <param name="file">The uploaded file.</param>
    /// <param name="allowedMimeTypes">The allowed MIME types.</param>
    ///
    /// <returns><c>true</c> when the MIME type is allowed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool HasMimeType(IFormFile file, IReadOnlySet<string> allowedMimeTypes)
        => file.ContentType.Length > 0 && allowedMimeTypes.Contains(file.ContentType);

    /// <summary>
    /// Checks whether an uploaded file appears to be an image.
    /// </summary>
    ///
    /// <param name="file">The uploaded file.</param>
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

        return _contentTypeProvider.TryGetContentType("file" + extension, out string? contentType)
               && contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Normalizes file extensions for case-insensitive extension matching.
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
    /// Normalizes MIME types for case-insensitive MIME matching.
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
    /// Resolves MIME types from explicit MIME values or file extensions.
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

            if (_contentTypeProvider.TryGetContentType("file" + extension, out string? contentType))
                mimeTypes.Add(contentType);
        }

        return mimeTypes;
    }
}
