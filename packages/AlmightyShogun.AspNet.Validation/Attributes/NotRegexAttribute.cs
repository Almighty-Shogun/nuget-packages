using System.Reflection;
using System.Text.RegularExpressions;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the inverse regular expression validation rule for a request property.
/// </summary>
///
/// <param name="pattern">The regular expression pattern the value must not match.</param>
/// <param name="options">The regular expression options.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NotRegexAttribute(string pattern, RegexOptions options = RegexOptions.None) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new RegexValidationRule<TRequest, TProperty>(pattern, options, false);
}
