namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Runs an operation that owns its own transaction again when another request wrote the same rows first, so a routine
/// session refresh landing while the operation is open costs it an attempt rather than the whole request.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ConflictRetry
{
    /// <summary>
    /// Runs an operation, and runs it again from the start whenever a collision defeats it, up to
    /// <see cref="AuthSessionDefaults.CredentialWriteAttempts"/> attempts.
    /// </summary>
    ///
    /// <param name="operation">
    /// The whole operation, its reads and its transaction included. It has to open and commit that transaction itself,
    /// since an attempt that lost has to read its rows again as well as write them again. It returns <c>true</c> once it
    /// has committed, and <c>false</c> when a guarded write matched no row, which is how a collision arrives that fails
    /// no statement.
    /// </param>
    ///
    /// <returns>A task that completes once an attempt has committed.</returns>
    ///
    /// <exception cref="ConcurrentSessionUpdateException">
    /// Every attempt lost the collision. It carries the last failure as its inner exception when a statement failed, and
    /// none when the last attempt lost by a guarded write matching no row. No attempt reached its commit, so nothing the
    /// operation meant to write survives.
    /// </exception>
    ///
    /// <remarks>
    /// Only a collision is retried. Anything else the operation throws, a refused password or a spent token included,
    /// leaves on the attempt that threw it, so a refusal costs one attempt rather than all of them.
    ///
    /// Nothing is done to the context between attempts, and nothing needs to be. The operations run through this write
    /// by statements the database applies directly rather than by saving tracked entities, so a lost attempt leaves
    /// nothing staged to be sent again, and whatever the application was tracking when it called in is left as it was.
    ///
    /// The attempts are bounded rather than repeated until one wins, so a user whose sessions are refreshing constantly
    /// cannot hold one request open indefinitely. That is also why the operation opens its own transaction per attempt:
    /// each attempt commits everything or nothing, so no retry can leave half of one behind.
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
