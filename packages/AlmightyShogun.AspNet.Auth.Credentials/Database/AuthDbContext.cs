using Microsoft.EntityFrameworkCore;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Base database context for credential authentication entities.
/// </summary>
///
/// <typeparam name="TUser">The application's user entity type.</typeparam>
/// <param name="options"> The database context options.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public abstract class AuthDbContext<TUser>(DbContextOptions options) : DbContext(options) where TUser : AuthUser
{
    /// <summary>
    /// The application users.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DbSet<TUser> Users => Set<TUser>();

    /// <summary>
    /// The user sessions used for refresh token authentication.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    /// <summary>
    /// The password reset tokens, including redeemed tokens.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    /// <summary>
    /// The email verification tokens, including redeemed tokens.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();

    /// <summary>
    /// The user two-factor enrollments, including pending enrollments.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DbSet<UserTwoFactor> UserTwoFactors => Set<UserTwoFactor>();

    /// <summary>
    /// The user lockout records.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DbSet<UserLockout> UserLockouts => Set<UserLockout>();

    /// <summary>
    /// The two-factor recovery codes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DbSet<TwoFactorRecoveryCode> TwoFactorRecoveryCodes => Set<TwoFactorRecoveryCode>();

    /// <summary>
    /// The two-factor login challenges, including redeemed challenges.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public DbSet<TwoFactorChallenge> TwoFactorChallenges => Set<TwoFactorChallenge>();

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
            .Property(token => token.Purpose)
            .HasConversion<int>();

        modelBuilder.Entity<EmailVerificationToken>()
            .HasIndex(token => token.TokenHash)
            .IsUnique();

        modelBuilder.Entity<EmailVerificationToken>()
            .HasIndex(token => new { token.UserId, token.ExpiresAt });

        modelBuilder.Entity<TwoFactorChallenge>()
            .HasOne<TUser>()
            .WithMany()
            .HasForeignKey(challenge => challenge.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<TwoFactorChallenge>()
            .HasIndex(challenge => challenge.TokenHash)
            .IsUnique();

        modelBuilder.Entity<TwoFactorChallenge>()
            .HasIndex(challenge => new { challenge.UserId, challenge.ExpiresAt });
    }
}
