using System.Reflection;
using System.Collections;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Reads loosely typed values as collections. A bound property may be an array, a list, or a dictionary, and a rule needs the same answer
/// from all of them without knowing which it holds.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationCollection
{
    /// <summary>
    /// Checks whether a value is a sequence, excluding strings, which are enumerable but which no rule means to treat as a collection.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns><c>true</c> when the value is array-like; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsArrayLike(object? value) => value is IEnumerable and not string;

    /// <summary>
    /// Checks whether a value is an ordered collection rather than merely enumerable, which is what the list rule distinguishes.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns><c>true</c> when the value is list-like; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsListLike(object? value) => value is Array or IList || ImplementsGenericList(value);

    /// <summary>
    /// Reads a sequence into a materialized list, so a rule that must inspect it more than once does not enumerate a lazy source twice.
    /// </summary>
    ///
    /// <param name="value">The bound value to convert, which may already be the target type or may be text that has to be parsed.</param>
    /// <param name="values">Receives the values read from the source, empty when none could be.</param>
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
    /// Reads the keys of a dictionary-like value, for the rules that constrain which keys a payload carries.
    /// </summary>
    ///
    /// <param name="value">The bound value to measure, accepted as text or as any numeric type.</param>
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
    /// Reads keys from a generic dictionary, which the non-generic interface cannot expose without knowing the key type, so the interface
    /// is found by reflection instead.
    /// </summary>
    ///
    /// <param name="value">The bound value to measure, accepted as text or as any numeric type.</param>
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

        return dictionaryKeys is not IEnumerable enumerableKeys ? Fail(out keys) : SetKeys(enumerableKeys, out keys);
    }

    /// <summary>
    /// Renders keys as a comparable set of strings, so a numeric key and its written form match the way a payload would spell them.
    /// </summary>
    ///
    /// <param name="values">The keys the payload must carry, matched against whatever the dictionary-like value holds.</param>
    /// <param name="keys">The resolved key set.</param>
    ///
    /// <returns><c>true</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool SetKeys(IEnumerable values, out IReadOnlySet<string> keys)
    {
        keys = values.Cast<object?>().Select(ValidationValue.ToDisplayValue).ToHashSet(StringComparer.Ordinal);

        return true;
    }

    /// <summary>
    /// Clears the output and reports failure in one expression, keeping the try-pattern methods above expression-bodied.
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
    /// Checks for a generic list interface on the runtime type, since a property declared as an object hides it from the compiler.
    /// </summary>
    ///
    /// <param name="value">The bound value to measure, accepted as text or as any numeric type.</param>
    ///
    /// <returns><c>true</c> when a generic list interface exists; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool ImplementsGenericList(object? value)
        => value?.GetType()
            .GetInterfaces()
            .Where(type => type.IsGenericType)
            .Select(type => type.GetGenericTypeDefinition())
            .Any(type => type == typeof(IList<>) || type == typeof(IReadOnlyList<>)) == true;
}
