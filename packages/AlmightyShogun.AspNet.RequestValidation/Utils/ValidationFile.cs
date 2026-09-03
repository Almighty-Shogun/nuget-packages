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
    /// <summary>
    /// Maps a file name to the content type its extension implies, so a declared type can be checked against the name rather than
    /// trusted. Shared because building one walks a sizeable table.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

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
    /// Checks whether an upload really is an image, by the bytes it opens with rather than by the name or content type the client sent.
    /// </summary>
    ///
    /// <param name="file">The upload to inspect. Its leading bytes are read, since nothing in its metadata is evidence.</param>
    /// <param name="cancellationToken">The token cancelling the read.</param>
    ///
    /// <returns><c>true</c> when the file opens with a recognized image signature; otherwise, <c>false</c>.</returns>
    ///
    /// <remarks>
    /// The name and the content type are both written by the caller, so a file called <c>photo.png</c> announced as <c>image/png</c>
    /// proves nothing about what it holds. Only <see cref="ImageSignature"/> decides, which is what stops arbitrary content passing an
    /// image rule by being named after one.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static Task<bool> IsImageAsync(IFormFile file, CancellationToken cancellationToken)
        => ImageSignature.IsImageAsync(file, cancellationToken);

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
    /// Collects the content types into a set compared case-insensitively, so a header's casing does not decide the outcome. Surrounding
    /// whitespace is not trimmed, so a configured value carrying any will not match.
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

            if (_contentTypeProvider.TryGetContentType("file" + extension, out string? contentType))
                mimeTypes.Add(contentType);
        }

        return mimeTypes;
    }
}
