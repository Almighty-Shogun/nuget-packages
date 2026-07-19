namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Resolves localized HTTP messages from message keys.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IMessageResolver
{
    /// <summary>
    /// Resolves a message by key.
    /// </summary>
    ///
    /// <param name="key">The message key to resolve.</param>
    ///
    /// <returns>The resolved message when it exists; otherwise, the original message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string Resolve(string key);

    /// <summary>
    /// Resolves a message by key and formats it with parameters.
    /// </summary>
    ///
    /// <param name="key">The message key to resolve.</param>
    /// <param name="parameters">The values used to format the resolved message.</param>
    ///
    /// <returns>The resolved and formatted message when it exists; otherwise, the original message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string Resolve(string key, IReadOnlyList<object?> parameters);
}
