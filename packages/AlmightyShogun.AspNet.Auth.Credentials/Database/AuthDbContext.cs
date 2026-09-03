using Microsoft.EntityFrameworkCore;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// The context this package's tables live in. An application derives its own context from it, so the auth tables sit
/// alongside application data in one database and one transaction.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, which decides the shape of the users table.</typeparam>
/// <param name="options">
/// The provider and connection the context runs on, passed straight to <see cref="DbContext"/>. Credential data lives
/// wherever the application points this, rather than in a database of the package's own.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class AuthDbContext<TUser>(DbContextOptions options) : DbContext(options) where TUser : AuthUser
{
    /// <summary>
    /// Gets the users, of the application's own entity type, so an application adds its own columns without a second table.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DbSet<TUser> Users => Set<TUser>();

    /// <summary>
    /// Gets the refresh-token sessions, one row per signed-in device.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    /// <summary>
    /// Gets the outstanding password resets, at most one row per user. A spent row stays until that user requests
    /// another reset and it is reused, so a replay can be told from an unknown token in the meantime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    /// <summary>
    /// Gets the issued email verifications, including spent ones. The package issues none of these itself, so the set
    /// exists for an application's own sign-up and change-of-address flows to write through.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();

    /// <summary>
    /// Gets the two-factor enrolments, one per user who has ever enrolled. Kept separate from the user row so a secret
    /// is only read when a code is being verified.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DbSet<UserTwoFactor> UserTwoFactors => Set<UserTwoFactor>();

    /// <summary>
    /// Gets the lockout rows, one per account with a run of failures behind it. Empty in a deployment that leaves
    /// lockout disabled, and emptied for an account as soon as it signs in successfully.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DbSet<UserLockout> UserLockouts => Set<UserLockout>();

    /// <summary>
    /// Gets the recovery codes issued against those enrolments, one row per code so spending one does not rewrite the
    /// rest.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DbSet<TwoFactorRecoveryCode> TwoFactorRecoveryCodes => Set<TwoFactorRecoveryCode>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TUser>()
            .HasMany(user => user.Sessions)
            .WithOne()
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<UserSession>()
            .HasIndex(session => session.RefreshTokenHash)
            .IsUnique();

        modelBuilder.Entity<UserSession>()
            .HasIndex(session => new { session.UserId, session.App, session.ExpiresAt });

        modelBuilder.Entity<UserSession>()
            .Property(session => session.ConcurrencyToken)
            .IsConcurrencyToken();

        modelBuilder.Entity<PasswordResetToken>()
            .HasOne<TUser>()
            .WithOne()
            .HasForeignKey<PasswordResetToken>(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<TUser>()
            .HasIndex(user => user.Identifier)
            .IsUnique();

        modelBuilder.Entity<TUser>()
            .HasOne(user => user.TwoFactor)
            .WithOne()
            .HasForeignKey<UserTwoFactor>(twoFactor => twoFactor.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<UserTwoFactor>()
            .HasIndex(twoFactor => twoFactor.UserId)
            .IsUnique();

        modelBuilder.Entity<TUser>()
            .HasOne(user => user.Lockout)
            .WithOne()
            .HasForeignKey<UserLockout>(lockout => lockout.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<UserLockout>()
            .HasIndex(lockout => lockout.UserId)
            .IsUnique();

        modelBuilder.Entity<UserTwoFactor>()
            .HasMany(twoFactor => twoFactor.RecoveryCodes)
            .WithOne()
            .HasForeignKey(code => code.UserTwoFactorId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<TwoFactorRecoveryCode>()
            .HasIndex(code => new { code.UserTwoFactorId, code.CodeHash });

        modelBuilder.Entity<TUser>()
            .HasIndex(user => user.Username)
            .IsUnique();

        modelBuilder.Entity<TUser>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<PasswordResetToken>()
            .HasIndex(token => token.TokenHash)
            .IsUnique();

        modelBuilder.Entity<EmailVerificationToken>()
            .HasOne<TUser>()
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<EmailVerificationToken>()
            .HasIndex(token => token.TokenHash)
            .IsUnique();

        modelBuilder.Entity<EmailVerificationToken>()
            .HasIndex(token => new { token.UserId, token.ExpiresAt });
    }
}
