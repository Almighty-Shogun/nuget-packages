using System.Text.Json;
using System.Text.RegularExpressions;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates format constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed partial class FormatValidationRule<TRequest, TProperty>(FormatMode mode) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        bool isValid = mode switch
        {
            FormatMode.Email => IsEmail(value),
            FormatMode.Url => IsUrl(value),
            FormatMode.Json => IsJson(value),
            FormatMode.Uuid => IsUuid(value),
            FormatMode.Ulid => IsUlid(value),
            FormatMode.HexColor => IsHexColor(value),
            FormatMode.MacAddress => IsMacAddress(value),
            _ => false
        };

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Checks whether a value is a valid email address.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is a valid email address; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsEmail(object? value) => value switch
    {
        string typed => ValidationFormat.IsEmail(typed),
        _ => false
    };

    /// <summary>
    /// Checks whether a value is a valid HTTP or HTTPS URL.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is a valid URL; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsUrl(object? value) => value switch
    {
        string typed => IsUrlText(typed),
        _ => false
    };

    /// <summary>
    /// Checks whether text is a valid HTTP or HTTPS URL.
    /// </summary>
    ///
    /// <param name="value">The text value to check.</param>
    ///
    /// <returns><c>true</c> when the text is a valid URL; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsUrlText(string value)
        => Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) && IsHttpUri(uri);

    /// <summary>
    /// Checks whether a value is valid JSON text.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is valid JSON; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsJson(object? value) => value switch
    {
        string typed => IsJsonText(typed),
        _ => false
    };

    /// <summary>
    /// Checks whether text is valid JSON.
    /// </summary>
    ///
    /// <param name="value">The text value to check.</param>
    ///
    /// <returns><c>true</c> when the text is valid JSON; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsJsonText(string value)
    {
        try
        {
            using var document = JsonDocument.Parse(value);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Checks whether a value is a valid UUID.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is a valid UUID; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsUuid(object? value) => value switch
    {
        string typed => Guid.TryParse(typed, out _),
        _ => false
    };

    /// <summary>
    /// Checks whether a value is a valid ULID.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is a valid ULID; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsUlid(object? value) => value switch
    {
        string typed => UlidRegex().IsMatch(typed),
        _ => false
    };

    /// <summary>
    /// Checks whether a value is a valid hex color.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is a valid hex color; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsHexColor(object? value) => value switch
    {
        string typed => HexColorRegex().IsMatch(typed),
        _ => false
    };

    /// <summary>
    /// Checks whether a value is a valid MAC address.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is a valid MAC address; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsMacAddress(object? value) => value switch
    {
        string typed => MacAddressRegex().IsMatch(typed),
        _ => false
    };

    /// <summary>
    /// Checks whether a URI uses HTTP or HTTPS.
    /// </summary>
    ///
    /// <param name="uri">The URI to check.</param>
    ///
    /// <returns><c>true</c> when the URI uses HTTP or HTTPS; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsHttpUri(Uri uri)
        => uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;

    /// <summary>
    /// Gets the validation message key for the configured format mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => mode switch
    {
        FormatMode.Email => "validation.email",
        FormatMode.Url => "validation.url",
        FormatMode.Json => "validation.json",
        FormatMode.Uuid => "validation.uuid",
        FormatMode.Ulid => "validation.ulid",
        FormatMode.HexColor => "validation.hex-color",
        _ => "validation.mac-address"
    };

    /// <summary>
    /// Gets the regular expression used to validate ULIDs.
    /// </summary>
    ///
    /// <returns>The ULID regular expression.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [GeneratedRegex("^[0-7][0-9A-HJKMNP-TV-Z]{25}$", RegexOptions.IgnoreCase)]
    private static partial Regex UlidRegex();

    /// <summary>
    /// Gets the regular expression used to validate hex colors.
    /// </summary>
    ///
    /// <returns>The hex color regular expression.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [GeneratedRegex("^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{4}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$")]
    private static partial Regex HexColorRegex();

    /// <summary>
    /// Gets the regular expression used to validate MAC addresses.
    /// </summary>
    ///
    /// <returns>The MAC address regular expression.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [GeneratedRegex("^(?:[0-9a-fA-F]{2}[:-]){5}[0-9a-fA-F]{2}$|^[0-9a-fA-F]{12}$")]
    private static partial Regex MacAddressRegex();
}
