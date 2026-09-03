using System.Reflection;
using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Reads the property a rule expression points at. Only a property read directly off the request is accepted, so a rule cannot be declared
/// against something the pipeline is unable to name or safely read.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationExpression
{
    /// <summary>
    /// Reads the property an expression points at, rejecting anything that is not a direct read off the lambda's own parameter.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type the expression reads from.</typeparam>
    /// <typeparam name="TProperty">
    /// The property type the expression yields, which is <see cref="object"/> for the untyped spellings the attribute path uses.
    /// </typeparam>
    /// <param name="expression">
    /// Points at the property, supplying both the name failures are reported under and the reader used to fetch its value. A conversion
    /// wrapped around the read, which the compiler inserts when a value-typed property is read as <see cref="object"/> , is unwrapped
    /// first.
    /// </param>
    ///
    /// <returns>The property the expression reads.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// The expression is not a direct property read: a method call, a literal, a field, or a nested read such as
    /// <c>request => request.User.Email</c> . A nested read is refused rather than supported, because the name derived from it would be the
    /// leaf property's alone and the compiled reader would throw whenever an intermediate value is null. Thrown as the rule is built rather
    /// than when a request arrives.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static PropertyInfo GetProperty<TRequest, TProperty>(Expression<Func<TRequest, TProperty>> expression)
    {
        Expression body = expression.Body is UnaryExpression { Operand: var operand } ? operand : expression.Body;

        if (body is MemberExpression { Member: PropertyInfo property, Expression: ParameterExpression parameter }
            && parameter == expression.Parameters[0])
            return property;

        throw new ArgumentOutOfRangeException(
            nameof(expression),
            $"Validation rules only support a property read directly off the request, such as request => request.Email. "
            + $"'{expression.Body}' is not one."
        );
    }

    /// <summary>
    /// Reads the field name an expression's property is reported under, which is the pairing every fluent rule needs.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type the expression reads from.</typeparam>
    /// <typeparam name="TProperty">The property type the expression yields.</typeparam>
    /// <param name="expression">Points at the property whose public field name is wanted.</param>
    ///
    /// <returns>The field name, honoring an explicit serialization name over the declared one.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// The expression is not a direct property read, on the same terms as
    /// <see cref="GetProperty{TRequest, TProperty}(Expression{Func{TRequest, TProperty}})"/> .
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string GetFieldName<TRequest, TProperty>(Expression<Func<TRequest, TProperty>> expression)
        => ValidationFieldName.FromProperty(GetProperty(expression));
}
