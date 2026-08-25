namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Thrown when the server answered and declined to run the command. The request reached the listener, so retrying it
/// unchanged produces the same refusal.
/// </summary>
///
/// <param name="reason">What the server objected to.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class RemoteCommandRefusedException(RemoteCommandRefusal reason) : RemoteCommandException(Describe(reason))
{
    /// <summary>
    /// Gets what the server objected to, which is the value to branch on. A reason this client does not know reports
    /// <see cref="RemoteCommandRefusal.Other"/> rather than an undefined value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RemoteCommandRefusal Reason { get; } = reason;

    /// <summary>
    /// Explains a reason in words, chosen from the value itself so the message cannot drift from what a caller matches.
    /// </summary>
    ///
    /// <param name="reason">The reason to explain.</param>
    ///
    /// <returns>The explanation, safe to show to whoever ran the command.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
