namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// The width and height a dimension rule compares against, held together so the general file constructor stays free of dimension arguments.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ImageDimensionConstraints(int width, int height)
{
    /// <summary>
    /// Requires both dimensions to match exactly, which is the rule for an image that must be a specific size rather than within a range.
    /// </summary>
    ///
    /// <param name="dimensions">The pair read from the image header, compared against the constraint this rule was built with.</param>
    ///
    /// <returns><c>true</c> when the dimensions match exactly; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool MatchesExact(ImageDimensions dimensions) => dimensions.Width == width && dimensions.Height == height;

    /// <summary>
    /// Requires both dimensions to reach the configured pair, so an image wide enough but too short still fails.
    /// </summary>
    ///
    /// <param name="dimensions">The pair read from the image header, compared against the constraint this rule was built with.</param>
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
    /// <param name="dimensions">The pair read from the image header, compared against the constraint this rule was built with.</param>
    ///
    /// <returns><c>true</c> when the dimensions meet the maximum; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool MatchesMaximum(ImageDimensions dimensions) => dimensions.Width <= width && dimensions.Height <= height;
}
