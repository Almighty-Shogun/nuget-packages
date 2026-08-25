namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Thrown when the connection opened but closed before a response arrived. The usual cause is the address not being
/// whitelisted, because the listener drops such a client without answering rather than explaining itself.
/// </summary>
///
/// <param name="innerException">
/// The transport failure underneath, or <c>null</c> when the peer closed cleanly between frames and there was no error
/// to observe.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class RemoteCommandDisconnectedException(Exception? innerException = null) : RemoteCommandException(
    "The server closed the connection without sending a response. The address may not be whitelisted.",
    innerException
);
