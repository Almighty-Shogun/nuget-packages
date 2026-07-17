using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlmightyShogun.EntityFrameworkCore.Utils;

public static class ModelBuilderExtensions
{
    /// <param name="modelBuilder">The model builder used to register relationships.</param>
    extension(ModelBuilder modelBuilder)
    {
        /// <summary>
        /// Configures a one-to-one relationship where <typeparamref name="TEntity"/> is the principal
        /// and <typeparamref name="TDependent"/> is the dependent.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The type of the principal entity.</typeparam>
        /// <typeparam name="TDependent">The type of the dependent entity.</typeparam>
        /// <param name="navigation">Reference navigation on the principal entity.</param>
        /// <param name="foreignKey">Foreign key property on the dependent entity.</param>
        /// <param name="principalKey">Optional principal key property. Defaults to the primary key.</param>
        /// <param name="isRequired">Whether the relationship is required.</param>
        /// <param name="deleteBehavior">Delete behavior for the relationship.</param>
        /// <param name="inverseNavigation">Optional reference navigation on the dependent entity back to the principal entity. When omitted, the relationship has no inverse navigation.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance.</returns>
        ///
        /// <authors>Almighty-Shogun</authors>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyOneToOne<TEntity, TDependent>(Expression<Func<TEntity, TDependent?>> navigation, Expression<Func<TDependent, object?>> foreignKey, Expression<Func<TEntity, object?>>? principalKey = null, bool isRequired = true, DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull, Expression<Func<TDependent, TEntity?>>? inverseNavigation = null) where TEntity : class where TDependent : class
        {
            ReferenceReferenceBuilder<TEntity, TDependent> relationship = modelBuilder
                .Entity<TEntity>()
                .HasOne(navigation)
                .WithOne(inverseNavigation)
                .HasForeignKey(foreignKey)
                .OnDelete(deleteBehavior)
                .IsRequired(isRequired);

            if (principalKey is not null)
                relationship.HasPrincipalKey(principalKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a one-to-many relationship where <typeparamref name="TEntity"/> is the principal
        /// and <typeparamref name="TDependent"/> is the dependent.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The type of the principal entity.</typeparam>
        /// <typeparam name="TDependent">The type of the dependent entity.</typeparam>
        /// <param name="navigation">Collection navigation on the principal entity.</param>
        /// <param name="foreignKey">Foreign key property on the dependent entity.</param>
        /// <param name="principalKey">Optional principal key property. Defaults to the primary key.</param>
        /// <param name="isRequired">Whether the relationship is required.</param>
        /// <param name="deleteBehavior">Delete behavior for the relationship.</param>
        /// <param name="inverseNavigation">Optional reference navigation on the dependent entity back to the principal entity. When omitted, the relationship has no inverse navigation.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance.</returns>
        ///
        /// <authors>Almighty-Shogun</authors>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyOneToMany<TEntity, TDependent>(Expression<Func<TEntity, IEnumerable<TDependent>?>> navigation, Expression<Func<TDependent, object?>> foreignKey, Expression<Func<TEntity, object?>>? principalKey = null, bool isRequired = false, DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull, Expression<Func<TDependent, TEntity?>>? inverseNavigation = null) where TEntity : class where TDependent : class
        {
            ReferenceCollectionBuilder<TEntity, TDependent> relationship = modelBuilder
                .Entity<TEntity>()
                .HasMany(navigation)
                .WithOne(inverseNavigation)
                .HasForeignKey(foreignKey)
                .OnDelete(deleteBehavior)
                .IsRequired(isRequired);

            if (principalKey is not null)
                relationship.HasPrincipalKey(principalKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a many-to-one relationship where <typeparamref name="TEntity"/> is the principal
        /// and <typeparamref name="TDependent"/> is the dependent.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The type of the principal entity.</typeparam>
        /// <typeparam name="TDependent">The type of the dependent entity.</typeparam>
        /// <param name="navigation">Reference navigation on the dependent entity pointing to the principal entity.</param>
        /// <param name="foreignKey">Foreign key property on the dependent entity.</param>
        /// <param name="principalKey">Optional principal key property. Defaults to the primary key.</param>
        /// <param name="isRequired">Specifies whether the relationship is required.</param>
        /// <param name="deleteBehavior">Delete behavior for the relationship.</param>
        /// <param name="inverseNavigation">Optional collection navigation on the principal entity containing dependent entities. When omitted, the relationship has no inverse navigation.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance.</returns>
        ///
        /// <authors>Almighty-Shogun</authors>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyManyToOne<TEntity, TDependent>(Expression<Func<TDependent, TEntity?>> navigation, Expression<Func<TDependent, object?>> foreignKey, Expression<Func<TEntity, object?>>? principalKey = null, bool isRequired = false, DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull, Expression<Func<TEntity, IEnumerable<TDependent>?>>? inverseNavigation = null) where TEntity : class where TDependent : class
        {
            ReferenceCollectionBuilder<TEntity, TDependent> relation = modelBuilder
                .Entity<TDependent>()
                .HasOne(navigation)
                .WithMany(inverseNavigation)
                .HasForeignKey(foreignKey)
                .OnDelete(deleteBehavior)
                .IsRequired(isRequired);

            if (principalKey is not null)
                relation.HasPrincipalKey(principalKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures automatic inclusion of the specified navigation property
        /// for the entity of type <typeparamref name="TEntity"/> during query execution.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The type of the entity for which the navigation property is configured.</typeparam>
        /// <param name="navigation">The navigation property to be automatically included in queries.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance.</returns>
        ///
        /// <authors>Almighty-Shogun</authors>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyAutoInclude<TEntity>(Expression<Func<TEntity, object?>> navigation) where TEntity : class
        {
            modelBuilder
                .Entity<TEntity>()
                .Navigation(navigation)
                .AutoInclude();

            return modelBuilder;
        }

        /// <summary>
        /// Configures an index for the entity of type <typeparamref name="TEntity"/>.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The type of the entity for which the index is configured.</typeparam>
        /// <param name="index">The property or property set to include in the index.</param>
        /// <param name="isUnique">Specifies whether the index should enforce uniqueness.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance.</returns>
        ///
        /// <authors>Almighty-Shogun</authors>
        /// <since>Unreleased</since>
        public ModelBuilder ApplyIndex<TEntity>(Expression<Func<TEntity, object?>> index, bool isUnique = false) where TEntity : class
        {
            modelBuilder
                .Entity<TEntity>()
                .HasIndex(index)
                .IsUnique(isUnique);

            return modelBuilder;
        }
    }
}
