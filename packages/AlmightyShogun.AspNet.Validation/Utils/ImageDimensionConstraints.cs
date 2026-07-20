namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Stores image dimension constraints for image validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ImageDimensionConstraints(int width, int height)
{
    /// <summary>
    /// Checks whether image dimensions exactly match the configured width and height.
    /// </summary>
    ///
    /// <param name="dimensions">The dimensions to check.</param>
    ///
    /// <returns><c>true</c> when the dimensions match exactly; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool MatchesExact(ImageDimensions dimensions) => dimensions.Width == width && dimensions.Height == height;

    /// <summary>
    /// Checks whether image dimensions are at least the configured width and height.
    /// </summary>
    ///
    /// <param name="dimensions">The dimensions to check.</param>
    ///
    /// <returns><c>true</c> when the dimensions meet the minimum; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool MatchesMinimum(ImageDimensions dimensions) => dimensions.Width >= width && dimensions.Height >= height;

    /// <summary>
    /// Checks whether image dimensions are no larger than the configured width and height.
    /// </summary>
    ///
    /// <param name="dimensions">The dimensions to check.</param>
    ///
    /// <returns><c>true</c> when the dimensions meet the maximum; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool MatchesMaximum(ImageDimensions dimensions) => dimensions.Width <= width && dimensions.Height <= height;
}
