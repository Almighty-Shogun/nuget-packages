using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Declares the rules for one request type in code, for constraints an attribute cannot express. Write one per request that needs it and
/// leave it in an assembly the registration scans; nothing else is needed to wire it up.
/// </summary>
///
/// <typeparam name="TRequest">
/// The request this validator declares rules for. One validator per request type: a second one for the same request is refused at startup
/// rather than one of them silently winning.
/// </typeparam>
///
/// <remarks>
/// The rules are declared once for the request type and cached for the life of the process, which is why they are declared here rather
/// than on the request. A validator holds no request instance and so cannot branch on one, meaning a requirement that varies per request
/// has to be expressed as a rule that varies, such as <c>RequiredIf</c> , rather than as an <c>if</c> around a rule.
///
/// The type needs a public parameterless constructor and should not take dependencies. It runs once, outside any request scope, so a
/// service captured here would outlive the scope it came from. A rule that genuinely needs services belongs in
/// <see cref="ICustomValidationRule{TRequest, TProperty}"/> , which is resolved per request.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class Validator<TRequest> where TRequest : class
{
    /// <summary>
    /// The rules collected while <see cref="Rules"/> runs, then copied out so later mutation of this list cannot reach the cached set.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly List<IRequestValidationRule<TRequest>> _rules = [];

    /// <summary>
    /// Declares the request's rules, by calling <see cref="RuleFor{TProperty}"/> once per property that needs any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected abstract void Rules();

    /// <summary>
    /// Begins a chain for one property, named by an expression so the field name and the value reader come from the same place.
    /// </summary>
    ///
    /// <typeparam name="TProperty">The property's type, which decides the rules the returned builder offers.</typeparam>
    /// <param name="expression">
    /// Points at the property to validate. It supplies both the field name failures are reported under and the reader used to fetch the
    /// value, so the two can never disagree.
    /// </param>
    ///
    /// <returns>A builder for that property, which the caller chains rules onto.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="expression"/> is not a property read directly off the request, such as a nested read or a method call. Thrown as
    /// the chain is built rather than when a request is validated.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected RuleBuilder<TRequest, TProperty> RuleFor<TProperty>(Expression<Func<TRequest, TProperty>> expression)
    {
        PropertyRule<TRequest, TProperty> propertyRule = new(expression);

        _rules.Add(propertyRule);

        return new RuleBuilder<TRequest, TProperty>(propertyRule);
    }

    /// <summary>
    /// Runs the declaration and hands back what it produced, which is what the rule cache calls on its first miss for the request type.
    /// </summary>
    ///
    /// <returns>The rules this validator declared, in declaration order.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal IReadOnlyList<IRequestValidationRule<TRequest>> BuildRules()
    {
        _rules.Clear();

        Rules();

        return [.. _rules];
    }
}
