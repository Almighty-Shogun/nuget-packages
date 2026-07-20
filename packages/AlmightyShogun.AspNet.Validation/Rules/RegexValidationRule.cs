using System.Text.RegularExpressions;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates regular expression constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class RegexValidationRule<TRequest, TProperty>(string pattern, RegexOptions options, bool shouldMatch) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationValue.TryGetText(value, out string text))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey()));

        bool isMatch = Regex.IsMatch(text, pattern, options);

        return ValueTask.FromResult(isMatch == shouldMatch
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Gets the validation message key for the configured regular expression match mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => shouldMatch ? "validation.regex" : "validation.not.regex";
}
