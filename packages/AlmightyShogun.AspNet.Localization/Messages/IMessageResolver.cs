namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Turns a message key into text in the negotiated language, which is the first of the caller's accepted languages, their
/// shortened forms, or the configured default that has any messages defined. A key that language does not define is
/// returned as-is, so a missing translation degrades to a readable identifier instead of a blank body.
/// </summary>
///
/// <remarks>
/// The registered implementation reads message files from disk as it resolves, so a directory disappearing or becoming
/// unreadable mid-request escapes as an exception rather than degrading. That matters most to the exception handlers that
/// resolve through this, where a throw while an error body is being built replaces the response with a second failure.
/// </remarks>
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
    /// <exception cref="DirectoryNotFoundException">A message directory was removed between being found and being enumerated.</exception>
    /// <exception cref="UnauthorizedAccessException">
    /// A message directory became unreadable between being found and being enumerated.
    /// </exception>
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
    /// The values substituted by position, as <c>{0}</c> and onwards. Too few for the template leaves it unformatted
    /// rather than throwing, so a placeholder can survive into the response; surplus values are ignored.
    /// </param>
    ///
    /// <returns>The formatted message, or the key itself when the negotiated language does not define it.</returns>
    ///
    /// <exception cref="DirectoryNotFoundException">A message directory was removed between being found and being enumerated.</exception>
    /// <exception cref="UnauthorizedAccessException">
    /// A message directory became unreadable between being found and being enumerated.
    /// </exception>
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
    /// The first candidate in the fallback chain that has messages defined for it, which is an accepted language, one of
    /// its progressively shortened forms such as <c>nl</c> for an accepted <c>nl-BE</c>, or the configured default when
    /// none of them does. Suitable for the <c>Content-Language</c> header, since it names what was actually served rather
    /// than what was requested.
    /// </returns>
    ///
    /// <exception cref="DirectoryNotFoundException">A message directory was removed between being found and being enumerated.</exception>
    /// <exception cref="UnauthorizedAccessException">
    /// A message directory became unreadable between being found and being enumerated.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string ResolveLanguage();
}
