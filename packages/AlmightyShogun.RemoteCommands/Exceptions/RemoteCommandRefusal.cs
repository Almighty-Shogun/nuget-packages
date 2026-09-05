namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Why the server declined to run a command. Sent on the wire as its underlying number, so this is the vocabulary both
/// ends share and the only place a refusal is named.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public enum RemoteCommandRefusal
{
    /// <summary>
    /// What this package's own server answers with when a command threw anything but a cancellation or a
    /// <c>JsonException</c>, so between a matched pair of ends it means the command ran and failed rather than that it
    /// was declined. It is also what a value introduced by a newer server arrives as, since the client maps anything it
    /// has no name for onto this and does not keep the original number.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Other = 0,

    /// <summary>
    /// The request was not readable as JSON, so the server never looked for a command name. This package's own client
    /// serializes every request before sending it, so from such a client the bytes were valid when they left and the
    /// fault is on the connection: frames out of step with each other, or corruption in transit.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MalformedPayload,

    /// <summary>
    /// The request parsed but named no command. A blank or whitespace name produces this rather than
    /// <see cref="CommandNotFound"/>, because there was nothing to look up.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MissingCommandName,

    /// <summary>
    /// The server requires a pre-shared key and the one sent did not match, or none was sent. The address was
    /// whitelisted, otherwise the connection would have been dropped without an answer.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Unauthorized,

    /// <summary>
    /// No command is registered under that name. Names match with ordinal case sensitivity, so this is as likely to be a
    /// difference in capitalization as a command the server does not have.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    CommandNotFound,

    /// <summary>
    /// The command exists but a <c>JsonException</c> escaped it, usually because the data could not become its message type,
    /// a property carried the wrong JSON type, or the payload was not an object. A command that itself raises one after
    /// running is reported the same way. An omitted property binds to its default, unless the message marks it
    /// <c>required</c>, which is refused here the same way a wrong type is.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    InvalidMessage
}
