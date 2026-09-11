namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Thrown when the server disconnects before sending a response.
/// </summary>
///
/// <param name="innerException">
/// The underlying transport exception, if available.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class RemoteCommandDisconnectedException(Exception? innerException = null) : RemoteCommandException(
    "The server closed the connection without sending a response. The address may not be whitelisted.",
    innerException
);
