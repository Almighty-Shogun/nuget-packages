using System.Reflection;

namespace AlmightyShogun.Core;

/// <summary>
/// Finds concrete implementations across assemblies by reflection, which is what the registration helpers in this package
/// are built on. It is a raw primitive with no dependency-injection semantics: nothing here reads
/// <see cref="SkipAutoRegistrationAttribute"/> or decides a lifetime.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class TypeDiscovery
{
    /// <summary>
    /// Retrieves the concrete types in the calling assembly that inherit from or implement <typeparamref name="T"/>.
    /// Use it from the assembly that owns the implementations; scanning a different one needs an explicit overload.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// The base type or interface to match. Assignability is used, so an indirect subclass or an interface implemented by a
    /// base class matches just as well as a direct one.
    /// </typeparam>
    ///
    /// <returns>
    /// The matching concrete types in the caller's own assembly, lazily. Empty when the assembly declares none.
    /// </returns>
    ///
    /// <remarks>
    /// The assembly is resolved from the call stack, so this reports whichever assembly contains the code that called it,
    /// not the one that started the process.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IEnumerable<Type> FindAssignableTypes<T>() => FindAssignableTypes<T>(Assembly.GetCallingAssembly());

    /// <summary>
    /// Retrieves the concrete types in one assembly that inherit from or implement <typeparamref name="T"/>. Reach for it
    /// when the implementations live somewhere other than the calling assembly, such as a separate contracts project.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// The base type or interface to match. Assignability is used, so an indirect subclass or an interface implemented by a
    /// base class matches just as well as a direct one.
    /// </typeparam>
    /// <param name="assembly">
    /// The assembly to scan. Passed through unchanged, so an assembly whose types cannot all be loaded still contributes
    /// the ones that did.
    /// </param>
    ///
    /// <returns>
    /// The matching concrete types in that assembly, lazily. Empty when it declares none.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IEnumerable<Type> FindAssignableTypes<T>(Assembly assembly) => FindAssignableTypes<T>([assembly]);
    
    /// <summary>
    /// Retrieves the concrete types in the specified assemblies that inherit from or implement <typeparamref name="T"/>.
    /// Interfaces and abstract classes are excluded, so every returned type can be instantiated.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// The base type or interface to match. Assignability is used, so an indirect subclass or an interface implemented by a
    /// base class matches just as well as a direct one.
    /// </typeparam>
    /// <param name="assemblies">
    /// The assemblies to scan, in the order they should be searched. An empty array yields nothing rather than falling
    /// back to the calling assembly; the parameterless overload is what does that.
    /// </param>
    ///
    /// <returns>
    /// The matching concrete types, in assembly then declaration order. The sequence is lazy, so the reflection work happens
    /// as it is enumerated rather than when this method returns.
    /// </returns>
    ///
    /// <remarks>
    /// This is a raw reflection primitive with no dependency-injection semantics. It does not honor
    /// <see cref="SkipAutoRegistrationAttribute"/>; only the registration helpers do. An assembly whose types cannot all be
    /// loaded contributes the types that did load, so one unresolvable dependency does not end the scan.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static IEnumerable<Type> FindAssignableTypes<T>(Assembly[] assemblies) => assemblies
        .SelectMany(SafeGetTypes)
        .Where(t => typeof(T).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

    /// <summary>
    /// Reads the types defined in an assembly, keeping the ones that loaded when others could not.
    /// </summary>
    ///
    /// <param name="assembly">The assembly to read the defined types from.</param>
    ///
    /// <returns>
    /// Every type the assembly defines, or the subset that loaded when a dependency could not be resolved.
    /// </returns>
    ///
    /// <remarks>
    /// A partially loadable assembly throws <see cref="ReflectionTypeLoadException"/> from
    /// <see cref="Assembly.GetTypes"/> while still exposing the types that succeeded on the exception itself. Discovery is a
    /// best-effort scan, so the usable subset is preferable to failing the whole call over one missing dependency.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
