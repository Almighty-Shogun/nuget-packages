namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Thrown when a path that cannot proceed without a refresh token finds no cookie carrying one. Indistinguishable from
/// the caller simply being signed out, which is why it maps to <c>401</c> rather than a bad-request status.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class MissingRefreshTokenException : Exception;
