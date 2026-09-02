using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Normalizes configured maintenance paths into a comparable form.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class MaintenancePath
{
    /// <summary>
    /// Normalizes a configured path into a leading-slash form without a trailing slash.
    /// </summary>
    ///
    /// <param name="path">The configured path.</param>
    /// <param name="fallback">The value used when the path is blank, normalized the same way rather than taken as given.</param>
    ///
    /// <returns>
    /// The path with a leading slash and no trailing one, <c>/</c> for a value that is nothing but slashes, or
    /// <see cref="PathString.Empty"/> when the path and the fallback are both blank.
    /// </returns>
    ///
    /// <remarks>
    /// The fallback goes through the same normalization as the path, because <see cref="PathString"/> rejects a non-empty value that does
    /// not start with a slash: passing one straight through would throw rather than produce an odd path.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static PathString Normalize(string? path, string? fallback = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            path = fallback;

        if (string.IsNullOrWhiteSpace(path))
            return PathString.Empty;

        string trimmed = path.Trim().TrimEnd('/');

        return trimmed.Length is 0
            ? new PathString("/")
            : new PathString(trimmed.StartsWith('/') ? trimmed : $"/{trimmed}");
    }
}
