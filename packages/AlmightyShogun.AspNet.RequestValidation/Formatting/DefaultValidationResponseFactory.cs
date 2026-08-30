using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Core;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the standard error body for a validation failure, delegating the shape to <see cref="ValidationResponseWriter"/> so every path
/// that reports a failure produces the same envelope.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class DefaultValidationResponseFactory(ValidationResponseWriter responseWriter) : IValidationResponseFactory
{
    /// <inheritdoc />
    public IActionResult Create(ValidationResponseContext context)
        => HttpErrorResult.Create(responseWriter.CreateResponse(context.Errors));
}
