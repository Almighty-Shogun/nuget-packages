using System.Reflection;
using AlmightyShogun.Utils;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Maps each request type to the validator that declares its rules, found by scanning at startup so a validator needs no registration of
/// its own.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidatorRegistry
{
    /// <summary>
    /// The validator type for each request type that has one, resolved once at startup rather than searched for per request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Dictionary<Type, Type> _validatorTypes = [];

    /// <summary>
    /// Scans for validators and records which request each one covers.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan, in the order they should be searched.</param>
    ///
    /// <exception cref="InvalidOperationException">
    /// Two validators cover the same request type, or a validator has no public parameterless constructor. Both are raised at startup so
    /// the offending class is named then, rather than on whichever request happens to reach it first.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public ValidatorRegistry(Assembly[] assemblies)
    {
        foreach (Type validatorType in TypeDiscovery.FindAssignableTypes<object>(assemblies))
        {
            if (GetRequestType(validatorType) is not { } requestType) continue;

            if (_validatorTypes.TryGetValue(requestType, out Type? existingType))
                throw new InvalidOperationException(
                    $"'{requestType.Name}' is validated by both '{existingType.Name}' and '{validatorType.Name}'. "
                    + "A request type may have only one validator."
                );

            if (validatorType.GetConstructor(Type.EmptyTypes) is null)
                throw new InvalidOperationException(
                    $"'{validatorType.Name}' has no public parameterless constructor. A validator declares rules once, outside any "
                    + "request scope, so it cannot take dependencies."
                );

            _validatorTypes[requestType] = validatorType;
        }
    }

    /// <summary>
    /// Reports whether a request type has a validator, so a request with neither a validator nor an attribute skips rule building.
    /// </summary>
    ///
    /// <param name="requestType">The request type.</param>
    ///
    /// <returns><c>true</c> when a validator covers the type; otherwise, <c>false</c>.</returns>
    ///
    /// <remarks>
    /// The match is exact rather than by assignability. A validator for a base request does not cover a derived one, since the rules it
    /// declares are expressions over the base and could not read a property the derived type adds.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool HasValidator(Type requestType) => _validatorTypes.ContainsKey(requestType);

    /// <summary>
    /// Builds the rules a request type's validator declares, or none when it has no validator.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type whose validator is wanted.</typeparam>
    ///
    /// <returns>The declared rules in declaration order, or an empty set when the type has no validator.</returns>
    ///
    /// <remarks>
    /// A fresh validator is constructed for each call and discarded after, since the rules it produced are what gets cached rather than
    /// the validator itself. Only the rule cache calls this, and only when it has no entry for the type, so it does not run per request.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<IRequestValidationRule<TRequest>> BuildRules<TRequest>() where TRequest : class
    {
        if (!_validatorTypes.TryGetValue(typeof(TRequest), out Type? validatorType))
            return [];

        Validator<TRequest> validator = (Validator<TRequest>)Activator.CreateInstance(validatorType)!;

        return validator.BuildRules();
    }

    /// <summary>
    /// Reads the request type a validator covers by walking its base types for the generic base.
    /// </summary>
    ///
    /// <param name="type">One concrete type found by the scan, which is usually not a validator at all.</param>
    ///
    /// <returns>The request type its <see cref="Validator{TRequest}"/> base names, or <c>null</c> when it has no such base.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static Type? GetRequestType(Type type)
    {
        for (Type? baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Validator<>))
                return baseType.GetGenericArguments()[0];

        return null;
    }
}
