namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Retries transactional operations when database concurrency conflicts occur.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ConflictRetry
{
    /// <summary>
    /// Executes an operation with bounded retries for concurrency conflicts.
    /// </summary>
    ///
    /// <param name="operation">
    /// The operation to execute. Must manage its own transaction and return
    /// true on success or false when a guarded write fails.
    /// </param>
    ///
    /// <returns>A task that completes when the operation succeeds.</returns>
    ///
    /// <exception cref="ConcurrentSessionUpdateException">
    /// Thrown when all retry attempts fail due to concurrency conflicts.
    /// </exception>
    ///
    /// <remarks>
    /// Retries up to <see cref="AuthSessionDefaults.CredentialWriteAttempts"/> times.
    /// Non-concurrency exceptions are propagated without retrying.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static async Task RunAsync(Func<Task<bool>> operation)
    {
        for (var attempt = 1;; attempt++)
        {
            bool committed;

            try
            {
                committed = await operation();
            }
            catch (Exception exception) when (ConcurrencyConflict.IsConflict(exception))
            {
                if (attempt >= AuthSessionDefaults.CredentialWriteAttempts)
                    throw new ConcurrentSessionUpdateException(exception);

                continue;
            }

            if (committed) return;

            if (attempt >= AuthSessionDefaults.CredentialWriteAttempts)
                throw new ConcurrentSessionUpdateException();
        }
    }
}
