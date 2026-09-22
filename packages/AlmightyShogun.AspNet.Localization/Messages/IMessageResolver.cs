namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Resolves localized messages and their language.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IMessageResolver
{
    /// <summary>
    /// Resolves a localized message.
    /// </summary>
    ///
    /// <param name="key">The message key.</param>
    ///
    /// <returns>
    /// The resolved message, or <paramref name="key"/> if no message is defined for it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    string Resolve(string key);

    /// <summary>
    /// Resolves and formats a localized message.
    /// </summary>
    ///
    /// <param name="key">The message key.</param>
    /// <param name="parameters">The values used to format the message.</param>
    ///
    /// <returns>
    /// The formatted message, the unformatted message if formatting fails,
    /// or <paramref name="key"/> if no message is defined for it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    string Resolve(string key, IReadOnlyList<object?> parameters);

    /// <summary>
    /// Resolves the language used for localized messages.
    /// </summary>
    ///
    /// <returns>The resolved language.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    string ResolveLanguage();
}
