namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when an operation that changes a credential or redeems a token lost the same collision with a concurrent write
/// on every one of its attempts. Nothing it meant to change was written: each attempt ran in a transaction of its own and
/// none of them reached its commit, so the request can be sent again unchanged.
/// </summary>
///
/// <param name="innerException">
/// The failure the last attempt lost to, kept as <see cref="Exception.InnerException"/> so the provider's own account of
/// the collision survives being answered with a status code. <c>null</c> when the last attempt lost to a guarded update
/// matching no row, since that collision fails no statement and leaves no exception to carry.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class ConcurrentSessionUpdateException(Exception? innerException = null) : Exception(null, innerException);
