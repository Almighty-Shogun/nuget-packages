using System.Reflection;
using System.Runtime.CompilerServices;

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
    /// Finds concrete types in the specified assembly that are assignable to <typeparamref name="T"/>.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// The type to which discovered types must be assignable.
    /// </typeparam>
    /// <param name="assembly">
    /// The assembly to scan. If <c>null</c>, the calling assembly is used.
    /// </param>
    ///
    /// <returns>
    /// The discovered types.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IEnumerable<Type> FindAssignableTypes<T>(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        
        return FindAssignableTypes<T>([assembly]);
    }

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
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static IEnumerable<Type> FindAssignableTypes<T>(Assembly[] assemblies) => assemblies
        .SelectMany(static assembly => assembly.GetTypes())
        .Where(t => typeof(T).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });
}
