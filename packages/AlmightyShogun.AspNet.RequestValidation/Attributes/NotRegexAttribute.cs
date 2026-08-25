using System.Reflection;
using System.Text.RegularExpressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to not match the regular expression pattern. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="pattern">The regular expression pattern the value must not match.</param>
/// <param name="options">The options to build with, passed through untouched so the caller decides whether to compile.</param>
/// <param name="description">An optional description of the expected shape, passed to the message template as <c>{0}</c>.</param>
/// <param name="matchTimeoutSeconds">The maximum seconds a single match may take before it is abandoned and the rule fails.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NotRegexAttribute(
    string pattern,
    RegexOptions options = RegexOptions.None,
    string? description = null,
    double matchTimeoutSeconds = 1
) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new RegexValidationRule<TRequest, TProperty>(
            pattern,
            options,
            false,
            description,
            TimeSpan.FromSeconds(matchTimeoutSeconds)
        );
}
