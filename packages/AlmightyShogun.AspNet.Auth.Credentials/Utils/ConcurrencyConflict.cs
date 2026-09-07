using System.Reflection;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Recognises the failures that mean another request wrote the same rows first, so the operation that lost can be run
/// again instead of escaping unhandled. One collision reaches a caller in three shapes: a save's statement matches no row,
/// because the row moved under a mapped concurrency token or is gone altogether, which Entity Framework reports as
/// <see cref="DbUpdateConcurrencyException"/>; a server enforcing snapshot isolation rejects a save's statement and
/// Entity Framework wraps the provider's exception in a plain <see cref="DbUpdateException"/>; and the same rejection of
/// a statement Entity Framework runs against the database directly, an update or a commit, arrives as the provider's own
/// exception with nothing wrapped around it.
/// </summary>
///
/// <remarks>
/// Nothing here references a provider package. The provider's exception is reached as <see cref="DbException"/>, which
/// is part of the framework, and the one value that has to come from a provider type is read by name.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ConcurrencyConflict
{
    /// <summary>
    /// The error a MySQL or MariaDB server with <c>innodb_snapshot_isolation</c> on raises when a statement would write
    /// a row that moved since the transaction first read it, reported as "Record has changed since last read".
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const int _recordChangedErrorNumber = 1020;

    /// <summary>
    /// Reports whether a failed write is one another request won, which is the only kind this package runs again.
    /// </summary>
    ///
    /// <param name="exception">The exception a save, an update, or a commit threw.</param>
    ///
    /// <returns>
    /// <c>true</c> for a <see cref="DbUpdateConcurrencyException"/>, and for any exception whose chain holds a
    /// <see cref="DbException"/> the server raised for a conflict, the exception itself included so a provider's own
    /// exception is recognised when nothing wrapped it. <c>false</c> for everything else, so a constraint violation such
    /// as the unique index on an email address is left to surface as itself either way.
    /// </returns>
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
    /// Reports whether the exception a provider raised says this transaction lost a race rather than that the statement
    /// was wrong.
    /// </summary>
    ///
    /// <param name="exception">The provider's own exception, reached through the inner exceptions of a save failure.</param>
    ///
    /// <returns>
    /// <c>true</c> for SQLSTATE <c>40001</c>, a serialization failure, and <c>40P01</c>, a deadlock the server broke by
    /// choosing a victim, and for <see cref="_recordChangedErrorNumber"/> on a MySQL or MariaDB provider, which is
    /// matched by its number rather than by a SQLSTATE.
    /// </returns>
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
    /// Reports whether an exception came from a MySQL or MariaDB provider, judged by the namespace its type sits in.
    /// Both the connector this package was seen to fail under and the older client put their exception type under a
    /// namespace beginning <c>MySql</c>.
    /// </summary>
    ///
    /// <param name="exception">The provider's own exception.</param>
    ///
    /// <returns><c>true</c> when the type's namespace begins with <c>MySql</c>, <c>false</c> when it is anything else or absent.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsMySqlProvider(DbException exception)
        => exception.GetType().Namespace?.StartsWith("MySql", StringComparison.Ordinal) is true;

    /// <summary>
    /// Reads the server error number off a provider exception without naming the provider's type, by looking for a
    /// <c>Number</c> or <c>ErrorCode</c> property declared below <see cref="DbException"/>.
    /// </summary>
    ///
    /// <param name="exception">The provider's own exception.</param>
    ///
    /// <returns>
    /// The number, taken from an <see cref="int"/> property or from an enumeration one converted to its underlying
    /// value, or <c>null</c> when no such property is declared on the type or any of its bases short of
    /// <see cref="DbException"/>.
    /// </returns>
    ///
    /// <remarks>
    /// The walk stops at <see cref="DbException"/> on purpose. The <c>ErrorCode</c> it inherits from
    /// <see cref="System.Runtime.InteropServices.ExternalException"/> is the HRESULT rather than the server's number, so
    /// reading that one would compare the wrong value.
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
    /// Reads one public instance property declared on a single type, ignoring anything a base declares under the same
    /// name so a property hiding another is read as the more derived type meant it.
    /// </summary>
    ///
    /// <param name="type">The one type in the hierarchy the property has to be declared on.</param>
    /// <param name="exception">The instance to read from.</param>
    /// <param name="name">The property name to look for.</param>
    ///
    /// <returns>The value, or <c>null</c> when that type declares no such property.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static object? ReadProperty(Type type, DbException exception, string name)
        => type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)?.GetValue(exception);
}
