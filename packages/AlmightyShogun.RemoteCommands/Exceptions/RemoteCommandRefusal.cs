namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Specifies why a remote command request was refused.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public enum RemoteCommandRefusal
{
    /// <summary>
    /// The request failed for a reason not represented by another refusal value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Other = 0,

    /// <summary>
    /// The request payload contains malformed JSON.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    MalformedPayload,

    /// <summary>
    /// The request does not specify a command name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    MissingCommandName,

    /// <summary>
    /// The request does not provide a valid pre-shared key.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Unauthorized,

    /// <summary>
    /// No command is registered with the requested name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    CommandNotFound,

    /// <summary>
    /// The request data could not be bound to the command's message type.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    InvalidMessage
}
