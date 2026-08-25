namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// The pixel dimensions read from an image header, in the order a person writes them.
/// </summary>
///
/// <param name="Width">The image width in pixels.</param>
/// <param name="Height">The image height in pixels.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record ImageDimensions(int Width, int Height);
