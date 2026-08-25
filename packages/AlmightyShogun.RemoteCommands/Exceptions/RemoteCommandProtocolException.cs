namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Thrown when a frame arrived but was not a response envelope, which means the two ends of the connection disagree
/// about the wire format rather than that the command failed.
/// </summary>
///
/// <param name="message">What was wrong with the frame.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class RemoteCommandProtocolException(string message) : RemoteCommandException(message);
