namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates password constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class PasswordValidationRule<TRequest, TProperty>(PasswordRequirement requirement) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationValue.TryGetText(value, out string text))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey()));

        bool isValid = IsValid(text, requirement);

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Checks whether password text satisfies a password requirement.
    /// </summary>
    ///
    /// <param name="text">The password text.</param>
    /// <param name="requirement">The password requirement.</param>
    ///
    /// <returns><c>true</c> when the password satisfies the requirement; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsValid(string text, PasswordRequirement requirement) => requirement switch
    {
        PasswordRequirement.Letters => HasLetters(text),
        PasswordRequirement.Mixed => HasMixedCase(text),
        PasswordRequirement.Numbers => HasNumbers(text),
        PasswordRequirement.Symbols => HasSymbols(text),
        PasswordRequirement.Secure => IsSecure(text),
        _ => false
    };

    /// <summary>
    /// Gets the validation message key for the configured password requirement.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => requirement switch
    {
        PasswordRequirement.Letters => "validation.password.letters",
        PasswordRequirement.Mixed => "validation.password.mixed",
        PasswordRequirement.Numbers => "validation.password.numbers",
        PasswordRequirement.Symbols => "validation.password.symbols",
        _ => "validation.password.secure"
    };

    /// <summary>
    /// Checks whether password text satisfies every built-in password requirement.
    /// </summary>
    ///
    /// <param name="text">The password text.</param>
    ///
    /// <returns><c>true</c> when the password is secure; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsSecure(string text)
        => HasLetters(text) && HasMixedCase(text) && HasNumbers(text) && HasSymbols(text);

    /// <summary>
    /// Checks whether password text contains at least one letter.
    /// </summary>
    ///
    /// <param name="text">The password text.</param>
    ///
    /// <returns><c>true</c> when the password contains a letter; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasLetters(string text) => text.Any(char.IsLetter);

    /// <summary>
    /// Checks whether password text contains both lowercase and uppercase letters.
    /// </summary>
    ///
    /// <param name="text">The password text.</param>
    ///
    /// <returns><c>true</c> when the password has mixed case; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasMixedCase(string text) => text.Any(char.IsLower) && text.Any(char.IsUpper);

    /// <summary>
    /// Checks whether password text contains at least one number.
    /// </summary>
    ///
    /// <param name="text">The password text.</param>
    ///
    /// <returns><c>true</c> when the password contains a number; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasNumbers(string text) => text.Any(char.IsDigit);

    /// <summary>
    /// Checks whether password text contains at least one symbol.
    /// </summary>
    ///
    /// <param name="text">The password text.</param>
    ///
    /// <returns><c>true</c> when the password contains a symbol; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasSymbols(string text) => text.Any(IsSymbol);

    /// <summary>
    /// Checks whether a character is punctuation or a symbol.
    /// </summary>
    ///
    /// <param name="character">The character to check.</param>
    ///
    /// <returns><c>true</c> when the character is a symbol; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsSymbol(char character) => char.IsPunctuation(character) || char.IsSymbol(character);
}
