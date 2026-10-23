namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when all attempts to resolve a concurrent credential update fail.
/// </summary>
///
/// <param name="innerException"> The last concurrency exception, if available</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class ConcurrentSessionUpdateException(Exception? innerException = null) : Exception(null, innerException);
