namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Thrown when a path that cannot proceed without a refresh token finds no cookie carrying one. Indistinguishable from
/// the caller simply being signed out.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class MissingRefreshTokenException : Exception;
