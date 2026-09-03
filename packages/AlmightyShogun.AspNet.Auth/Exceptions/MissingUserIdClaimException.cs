namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Thrown when an authenticated principal carries no readable identifier, either because the claim is absent or because
/// its value is not a well-formed one. A token that authenticates but identifies nobody is refused rather than trusted.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class MissingUserIdClaimException : Exception;
