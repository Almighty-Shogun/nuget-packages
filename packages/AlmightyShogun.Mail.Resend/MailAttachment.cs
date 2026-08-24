namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents a file delivered alongside a message body.
/// </summary>
///
/// <param name="FileName">
/// The name the recipient sees and saves under. It is sent verbatim, so a path separator here becomes part of the name.
/// </param>
/// <param name="Content">
/// The raw bytes, held in memory for the whole send, which is what bounds a workable attachment size rather than a limit
/// the package imposes.
/// </param>
/// <param name="ContentType">
/// The MIME type, or <c>null</c> to let Resend infer one from <paramref name="FileName"/>.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MailAttachment(string FileName, byte[] Content, string? ContentType = null);
