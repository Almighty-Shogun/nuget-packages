using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates uploaded file constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class FileConstraintValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    private readonly FileConstraintMode _mode;

    private readonly IReadOnlyList<string> _values;

    private readonly IReadOnlySet<string> _normalizedValues;

    private readonly ImageDimensionConstraints? _dimensionConstraints;

    /// <summary>
    /// Creates a file constraint validation rule.
    /// </summary>
    ///
    /// <param name="mode">The file constraint mode.</param>
    /// <param name="values">The optional comparison values.</param>
    /// <param name="dimensionConstraints">The optional image dimension constraints.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public FileConstraintValidationRule(FileConstraintMode mode, IReadOnlyList<string>? values = null, ImageDimensionConstraints? dimensionConstraints = null)
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
    public async ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValidationRuleResult.Success();

        if (!ValidationFile.TryGetFiles(value, out IReadOnlyList<IFormFile> files))
            return ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters(field));

        bool isValid = _mode switch
        {
            FileConstraintMode.Uploaded => true,
            FileConstraintMode.Image => files.All(ValidationFile.IsImage),
            FileConstraintMode.Extensions => files.All(file => ValidationFile.HasExtension(file, _normalizedValues)),
            FileConstraintMode.Mimes => files.All(file => ValidationFile.HasMimeType(file, _normalizedValues)),
            FileConstraintMode.MimeTypes => files.All(file => ValidationFile.HasMimeType(file, _normalizedValues)),
            FileConstraintMode.Dimensions or FileConstraintMode.MinDimensions or FileConstraintMode.MaxDimensions => await HasValidDimensionsAsync(files, cancellationToken),
            _ => false
        };

        return isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters(field));
    }

    /// <summary>
    /// Checks whether all uploaded files match the configured image dimension constraints.
    /// </summary>
    ///
    /// <param name="files">The uploaded files.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
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
    /// Checks image dimensions against the configured dimension mode.
    /// </summary>
    ///
    /// <param name="dimensions">The image dimensions.</param>
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
        _ => false
    };

    /// <summary>
    /// Gets the validation message key for the configured file constraint mode.
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
        _ => "validation.dimensions"
    };

    /// <summary>
    /// Gets the validation message parameters for the configured file constraint mode.
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
        FileConstraintMode.Extensions or FileConstraintMode.Mimes or FileConstraintMode.MimeTypes => [ValidationValue.JoinValues(_values)],
        _ => []
    };
}
