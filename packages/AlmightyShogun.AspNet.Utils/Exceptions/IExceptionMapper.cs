namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Decides which exceptions a package or an application answers, and how. This package defines the shape and nothing
/// else: pair an implementation with your own <c>IExceptionHandler</c>, and the exceptions stay plain throughout.
/// </summary>
///
/// <remarks>
/// Called on the exception path of a failing request, and normally registered as a singleton, so an implementation
/// must be thread-safe and should be a pattern match rather than anything that touches configuration or a database.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IExceptionMapper
{
    /// <summary>
    /// Inspects one exception and either claims it or declines. Called on every failing request, so it runs on the
    /// error path of a live application rather than only at startup.
    /// </summary>
    ///
    /// <param name="exception">
    /// The exception being handled, of any type. An implementation matches the ones it owns and declines the rest
    /// rather than assuming it is the only mapper registered.
    /// </param>
    ///
    /// <returns>
    /// The mapping to answer with, or <c>null</c> to decline. A handler that gets <c>null</c> returns <c>false</c> and
    /// leaves the exception to the handlers behind it, so a mapper never has to guess a status for something it does
    /// not recognize.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ErrorMapping? Map(Exception exception);
}
