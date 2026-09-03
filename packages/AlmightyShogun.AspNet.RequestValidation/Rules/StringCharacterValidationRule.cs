namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Restricts text to a character class, or requires it to already be in a given case.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class StringCharacterValidationRule<TRequest, TProperty>(
    StringCharacterMode mode
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
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
            StringCharacterMode.Lowercase => IsLowercase(text),
            StringCharacterMode.Uppercase => IsUppercase(text),
            _ => throw new InvalidOperationException($"Unsupported StringCharacterMode value '{mode}'.")
        };

        return ValueTask.FromResult(isValid ? ValidationRuleResult.Success() : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
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
        StringCharacterMode.Uppercase => "validation.uppercase",
        _ => throw new InvalidOperationException($"Unsupported StringCharacterMode value '{mode}'.")
    };

    /// <summary>
    /// Checks whether a character is allowed by the alpha-dash rule.
    /// </summary>
    ///
    /// <param name="character">One character of the value, tested on its own rather than through a culture-aware string comparison.</param>
    ///
    /// <returns><c>true</c> when the character is allowed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAlphaDashCharacter(char character) => char.IsLetterOrDigit(character) || character is '-' or '_';

    /// <summary>
    /// Checks that text is lowercase, which takes both that nothing in it is uppercase and that something in it has a case at all.
    /// </summary>
    ///
    /// <param name="text">The value read as text.</param>
    ///
    /// <returns>
    /// <c>true</c> when every cased character is lowercase and at least one character is cased; otherwise, <c>false</c> . Digits and
    /// punctuation neither pass nor fail it on their own, so <c>"abc-1"</c> passes while <c>"123"</c> does not.
    /// </returns>
    ///
    /// <remarks>
    /// The presence of a cased character is required deliberately. Testing only that lowercasing changes nothing lets a value with no
    /// letters in it satisfy both this rule and the uppercase one, which is not what either rule's name promises.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsLowercase(string text) => text.Any(IsCasedCharacter) && !text.Any(char.IsUpper);

    /// <summary>
    /// Checks that text is uppercase, on the same terms as <see cref="IsLowercase"/> .
    /// </summary>
    ///
    /// <param name="text">The value read as text.</param>
    ///
    /// <returns>
    /// <c>true</c> when every cased character is uppercase and at least one character is cased; otherwise, <c>false</c> .
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsUppercase(string text) => text.Any(IsCasedCharacter) && !text.Any(char.IsLower);

    /// <summary>
    /// Reports whether a character has a case to be in, which a digit, a symbol, and a letter from a caseless script do not.
    /// </summary>
    ///
    /// <param name="character">One character of the value.</param>
    ///
    /// <returns><c>true</c> when the character's upper and lower forms differ; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsCasedCharacter(char character) => char.ToLowerInvariant(character) != char.ToUpperInvariant(character);
}
