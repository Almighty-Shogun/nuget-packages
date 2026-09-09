namespace AlmightyShogun.Utils;

/// <summary>
/// Provides utility methods for console input, output, and cancellation handling.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ConsoleUtils
{
    /// <summary>
    /// Indicates whether cancellation prevention is active.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool _cancellationPrevented;

    private static readonly Lock _cancellationLock = new();

    /// <summary>
    /// Sets the console window title.
    /// </summary>
    ///
    /// <param name="title">
    /// The title to set.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static void Title(string title) => Console.Title = title;

    /// <summary>
    /// Clears the previous console line and moves the cursor to the beginning.
    /// </summary>
    ///
    /// <remarks>
    /// Does nothing when output is redirected or the cursor is already at the top
    /// of the console.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public static void RemoveLastLine()
    {
        if (Console.IsOutputRedirected || Console.CursorTop <= 0) return;

        try
        {
            int line = Console.CursorTop - 1;

            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, line);
        }
        catch (Exception exception) when (exception is IOException or ArgumentOutOfRangeException or InvalidOperationException) { }
    }

    /// <summary>
    /// Prompts the user for input and returns the entered value.
    /// </summary>
    ///
    /// <param name="question">
    /// The question to display.
    /// </param>
    /// <param name="defaultValue">
    /// The value to return when no input is provided. If <c>null</c>,
    /// empty input causes the question to be repeated.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    ///
    /// <returns>
    /// The entered value or <paramref name="defaultValue"/> if the input
    /// is empty or the input stream has ended.
    /// </returns>
    ///
    /// <exception cref="OperationCanceledException">
    /// Operation was canceled.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static async Task<string?> AskQuestionAsync(
        string question,
        string? defaultValue = null,
        CancellationToken cancellationToken = default
    )
    {
        while (true)
        {
            await Console.Out.WriteAsync($"[QUESTION] {question}: ");
            Console.ForegroundColor = ConsoleColor.Blue;

            try
            {
                string? input = await Console.In.ReadLineAsync(cancellationToken);

                if (input is null)
                    return defaultValue;

                if (input.Length >= 1)
                    return input;

                if (defaultValue is not null)
                    return defaultValue;
            }
            finally
            {
                Console.ResetColor();
                RemoveLastLine();
            }
        }
    }

    /// <summary>
    /// Prevents console cancellation signals from terminating the process.
    /// </summary>
    ///
    /// <remarks>
    /// This operation is thread-safe and idempotent. Cancellation prevention
    /// remains active for the lifetime of the process.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    public static void PreventCancellation()
    {
        lock (_cancellationLock)
        {
            if (_cancellationPrevented)
                return;

            Console.CancelKeyPress += (_, e) => e.Cancel = true;
            _cancellationPrevented = true;
        }
    }
}
