using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Normalizes maintenance paths for comparison.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class MaintenancePath
{
    /// <summary>
    /// Normalizes a path to a leading slash with no trailing slash.
    /// </summary>
    ///
    /// <param name="path">The path to normalize.</param>
    /// <param name="fallback">The path to use when <paramref name="path"/> is blank.</param>
    ///
    /// <returns>
    /// The normalized path, <c>/</c> for a slash-only value, or
    /// <see cref="PathString.Empty"/> when both inputs are blank.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
