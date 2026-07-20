using System.Reflection;
using System.Collections;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Provides collection helpers used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationCollection
{
    /// <summary>
    /// Checks whether a value is enumerable and not a string.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is array-like; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsArrayLike(object? value) => value is IEnumerable and not string;

    /// <summary>
    /// Checks whether a value is list-like.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is list-like; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsListLike(object? value) => value is Array or IList || ImplementsGenericList(value);

    /// <summary>
    /// Attempts to read a value as an object list.
    /// </summary>
    ///
    /// <param name="value">The value to read.</param>
    /// <param name="values">The resolved values.</param>
    ///
    /// <returns><c>true</c> when the value can be enumerated; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetValues(object? value, out IReadOnlyList<object?> values)
    {
        (bool isValid, IReadOnlyList<object?> resolvedValues) = value switch
        {
            string => (false, []),
            IEnumerable typed => (true, typed.Cast<object?>().ToArray()),
            _ => (false, [])
        };

        values = resolvedValues;

        return isValid;
    }

    /// <summary>
    /// Attempts to read dictionary keys from a value.
    /// </summary>
    ///
    /// <param name="value">The value to inspect.</param>
    /// <param name="keys">The resolved keys.</param>
    ///
    /// <returns><c>true</c> when keys can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetKeys(object? value, out IReadOnlySet<string> keys) => value switch
    {
        IDictionary typed => SetKeys(typed.Keys, out keys),
        _ => TryGetGenericDictionaryKeys(value, out keys)
    };

    /// <summary>
    /// Attempts to read keys from a generic dictionary implementation.
    /// </summary>
    ///
    /// <param name="value">The value to inspect.</param>
    /// <param name="keys">The resolved keys.</param>
    ///
    /// <returns><c>true</c> when generic dictionary keys can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetGenericDictionaryKeys(object? value, out IReadOnlySet<string> keys)
    {
        Type? dictionaryInterface = value?.GetType()
            .GetInterfaces()
            .FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>));

        if (dictionaryInterface is null)
            return Fail(out keys);

        PropertyInfo? keysProperty = dictionaryInterface.GetProperty("Keys");
        object? dictionaryKeys = keysProperty?.GetValue(value);

        return dictionaryKeys is not IEnumerable enumerableKeys
            ? Fail(out keys)
            : SetKeys(enumerableKeys, out keys);
    }

    /// <summary>
    /// Converts enumerable key values into a string key set.
    /// </summary>
    ///
    /// <param name="values">The key values.</param>
    /// <param name="keys">The resolved key set.</param>
    ///
    /// <returns><c>true</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool SetKeys(IEnumerable values, out IReadOnlySet<string> keys)
    {
        keys = values
            .Cast<object?>()
            .Select(ValidationValue.ToDisplayValue)
            .ToHashSet(StringComparer.Ordinal);

        return true;
    }

    /// <summary>
    /// Sets the key output to an empty set and returns failure.
    /// </summary>
    ///
    /// <param name="keys">The output keys.</param>
    ///
    /// <returns><c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool Fail(out IReadOnlySet<string> keys)
    {
        keys = new HashSet<string>(StringComparer.Ordinal);
        return false;
    }

    /// <summary>
    /// Checks whether a value implements a generic list interface.
    /// </summary>
    ///
    /// <param name="value">The value to inspect.</param>
    ///
    /// <returns><c>true</c> when a generic list interface exists; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool ImplementsGenericList(object? value) => value?.GetType()
        .GetInterfaces()
        .Where(type => type.IsGenericType)
        .Select(type => type.GetGenericTypeDefinition())
        .Any(type => type == typeof(IList<>) || type == typeof(IReadOnlyList<>)) == true;
}
