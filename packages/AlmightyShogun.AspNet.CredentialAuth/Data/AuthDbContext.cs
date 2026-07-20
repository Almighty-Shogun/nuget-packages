using Microsoft.EntityFrameworkCore;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Provides the Entity Framework database context contract for authentication data.
/// </summary>
///
/// <param name="options">The Entity Framework database context options.</param>
///
/// <typeparam name="TUser">The authentication user entity type.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class AuthDbContext<TUser>(DbContextOptions options) : DbContext(options) where TUser : AuthUser
{
    /// <summary>
    /// Gets the authentication users.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DbSet<TUser> Users => Set<TUser>();

    /// <summary>
    /// Gets the authentication user sessions.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    /// <summary>
    /// Gets the password reset tokens.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

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
            .HasIndex(session => session.RefreshToken)
            .IsUnique();

        modelBuilder.Entity<UserSession>()
            .HasIndex(session => new { session.UserId, session.App, session.ExpiresAt });

        modelBuilder.Entity<PasswordResetToken>()
            .HasOne<TUser>()
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<TUser>()
            .HasIndex(user => user.Username)
            .IsUnique();

        modelBuilder.Entity<TUser>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<PasswordResetToken>()
            .HasIndex(token => token.TokenHash)
            .IsUnique();

        modelBuilder.Entity<PasswordResetToken>()
            .HasIndex(token => new { token.UserId, token.ExpiresAt });
    }
}
