namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Fits a request-supplied value to the column it is stored in. The IP address and the User-Agent this package records
/// arrive off a request header, so their length is the caller's to decide rather than the package's, and the columns
/// holding them are fixed width.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
internal static class ColumnValue
{
    /// <summary>
    /// Trims a value to the column length. A value longer than its column otherwise fails the write with a database
    /// error, on the providers that enforce the width: PostgreSQL and SQL Server reject it, SQLite stores it whole.
    /// </summary>
    ///
    /// <param name="value">The value as it arrived, normally straight off a request header.</param>
    /// <param name="maxLength">The column width, which is what the value has to fit.</param>
    ///
    /// <returns>The value, trimmed when it exceeds the column length.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    internal static string? Truncate(string? value, int maxLength)
        => value is null || value.Length <= maxLength ? value : value[..maxLength];
}
