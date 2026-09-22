namespace AlmightyShogun.AspNet.Core;

/// <summary>
///  Maps exceptions to HTTP error responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IExceptionMapper
{
    /// <summary>
    /// Maps an exception to an HTTP error response.
    /// </summary>
    ///
    /// <param name="exception">
    /// The exception to map.
    /// </param>
    ///
    /// <returns>
    /// The error mapping, or <c>null</c> when the exception is not mapped.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    ErrorMapping? Map(Exception exception);
}
