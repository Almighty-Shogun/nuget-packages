namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Provides utilities for fitting values within database column limits.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
internal static class ColumnValue
{
    /// <summary>
    /// Truncates a string to the specified maximum length.
    /// </summary>
    ///
    /// <param name="value">The value to truncate.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    ///
    /// <returns>The truncated value, or null if the input is null.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    internal static string? Truncate(string? value, int maxLength)
        => value is null || value.Length <= maxLength ? value : value[..maxLength];
}
