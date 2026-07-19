namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Represents an exception that should be converted into a standardized HTTP error response.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class HttpErrorException : Exception
{
    /// <summary>
    /// Gets the optional message key that should be resolved for the response description.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal string? MessageKey { get; }

    /// <summary>
    /// Gets the values used to format the resolved message.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal object?[] MessageParameters { get; }

    /// <summary>
    /// Gets the HTTP status code that should be returned.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int StatusCode { get; }

    /// <summary>
    /// Creates an HTTP error exception for the specified status code and optional message key.
    /// </summary>
    ///
    /// <param name="statusCode">The HTTP status code that should be returned.</param>
    /// <param name="message">The optional message key to resolve for the response description.</param>
    /// <param name="parameters">The optional values used to format the resolved message.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public HttpErrorException(int statusCode, string? message = null, params object?[] parameters) : base(message)
    {
        StatusCode = statusCode;
        MessageKey = string.IsNullOrWhiteSpace(message) ? null : message;
        MessageParameters = parameters;
    }
}
