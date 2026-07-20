namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Represents image width and height.
/// </summary>
///
/// <param name="Width">The image width in pixels.</param>
/// <param name="Height">The image height in pixels.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record ImageDimensions(int Width, int Height);
