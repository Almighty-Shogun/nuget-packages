namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Thrown when a reset link is presented with a token that cannot be redeemed, whether it is unknown, already spent, or
/// past its expiry. Answering the same way for all three stops the endpoint confirming which tokens once existed.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class InvalidPasswordResetTokenException : Exception;
