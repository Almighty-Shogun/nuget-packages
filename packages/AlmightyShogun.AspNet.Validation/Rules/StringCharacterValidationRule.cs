namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates string character constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class StringCharacterValidationRule<TRequest, TProperty>(StringCharacterMode mode) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationValue.TryGetText(value, out string text))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey()));

        bool isValid = mode switch
        {
            StringCharacterMode.Alpha => text.All(char.IsLetter),
            StringCharacterMode.AlphaNumeric => text.All(char.IsLetterOrDigit),
            StringCharacterMode.AlphaDash => text.All(IsAlphaDashCharacter),
            StringCharacterMode.Ascii => ValidationValue.IsAscii(text),
            StringCharacterMode.Lowercase => text.All(IsLowercaseCharacter),
            StringCharacterMode.Uppercase => text.All(IsUppercaseCharacter),
            _ => false
        };

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Gets the validation message key for the configured string character mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => mode switch
    {
        StringCharacterMode.Alpha => "validation.alpha",
        StringCharacterMode.AlphaNumeric => "validation.alpha.num",
        StringCharacterMode.AlphaDash => "validation.alpha.dash",
        StringCharacterMode.Ascii => "validation.ascii",
        StringCharacterMode.Lowercase => "validation.lowercase",
        _ => "validation.uppercase"
    };

    /// <summary>
    /// Checks whether a character is allowed by the alpha-dash rule.
    /// </summary>
    ///
    /// <param name="character">The character to check.</param>
    ///
    /// <returns><c>true</c> when the character is allowed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAlphaDashCharacter(char character)
        => char.IsLetterOrDigit(character) || character is '-' or '_';

    /// <summary>
    /// Checks whether a character is lowercase when converted invariantly.
    /// </summary>
    ///
    /// <param name="character">The character to check.</param>
    ///
    /// <returns><c>true</c> when the character is lowercase; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsLowercaseCharacter(char character)
        => character == char.ToLowerInvariant(character);

    /// <summary>
    /// Checks whether a character is uppercase when converted invariantly.
    /// </summary>
    ///
    /// <param name="character">The character to check.</param>
    ///
    /// <returns><c>true</c> when the character is uppercase; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsUppercaseCharacter(char character)
        => character == char.ToUpperInvariant(character);
}
