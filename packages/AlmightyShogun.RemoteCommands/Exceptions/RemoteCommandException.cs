namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// The base for every failure <see cref="RemoteCommandClient"/> reports, so one <c>catch</c> covers a command that did
/// not run without having to name each reason it did not.
/// </summary>
///
/// <param name="message">The explanation, safe to show to whoever ran the command.</param>
/// <param name="innerException">The transport failure underneath, or <c>null</c> when the server answered.</param>
///
/// <remarks>
/// Abstract on purpose. Which of these is thrown says what went wrong, so a caller that needs to tell a server that
/// refused from a server that never answered branches on the type rather than on a string.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class RemoteCommandException(string message, Exception? innerException = null) : Exception(message, innerException);
