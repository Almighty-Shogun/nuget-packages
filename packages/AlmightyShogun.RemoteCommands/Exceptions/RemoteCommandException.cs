using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// The base for the failures <see cref="RemoteCommandClient"/> raises itself, so one <c>catch</c> covers a refusal, an
/// unreachable server, and a disconnection without having to name each. It does not cover everything a send can throw:
/// a framing error surfaces as <see cref="InvalidDataException"/>, an unreadable envelope as <see cref="JsonException"/>,
/// and a canceled wait as <see cref="OperationCanceledException"/>, none of which derive from this type.
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
