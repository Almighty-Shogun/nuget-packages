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
    /// <param name="fallback">The value used when the path is blank.</param>
    ///
    /// <returns>The normalized path.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static PathString Normalize(string? path, string? fallback = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            return fallback is null ? PathString.Empty : new PathString(fallback);

        string trimmed = path.Trim();

        return new PathString(trimmed.StartsWith('/') ? trimmed.TrimEnd('/') : $"/{trimmed.TrimEnd('/')}");
    }
}
