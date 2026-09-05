using System.Reflection;
using AlmightyShogun.Utils;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the attribute rules of every request type found in a set of assemblies and discards them, so a rule that cannot be constructed
/// throws where the check runs rather than on the request that first reaches it.
/// </summary>
///
/// <remarks>
/// Nothing in this package calls <see cref="Verify"/>. Rules are built lazily instead, on the first request of each type, by
/// <c>ValidationRuleCache.GetRules</c>; <c>AddAspNetValidation</c> never runs this check.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationRuleVerifier
{
    /// <summary>
    /// Finds the request types in the given assemblies and builds each one's rules, discarding the result. Types that are not classes,
    /// generic type definitions, and types declaring no validation attribute are skipped.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan for types carrying validation attributes.</param>
    ///
    /// <exception cref="InvalidOperationException">
    /// A request type's rules could not be built. The message names the type and the failure that caused it is kept as the inner
    /// exception.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static void Verify(Assembly[] assemblies)
    {
        MethodInfo createRules = typeof(AttributeRuleFactory).GetMethod(nameof(AttributeRuleFactory.CreateRules))!;

        foreach (Type requestType in TypeDiscovery.FindAssignableTypes<object>(assemblies))
        {
            if (!requestType.IsClass || requestType.IsGenericTypeDefinition) continue;

            if (!AttributeRuleFactory.HasRules(requestType)) continue;

            try
            {
                createRules.MakeGenericMethod(requestType).Invoke(null, null);
            }
            catch (TargetInvocationException exception)
            {
                throw new InvalidOperationException(
                    $"The validation rules on '{requestType.Name}' could not be built.",
                    exception.InnerException
                );
            }
        }
    }
}
