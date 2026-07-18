using Serilog.Events;
using Serilog.Parsing;
using Serilog.Formatting;
using System.Globalization;

namespace AlmightyShogun.Logging;

/// <summary>
/// Formats Serilog events with level colors, property colors, and optional property value formatting.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal sealed class ColorFormatter : ITextFormatter
{
    /// <summary>
    /// Writes a log event as colored console text.
    /// </summary>
    ///
    /// <param name="logEvent">The Serilog event containing the level, timestamp, template, properties, and optional exception.</param>
    /// <param name="output">The writer that receives the formatted log line.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public void Format(LogEvent logEvent, TextWriter output)
    {
        MessageTemplate messageTemplate = logEvent.MessageTemplate;
        IReadOnlyDictionary<string, LogEventPropertyValue> properties = logEvent.Properties;

        output.Write(GetLogLevelColor(logEvent.Level));
        output.Write($"[{logEvent.Timestamp:HH:mm:ss} {logEvent.Level.ToString()[..3].ToUpper()}] ");
        output.Write(AnsiColor.Reset);

        foreach (MessageTemplateToken token in messageTemplate.Tokens)
        {
            switch (token)
            {
                case TextToken textToken:
                    output.Write(textToken.Text);
                break;
                case PropertyToken propToken:
                {
                    string format = propToken.Format ?? "";
                    string propName = propToken.PropertyName;

                    string? colorSpec = null;
                    string? numericFormat = null;

                    if (format.Contains('|'))
                    {
                        string[] parts = format.Split('|', 2);

                        numericFormat = parts[0];
                        colorSpec = parts[1];
                    }
                    else
                    {
                        if (IsKnownColor(format))
                        {
                            colorSpec = format;
                        }
                        else
                        {
                            numericFormat = format;
                        }
                    }

                    if (!properties.TryGetValue(propName, out LogEventPropertyValue? propertyValue))
                    {
                        output.Write("{");
                        output.Write(propName);

                        if (!string.IsNullOrEmpty(format))
                        {
                            output.Write(":");
                            output.Write(format);
                        }

                        output.Write("}");
                        continue;
                    }

                    string renderedValue = RenderPropertyValue(propertyValue, numericFormat);

                    string ansiColor = colorSpec != null ? AnsiColor.FromShort(colorSpec) : GetDefaultColor(propertyValue);

                    output.Write(ansiColor);
                    output.Write(renderedValue);
                    output.Write(AnsiColor.Reset);

                    break;
                }
            }
        }

        if (logEvent.Exception is not null)
        {
            output.WriteLine();
            output.Write(AnsiColor.DarkGray);
            output.Write(logEvent.Exception);
            output.Write(AnsiColor.Reset);
        }

        output.WriteLine();
    }

    /// <summary>
    /// Renders a log event property value with optional numeric formatting.
    /// </summary>
    ///
    /// <param name="value">The log event property value to render.</param>
    /// <param name="numericFormat">The optional numeric format string to apply when the scalar value supports formatting.</param>
    ///
    /// <returns>The rendered property value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private static string RenderPropertyValue(LogEventPropertyValue value, string? numericFormat)
    {
        if (value is ScalarValue scalar)
        {
            object? obj = scalar.Value;

            if (obj == null)
                return "null";

            if (string.IsNullOrEmpty(numericFormat) || obj is not IFormattable formattable)
                return obj.ToString() ?? string.Empty;

            try
            {
                return formattable.ToString(numericFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
                return obj.ToString() ?? string.Empty;
            }
        }

        using var stringWriter = new StringWriter();
        value.Render(stringWriter);

        return stringWriter.ToString();
    }

    /// <summary>
    /// Determines whether a string is a supported color shorthand code.
    /// </summary>
    ///
    /// <param name="colorCode">The color shorthand code to evaluate.</param>
    ///
    /// <returns><c>true</c> when the value is a supported color shorthand code; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private static bool IsKnownColor(string colorCode)
    {
        if (string.IsNullOrWhiteSpace(colorCode)) return false;

        return colorCode.ToLowerInvariant() is "r" or "g" or "b" or "c" or "y" or "m" or "br" or "bg" or "bb" or "bc" or "by" or "bm";
    }

    /// <summary>
    /// Determines the default ANSI color for a log event property value.
    /// </summary>
    ///
    /// <param name="value">The log event property value to evaluate.</param>
    ///
    /// <returns>The ANSI color code selected for the property value type.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private static string GetDefaultColor(LogEventPropertyValue value)
    {
        if (value is not ScalarValue scalar)
            return AnsiColor.White;

        object? obj = scalar.Value;

        if (obj == null)
            return AnsiColor.DarkGray;

        return obj switch
        {
            string => AnsiColor.White,
            int or long or float or double or decimal => AnsiColor.Cyan,
            bool => AnsiColor.Magenta,
            _ => AnsiColor.White,
        };
    }

    /// <summary>
    /// Determines the ANSI color for a log event level.
    /// </summary>
    ///
    /// <param name="logLevel">The <see cref="LogEventLevel"/> value to evaluate.</param>
    ///
    /// <returns>The ANSI color code selected for the log level.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private static string GetLogLevelColor(LogEventLevel logLevel) => logLevel switch
    {
        LogEventLevel.Verbose => AnsiColor.White,
        LogEventLevel.Debug => AnsiColor.White,
        LogEventLevel.Information => AnsiColor.Green,
        LogEventLevel.Warning => AnsiColor.Yellow,
        LogEventLevel.Error => AnsiColor.Red,
        LogEventLevel.Fatal => AnsiColor.BrightRed,
        _ => AnsiColor.White
    };
}
