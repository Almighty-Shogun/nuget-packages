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

            if (entry.Errors.Any(error => error.Exception is not null))
                return true;

            if (IsBodyLevelKey(key, bodyParameterNames))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Rewrites each model-state entry as a field error, keyed by the public field name a client would recognize.
    /// </summary>
    ///
    /// <param name="modelState">The model state dictionary.</param>
    ///
    /// <returns>The extracted validation error bag.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationBag Extract(ModelStateDictionary modelState)
    {
        ValidationBag errors = new();

        foreach ((string key, ModelStateEntry? entry) in modelState)
        {
            if (entry.Errors.Count == 0) continue;

            string field = ToCamelCase(GetFieldName(key));

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
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ResolveErrorKey(ModelError error)
    {
        if (error.Exception is not null)
            return "validation.json";

        return !string.IsNullOrWhiteSpace(error.ErrorMessage) ? error.ErrorMessage : "validation.required";
    }

    /// <summary>
    /// Takes the last segment of a dotted or indexed key, so a nested failure is reported against the field it actually concerns.
    /// </summary>
    ///
    /// <param name="key">The model state key.</param>
    ///
    /// <returns>The field name.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string GetFieldName(string key)
    {
        if (string.IsNullOrEmpty(key))
            return "request";

        int dotIndex = key.LastIndexOf('.');

        return dotIndex >= 0 ? key[(dotIndex + 1)..] : key;
    }

    /// <summary>
    /// Converts a name to the camel-cased form failures are reported under, matching what a JSON client sent.
    /// </summary>
    ///
    /// <param name="value">The property name to convert, as declared in the type rather than as a client would spell it.</param>
    ///
    /// <returns>The camel-cased field name.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
