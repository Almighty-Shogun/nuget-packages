using System.Reflection;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Detects database concurrency conflicts for retry handling.
/// </summary>
///
/// <remarks>
/// Supports EF Core concurrency exceptions and provider-specific write conflicts
/// without requiring direct provider dependencies.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ConcurrencyConflict
{
    /// <summary>
    /// MySQL/MariaDB error number indicating a record changed since it was read.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const int _recordChangedErrorNumber = 1020;

    /// <summary>
    /// Determines whether an exception represents a database concurrency conflict.
    /// </summary>
    ///
    /// <param name="exception">The exception to inspect.</param>
    ///
    /// <returns>True if a concurrency conflict is detected; otherwise, false.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static bool IsConflict(Exception exception)
    {
        if (exception is DbUpdateConcurrencyException)
            return true;

        for (Exception? inner = exception; inner is not null; inner = inner.InnerException)
            if (inner is DbException database && IsWriteConflict(database))
                return true;

        return false;
    }

    /// <summary>
    /// Checks whether a database exception indicates a serialization failure,
    /// deadlock, or supported provider-specific write conflict.
    /// </summary>
    ///
    /// <param name="exception">The database exception.</param>
    ///
    /// <returns>True if the exception indicates a write conflict; otherwise, false.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsWriteConflict(DbException exception)
    {
        if (exception.SqlState is "40001" or "40P01")
            return true;

        return IsMySqlProvider(exception) && ErrorNumber(exception) == _recordChangedErrorNumber;
    }

    /// <summary>
    /// Determines whether an exception originates from a MySQL or MariaDB provider.
    /// </summary>
    ///
    /// <param name="exception">The database exception.</param>
    ///
    /// <returns>True if the exception type belongs to a MySql namespace; otherwise, false.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsMySqlProvider(DbException exception)
        => exception.GetType().Namespace?.StartsWith("MySql", StringComparison.Ordinal) is true;

    /// <summary>
    /// Reads a provider-specific error number using reflection.
    /// </summary>
    ///
    /// <param name="exception">The database exception.</param>
    ///
    /// <returns>The error number, or null if unavailable.</returns>
    ///
    /// <remarks>
    ///  Excludes inherited DbException properties to avoid reading an HRESULT.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static int? ErrorNumber(DbException exception)
    {
        for (Type? type = exception.GetType(); type is not null && type != typeof(DbException); type = type.BaseType)
        {
            object? value = ReadProperty(type, exception, "Number") ?? ReadProperty(type, exception, "ErrorCode");

            switch (value)
            {
                case int number:
                    return number;
                case Enum code:
                    return Convert.ToInt32(code);
            }
        }

        return null;
    }

    /// <summary>
    /// Reads a public instance property declared directly on a type.
    /// </summary>
    ///
    /// <param name="type">The declaring type.</param>
    /// <param name="exception">The exception instance.</param>
    /// <param name="name">The property name.</param>
    ///
    /// <returns>The property value, or null if unavailable.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static object? ReadProperty(Type type, DbException exception, string name)
        => type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)?.GetValue(exception);
}
