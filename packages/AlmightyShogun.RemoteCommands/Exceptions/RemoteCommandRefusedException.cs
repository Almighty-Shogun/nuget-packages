namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Thrown when the server refuses a remote command request.
/// </summary>
///
/// <param name="reason">The refusal reason.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class RemoteCommandRefusedException(RemoteCommandRefusal reason) : RemoteCommandException(Describe(reason))
{
    /// <summary>
    /// Gets the refusal reason.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public RemoteCommandRefusal Reason { get; } = reason;

    /// <summary>
    /// Creates an error message for a refusal reason.
    /// </summary>
    ///
    /// <param name="reason">The refusal reason.</param>
    ///
    /// <returns>
    /// The corresponding error message.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static string Describe(RemoteCommandRefusal reason) => reason switch
    {
        RemoteCommandRefusal.MalformedPayload => "The server could not read the request as JSON.",
        RemoteCommandRefusal.MissingCommandName => "The request did not name a command to run.",
        RemoteCommandRefusal.Unauthorized => "The pre-shared key was missing or did not match.",
        RemoteCommandRefusal.CommandNotFound => "The server has no command registered under that name.",
        RemoteCommandRefusal.InvalidMessage => "The data sent did not match the message type the command expects.",
        _ => "The server refused the request for a reason this client does not recognize."
    };
}
