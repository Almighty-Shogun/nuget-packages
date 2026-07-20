namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the file constraint options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum FileConstraintMode
{
    /// <summary>
    /// Requires the value to contain uploaded files.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Uploaded,

    /// <summary>
    /// Requires uploaded files to be images.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Image,

    /// <summary>
    /// Requires uploaded files to use configured file extensions.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Extensions,

    /// <summary>
    /// Requires uploaded files to match MIME types resolved from configured extensions or MIME values.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Mimes,

    /// <summary>
    /// Requires uploaded files to match configured MIME type values.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MimeTypes,

    /// <summary>
    /// Requires uploaded images to match the exact configured dimensions.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Dimensions,

    /// <summary>
    /// Requires uploaded images to meet the configured minimum dimensions.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MinDimensions,

    /// <summary>
    /// Requires uploaded images to stay within the configured maximum dimensions.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MaxDimensions
}
