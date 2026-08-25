namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to be absent from the request. Use it for server-controlled values that clients must never send. Presence rules run
/// before value rules, so a field this rejects reports that rather than a later format or size failure.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MissingAttribute() : ValidationRuleAttribute(PresenceMode.Missing);
