using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Utils;

/// <summary>
/// Excludes a type from the assembly scanning performed by
/// <see cref="ServiceCollectionExtensions.RegisterOnInherit{T}(IServiceCollection, ServiceLifetime, bool, Func{Type, bool}, Assembly[])"/>.
/// Apply it to a concrete type that would otherwise be discovered but needs registering by hand, with a factory or a
/// non-default lifetime, or that should not be registered at all.
/// </summary>
///
/// <remarks>
/// The attribute is read with <c>inherit: false</c>, so it applies only to the type that carries it. A class deriving from a
/// marked base is still discovered and registered, which is deliberate: skipping a base is a statement about that base, not
/// about everything built on it.
///
/// It is honored only by the registration helpers.
/// <see cref="ApplicationUtils.GetOnInherit{T}"/> is a raw reflection primitive with no dependency-injection semantics and
/// returns marked types like any other.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SkipAutoRegistrationAttribute : Attribute;
