namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Turns a message key into text in the language the caller asked for. Resolution never fails: an unresolvable key is
/// returned as-is, so a missing translation degrades to a readable identifier instead of an exception or a blank body.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IMessageResolver
{
    /// <summary>
    /// Resolves a message that takes no parameters. Equivalent to the overload with an empty parameter list, and the
    /// one to use for a fixed message, since a template resolved this way keeps its placeholders literally.
    /// </summary>
    ///
    /// <param name="key">
    /// The dot-separated key, such as <c>http-error.404</c>, where the first segment names the message file and the rest
    /// the path within it.
    /// </param>
    ///
    /// <returns>
    /// The message as the negotiated language defines it, or the key itself when that language does not. A returned key
    /// signals a message file missing an entry, since the key is not tried against any other language.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string Resolve(string key);

    /// <summary>
    /// Resolves a message and substitutes the supplied values into its placeholders.
    /// </summary>
    ///
    /// <param name="key">The dot-separated key, resolved in the same negotiated language as <see cref="Resolve(string)"/>.</param>
    /// <param name="parameters">
    /// The values substituted by position, as <c>{0}</c> and onwards. A count that does not match the template leaves
    /// the template unformatted rather than throwing, so a placeholder can survive into the response.
    /// </param>
    ///
    /// <returns>The formatted message, or the key itself when the negotiated language does not define it.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string Resolve(string key, IReadOnlyList<object?> parameters);

    /// <summary>
    /// Resolves the language that messages are currently being served in, following the same fallback chain as
    /// <see cref="Resolve(string)"/>.
    /// </summary>
    ///
    /// <returns>
    /// The first accepted language that has messages defined for it, or the configured default when none does. Suitable
    /// for the <c>Content-Language</c> header, since it names what was actually served rather than what was requested.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string ResolveLanguage();
}
