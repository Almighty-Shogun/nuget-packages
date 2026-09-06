namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when a verification link is presented with a token that cannot be redeemed, whether it is unknown, already spent,
/// past its expiry, issued for the other purpose, or a registration token naming an address the account no longer holds.
/// Answering the same way for all five stops the endpoint confirming which tokens once existed, which flow one belongs to,
/// or what address the account is on.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class InvalidEmailVerificationTokenException : Exception;
