using Serilog.Events;
using Serilog.Parsing;
using Serilog.Formatting;
using System.Globalization;

namespace AlmightyShogun.Serilog;

/// <summary>
/// Formats Serilog events for colored console output.
/// </summary>
///
/// <param name="enableColors">
/// Whether ANSI colors are enabled.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal sealed class ColorFormatter(bool enableColors) : ITextFormatter
{
    /// <summary>
    /// Indicates whether ANSI colors should be enabled based on output redirection
    /// and the <c>NO_COLOR</c> environment variable.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static bool OutputSupportsColors { get; } =
        !Console.IsOutputRedirected && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NO_COLOR"));

    /// <summary>
    /// Formats a Serilog event and writes it to the output.
    /// </summary>
    ///
    /// <param name="logEvent">The log event to format.</param>
    /// <param name="output">The writer receiving the formatted event.</param>
    ///
    /// <remarks>
    /// Property format specifiers may include a color shorthand after <c>|</c>, such as <c>{Count:N0|c}</c>.
    /// Without an explicit color shorthand, property colors are selected from the value type.
    /// Missing properties are written back as placeholders rather than omitted.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public void Format(LogEvent logEvent, TextWriter output)
    {
        MessageTemplate messageTemplate = logEvent.MessageTemplate;
        IReadOnlyDictionary<string, LogEventPropertyValue> properties = logEvent.Properties;

        Write(output, GetLogLevelColor(logEvent.Level));

        output.Write(
            string.Create(
                CultureInfo.InvariantCulture,
                $"[{logEvent.Timestamp:HH:mm:ss} {logEvent.Level.ToString()[..3].ToUpperInvariant()}] "));

        Write(output, AnsiColor.Reset);

        foreach (MessageTemplateToken token in messageTemplate.Tokens)
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
                    string numericFormat = format;

                    if (format.Contains('|'))
                    {
                        string[] parts = format.Split('|', 2);

                        numericFormat = parts[0];
                        colorSpec = parts[1];
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

                    if (propToken.Alignment is { } alignment)
                        renderedValue = alignment.Direction == AlignmentDirection.Left
                            ? renderedValue.PadRight(alignment.Width)
                            : renderedValue.PadLeft(alignment.Width);

                    string ansiColor = colorSpec is not null ? AnsiColor.FromShort(colorSpec) : GetDefaultColor(propertyValue);

                    Write(output, ansiColor);
                    output.Write(renderedValue);
                    Write(output, AnsiColor.Reset);

                    break;
                }
            }

        if (logEvent.Exception is not null)
        {
            output.WriteLine();
            Write(output, AnsiColor.DarkGray);
            output.Write(logEvent.Exception);
            Write(output, AnsiColor.Reset);
        }

        output.WriteLine();
    }

    /// <summary>
    /// Renders a Serilog property value as text.
    /// </summary>
    ///
    /// <param name="value">
    /// The property value to render.
    /// </param>
    /// <param name="numericFormat">
    /// The format to apply to formattable scalar values.
    /// </param>
    ///
    /// <returns>The rendered value, or the literal <c>null</c> for a null scalar.</returns>
    ///
    /// <remarks>
    /// Formattable scalar values are rendered using <see cref="CultureInfo.InvariantCulture"/>.
    /// If a format is invalid, the value is rendered without it.
    /// Non-scalar values use Serilog's default rendering.
    /// </remarks>
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
            catch (FormatException)
            {
                return obj.ToString() ?? string.Empty;
            }
        }

        using var stringWriter = new StringWriter();
        value.Render(stringWriter);

        return stringWriter.ToString();
    }

    /// <summary>
    /// Writes an ANSI color code when colors are enabled.
    /// </summary>
    ///
    /// <param name="output">The writer to write to.</param>
    /// <param name="ansiColor">The ANSI color code to write.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void Write(TextWriter output, string ansiColor)
    {
        if (enableColors)
            output.Write(ansiColor);
    }

    /// <summary>
    /// Determines the default ANSI color for a property value.
    /// </summary>
    ///
    /// <param name="value">The property value to determine a color for.</param>
    ///
    /// <returns>
    /// Cyan for numeric values, magenta for booleans, dark gray for <c>null</c>,
    /// and white for all other values.
    /// </returns>
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
            byte or sbyte or short or ushort or int or uint or long or ulong or nint or nuint => AnsiColor.Cyan,
            float or double or decimal or Half => AnsiColor.Cyan,
            bool => AnsiColor.Magenta,
            _ => AnsiColor.White
        };
    }

    /// <summary>
    /// Determines the ANSI color for a log level.
    /// </summary>
    ///
    /// <param name="logLevel">The log level to determine a color for.</param>
    ///
    /// <returns>
    /// Green for <c>Information</c>, yellow for <c>Warning</c>, red for <c>Error</c>,
    /// bright red for <c>Fatal</c>, and white for <c>Verbose</c>, <c>Debug</c>, and unrecognized values.
    /// </returns>
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
