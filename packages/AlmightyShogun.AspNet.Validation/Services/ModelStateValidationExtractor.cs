using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Extracts validation errors from ASP.NET model state.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ModelStateValidationExtractor
{
    /// <summary>
    /// Checks whether model state contains body-level or JSON parsing errors.
    /// </summary>
    ///
    /// <param name="context">The MVC action context.</param>
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
            if (entry.Errors.Count == 0)
                continue;

            if (entry.Errors.Any(error => error.Exception is not null))
                return true;

            if (IsBodyLevelKey(key, bodyParameterNames))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Converts model state errors into a validation error bag.
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
            if (entry.Errors.Count == 0)
                continue;

            string field = ToCamelCase(GetFieldName(key));

            foreach (ModelError error in entry.Errors)
                errors.Add(field, ResolveErrorKey(error));
        }

        return errors;
    }

    /// <summary>
    /// Resolves the names of action parameters that are bound from the request body.
    /// </summary>
    ///
    /// <param name="context">The MVC action context.</param>
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
    /// Checks whether a model state key points to the request body itself.
    /// </summary>
    ///
    /// <param name="key">The model state key.</param>
    /// <param name="bodyParameterNames">The body-bound parameter names.</param>
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
    /// Resolves the validation message key for a model error.
    /// </summary>
    ///
    /// <param name="error">The model error.</param>
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
    /// Resolves the final field name segment from a model state key.
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
    /// Converts a field name to camel case.
    /// </summary>
    ///
    /// <param name="value">The field name.</param>
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
