using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Constrains an upload by its presence, its type, its extension, or the dimensions of the image it holds.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class FileConstraintValidationRule<TRequest, TProperty>
    : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// Which file constraint this rule enforces, and with it which message key a failure reports.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly FileConstraintMode _mode;

    /// <summary>
    /// The configured values as written, kept for the failure message so a client is told what was accepted.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlyList<string> _values;

    /// <summary>
    /// The same values prepared for comparison, so each file is matched without normalising the configured list again per request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlySet<string> _normalizedValues;

    /// <summary>
    /// The width and height bounds, set only for the dimension modes and left null for every other constraint.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ImageDimensionConstraints? _dimensionConstraints;

    /// <summary>
    /// Builds the upload rule from whichever constraint the caller supplied, since the families share one class.
    /// </summary>
    ///
    /// <param name="mode">Which property of the upload is constrained, which also picks the message the failure reports.</param>
    /// <param name="values">The values compared against, absent for a rule whose constraint needs none.</param>
    /// <param name="dimensionConstraints">The width and height to enforce, absent for a file rule that constrains something else.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public FileConstraintValidationRule(
        FileConstraintMode mode,
        IReadOnlyList<string>? values = null,
        ImageDimensionConstraints? dimensionConstraints = null
    )
    {
        _mode = mode;
        _values = values ?? [];
        _dimensionConstraints = dimensionConstraints;
        _normalizedValues = mode switch
        {
            FileConstraintMode.Extensions => ValidationFile.NormalizeExtensions(_values),
            FileConstraintMode.Mimes => ValidationFile.ResolveMimeTypes(_values),
            FileConstraintMode.MimeTypes => ValidationFile.NormalizeMimeTypes(_values),
            _ => new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        };
    }

    /// <inheritdoc />
    public async ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        if (ValidationValue.IsEmpty(value))
            return ValidationRuleResult.Success();

        if (!ValidationFile.TryGetFiles(value, out IReadOnlyList<IFormFile> files))
            return ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters(field));

        bool isValid = _mode switch
        {
            FileConstraintMode.Uploaded => files.All(file => file.Length > 0),
            FileConstraintMode.Image => await AreAllImagesAsync(files, cancellationToken),
            FileConstraintMode.Extensions => files.All(file => ValidationFile.HasExtension(file, _normalizedValues)),
            FileConstraintMode.Mimes => files.All(file => ValidationFile.HasMimeType(file, _normalizedValues)),
            FileConstraintMode.MimeTypes => files.All(file => ValidationFile.HasMimeType(file, _normalizedValues)),
            FileConstraintMode.Dimensions or FileConstraintMode.MinDimensions or FileConstraintMode.MaxDimensions
                => await HasValidDimensionsAsync(files, cancellationToken),
            _ => throw new InvalidOperationException($"Unsupported FileConstraintMode value '{_mode}'.")
        };

        return isValid ? ValidationRuleResult.Success() : ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters(field));
    }

    /// <summary>
    /// Checks whether every upload really is an image, reading each one's leading bytes rather than trusting its name or content type.
    /// </summary>
    ///
    /// <param name="files">The uploaded files.</param>
    /// <param name="cancellationToken">Cancels the work a rule does on its own, such as reading an uploaded file.</param>
    ///
    /// <returns><c>true</c> when every file opens with a recognized image signature; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static async Task<bool> AreAllImagesAsync(IReadOnlyList<IFormFile> files, CancellationToken cancellationToken)
    {
        foreach (IFormFile file in files)
            if (!await ValidationFile.IsImageAsync(file, cancellationToken))
                return false;

        return true;
    }

    /// <summary>
    /// Checks whether all uploaded files match the configured image dimension constraints.
    /// </summary>
    ///
    /// <param name="files">The uploaded files.</param>
    /// <param name="cancellationToken">Cancels the work a rule does on its own, such as reading an uploaded file.</param>
    ///
    /// <returns><c>true</c> when all files have valid dimensions; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<bool> HasValidDimensionsAsync(IReadOnlyList<IFormFile> files, CancellationToken cancellationToken)
    {
        if (_dimensionConstraints is null)
            return false;

        foreach (IFormFile file in files)
        {
            ImageDimensions? dimensions = await ImageDimensionsReader.TryReadAsync(file, cancellationToken);

            if (dimensions is null || !MatchesDimensions(dimensions))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Applies the configured dimension comparison, which is the only check here that must read the file's bytes rather than its metadata.
    /// </summary>
    ///
    /// <param name="dimensions">The pair read from the header, compared against the constraint this rule holds.</param>
    ///
    /// <returns><c>true</c> when the dimensions match; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool MatchesDimensions(ImageDimensions dimensions) => _mode switch
    {
        FileConstraintMode.Dimensions => _dimensionConstraints!.MatchesExact(dimensions),
        FileConstraintMode.MinDimensions => _dimensionConstraints!.MatchesMinimum(dimensions),
        FileConstraintMode.MaxDimensions => _dimensionConstraints!.MatchesMaximum(dimensions),
        _ => throw new InvalidOperationException($"Unsupported FileConstraintMode dimension value '{_mode}'.")
    };

    /// <summary>
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => _mode switch
    {
        FileConstraintMode.Uploaded => "validation.uploaded",
        FileConstraintMode.Image => "validation.image",
        FileConstraintMode.Extensions => "validation.extensions",
        FileConstraintMode.Mimes => "validation.mimes",
        FileConstraintMode.MimeTypes => "validation.mimetypes",
        FileConstraintMode.Dimensions or FileConstraintMode.MinDimensions or FileConstraintMode.MaxDimensions
            => "validation.dimensions",
        _ => throw new InvalidOperationException($"Unsupported FileConstraintMode value '{_mode}'.")
    };

    /// <summary>
    /// Maps the configured mode onto the values a message template substitutes, so the bounds a rule was built with appear in the sentence
    /// the client reads.
    /// </summary>
    ///
    /// <param name="field">The field being validated.</param>
    ///
    /// <returns>The validation message parameters.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private object?[] GetMessageParameters(string field) => _mode switch
    {
        FileConstraintMode.Uploaded => [field],
        FileConstraintMode.Extensions or FileConstraintMode.Mimes or FileConstraintMode.MimeTypes
            => [ValidationDisplay.JoinValues(_values)],
        _ => []
    };
}
