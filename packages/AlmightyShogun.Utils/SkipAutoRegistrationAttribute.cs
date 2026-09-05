using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Utils;

/// <summary>
/// Excludes a type from the registration performed by
/// <see cref="ServiceCollectionExtensions.RegisterOnInherit{T}(IServiceCollection, Assembly[], ServiceLifetime, bool, Func{Type, bool})"/>
/// and so from <see cref="ServiceCollectionExtensions.RegisterOnInherit{T}(IServiceCollection, ServiceLifetime)"/>, which
/// delegates to it. The type is still scanned and still discovered; only the registration step drops it, and only where the
/// attribute sits on the type itself rather than on a base it derives from. Apply it to a concrete type that would otherwise
/// be discovered but needs registering by hand, with a factory or a non-default lifetime, or that should not be registered at
/// all.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SkipAutoRegistrationAttribute : Attribute;
