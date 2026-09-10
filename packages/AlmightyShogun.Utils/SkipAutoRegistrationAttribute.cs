using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Utils;

/// <summary>
/// Excludes a class from the registration performed by
/// <see cref="ServiceCollectionExtensions.RegisterOnInherit{T}(IServiceCollection, Assembly[], ServiceLifetime, bool, Func{Type, bool})"/>
/// and <see cref="ServiceCollectionExtensions.RegisterOnInherit{T}(IServiceCollection, ServiceLifetime)"/>.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SkipAutoRegistrationAttribute : Attribute;
