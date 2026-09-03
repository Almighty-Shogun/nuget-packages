using System.Reflection;
using AlmightyShogun.Utils;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds every request type's attribute rules once at startup, so a rule that cannot be constructed is reported while the application is
/// starting rather than on whichever request reaches it first.
/// </summary>
///
/// <remarks>
/// Rules are otherwise built lazily, on the first request of each type. That is the right moment for the work but the wrong moment for the
/// failure: a mistyped field name, an empty set of values, or a custom rule naming an incompatible type would each take down one endpoint
/// long after deployment. Constructing them here turns all of those into a startup fault.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationRuleVerifier
{
    /// <summary>
    /// Finds the request types in the given assemblies and builds each one's rules, discarding the result.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan, the same ones the validators are discovered in.</param>
    ///
    /// <exception cref="InvalidOperationException">
    /// A request type's rules could not be built. The message names the type, since the failure it wraps describes the rule but not where
    /// it was declared.
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
