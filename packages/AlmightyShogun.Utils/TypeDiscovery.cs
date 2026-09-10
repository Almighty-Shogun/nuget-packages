using System.Reflection;

namespace AlmightyShogun.Utils;

/// <summary>
/// Provides utilities for discovering assignable types in assemblies.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class TypeDiscovery
{
    /// <summary>
    /// Finds concrete types in the calling assembly that are assignable to <typeparamref name="T"/>.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// The type to which discovered types must be assignable.
    /// </typeparam>
    ///
    /// <returns>
    /// The discovered types.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public static IEnumerable<Type> FindAssignableTypes<T>() => FindAssignableTypes<T>(Assembly.GetCallingAssembly());

    /// <summary>
    /// Finds concrete types in the specified assembly that are assignable to <typeparamref name="T"/>.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// The type to which discovered types must be assignable.
    /// </typeparam>
    /// <param name="assembly">
    /// The assembly to scan.
    /// </param>
    ///
    /// <returns>
    /// The discovered types.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public static IEnumerable<Type> FindAssignableTypes<T>(Assembly assembly) => FindAssignableTypes<T>([assembly]);
    
    /// <summary>
    /// Finds concrete types in the specified assemblies that are assignable to <typeparamref name="T"/>.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// The type to which discovered types must be assignable.
    /// </typeparam>
    /// <param name="assemblies">
    /// The assemblies to scan.
    /// </param>
    ///
    /// <returns>
    /// The discovered types.
    /// </returns>
    ///
    /// <remarks>
    /// Types that cannot be loaded from assemblies are skipped.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static IEnumerable<Type> FindAssignableTypes<T>(Assembly[] assemblies) => assemblies
        .SelectMany(SafeGetTypes)
        .Where(t => typeof(T).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

    /// <summary>
    /// Gets the types that can be loaded from the specified assembly.
    /// </summary>
    ///
    /// <param name="assembly">The assembly to inspect.</param>
    ///
    /// <returns>
    /// The types that were successfully loaded.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type is not null)!;
        }
    }
}
