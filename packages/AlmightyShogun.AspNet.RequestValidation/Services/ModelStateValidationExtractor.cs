using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Turns MVC's model-state failures into this package's error shape, so a binding failure and a rule failure reach the client as the same
/// body rather than as two different ones.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ModelStateValidationExtractor
{
    /// <summary>
    /// Detects a failure against the body itself rather than one of its fields, which is the case that reports as an unreadable body.
    /// </summary>
    ///
    /// <param name="context">The action context the response is built for.</param>
    ///
    /// <returns><c>true</c> when model state contains a body-level error; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool HasBodyLevelError(ActionContext context)
    {
        HashSet<string> bodyParameterNames = GetBodyParameterNames(context);

        foreach ((string key, ModelStateEntry? entry) in context.ModelState)
        {
            if (entry.Errors.Count == 0) continue;

            if (IsBodyLevelKey(key, bodyParameterNames))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Rewrites each model-state entry as a field error, keyed by the path a client would recognize rather than by the binder's own key.
    /// </summary>
    ///
    /// <param name="modelState">The model state dictionary.</param>
    ///
    /// <returns>
    /// The failures keyed by full path, so two nested failures sharing a leaf name stay separate entries rather than merging into one.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationBag Extract(ModelStateDictionary modelState)
    {
        ValidationBag errors = new();

        foreach ((string key, ModelStateEntry? entry) in modelState)
        {
            if (entry.Errors.Count == 0) continue;

            string field = ToFieldPath(key);

            foreach (ModelError error in entry.Errors)
                errors.Add(field, ResolveErrorKey(error));
        }

        return errors;
    }

    /// <summary>
    /// Finds which parameters came from the body, so a key naming one of them can be told apart from a key naming a field inside it.
    /// </summary>
    ///
    /// <param name="context">The action context the response is built for.</param>
    ///
    /// <returns>The body-bound parameter names.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static HashSet<string> GetBodyParameterNames(ActionContext context)
    {
        HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);

        foreach (ParameterDescriptor parameter in context.ActionDescriptor.Parameters)
        {
            if (parameter.BindingInfo?.BindingSource == BindingSource.Body)
                names.Add(parameter.Name);

            if (parameter is ControllerParameterDescriptor controllerParameter
                && controllerParameter.ParameterInfo.GetCustomAttributes(typeof(FromBodyAttribute), true).Length > 0)
                names.Add(parameter.Name);
        }

        return names;
    }

    /// <summary>
    /// Reports whether a key addresses the whole body rather than a field, which decides between an invalid-body error and a field error.
    /// </summary>
    ///
    /// <param name="key">The model state key.</param>
    /// <param name="bodyParameterNames">
    /// The parameters bound from the body, used to tell a key naming the body apart from one naming a field.
    /// </param>
    ///
    /// <returns><c>true</c> when the key is body-level; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsBodyLevelKey(string key, HashSet<string> bodyParameterNames)
    {
        if (string.IsNullOrEmpty(key))
            return true;

        return key.Equals("$", StringComparison.Ordinal) || bodyParameterNames.Contains(key);
    }

    /// <summary>
    /// Chooses the message key for a binding failure, so the sentence a client reads comes from the same message files as every rule
    /// failure.
    /// </summary>
    ///
    /// <param name="error">One model-state entry, which may describe a field or the body as a whole.</param>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <remarks>
    /// The binder's own message is deliberately not used as the key. It is an unlocalized sentence that often quotes the value sent, so
    /// adopting it would make the reported code and error identifier change with the payload and leave the description in the framework's
    /// language rather than the caller's. The reason it carried is lost, which is the cost of an identifier a client can branch on.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ResolveErrorKey(ModelError error)
    {
        if (error.Exception is not null)
            return "validation.json";

        return string.IsNullOrWhiteSpace(error.ErrorMessage) ? "validation.required" : "validation.invalid";
    }

    /// <summary>
    /// Rewrites a model-state key as the path a client sees, renaming each segment while keeping the path intact.
    /// </summary>
    ///
    /// <param name="key">The model state key, which may be dotted, indexed, or both.</param>
    ///
    /// <returns>
    /// The full path with every segment renamed, so <c>BillingAddress.Street</c> reads as <c>billingAddress.street</c> and
    /// <c>Items[0].Name</c> as <c>items[0].name</c> . An empty key reports as <c>request</c> , since a failure against the whole payload
    /// has no field to name.
    /// </returns>
    ///
    /// <remarks>
    /// The path is kept rather than reduced to its last segment. Two nested failures often share a leaf name, and collapsing them merges
    /// unrelated errors into one entry that tells the client nothing about which property failed.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ToFieldPath(string key) => string.IsNullOrEmpty(key)
        ? "request"
        : string.Join('.', key.Split('.').Select(ToPathSegment));

    /// <summary>
    /// Renames one segment of a key, leaving any indexer attached to it untouched so the position it names survives the rename.
    /// </summary>
    ///
    /// <param name="segment">One dot-separated segment, such as <c>Street</c> or <c>Items[0]</c> .</param>
    ///
    /// <returns>The segment with its name part converted and its indexer, when it has one, reattached.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ToPathSegment(string segment)
    {
        int bracketIndex = segment.IndexOf('[', StringComparison.Ordinal);

        if (bracketIndex < 0)
            return ValidationFieldName.FromDeclaredName(segment);

        return ValidationFieldName.FromDeclaredName(segment[..bracketIndex]) + segment[bracketIndex..];
    }
}
