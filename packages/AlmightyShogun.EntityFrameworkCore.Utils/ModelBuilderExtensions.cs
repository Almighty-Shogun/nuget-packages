using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlmightyShogun.EntityFrameworkCore.Utils;

public static class ModelBuilderExtensions
{
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
        ///
        /// <authors>Almighty-Shogun</authors>
        /// <since>1.0.0</since>
        public void ApplyOneToOne<TEntity, TDependent>(Expression<Func<TEntity, TDependent?>> navigation, Expression<Func<TDependent, object?>> foreignKey, Expression<Func<TEntity, object?>>? principalKey = null, bool isRequired = true, DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull) where TEntity : class where TDependent : class
        {
            ReferenceReferenceBuilder<TEntity, TDependent> relationship = modelBuilder
                .Entity<TEntity>()
                .HasOne(navigation)
                .WithOne()
                .HasForeignKey(foreignKey)
                .OnDelete(deleteBehavior)
                .IsRequired(isRequired);

            if (principalKey is not null)
                relationship.HasPrincipalKey(principalKey);
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
        ///
        /// <authors>Almighty-Shogun</authors>
        /// <since>1.0.0</since>
        public void ApplyOneToMany<TEntity, TDependent>(Expression<Func<TEntity, IEnumerable<TDependent>?>> navigation, Expression<Func<TDependent, object?>> foreignKey, Expression<Func<TEntity, object?>>? principalKey = null, bool isRequired = false, DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull) where TEntity : class where TDependent : class
        {
            ReferenceCollectionBuilder<TEntity, TDependent> relationship = modelBuilder
                .Entity<TEntity>()
                .HasMany(navigation)
                .WithOne()
                .HasForeignKey(foreignKey)
                .OnDelete(deleteBehavior)
                .IsRequired(isRequired);

            if (principalKey is not null)
                relationship.HasPrincipalKey(principalKey);
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
        ///
        /// <authors>Almighty-Shogun</authors>
        /// <since>1.0.0</since>
        public void ApplyManyToOne<TEntity, TDependent>(Expression<Func<TDependent, TEntity?>> navigation, Expression<Func<TDependent, object?>> foreignKey, Expression<Func<TEntity, object?>>? principalKey = null, bool isRequired = false, DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull) where TEntity : class where TDependent : class
        {
            ReferenceCollectionBuilder<TEntity, TDependent> relation = modelBuilder
                .Entity<TDependent>()
                .HasOne(navigation)
                .WithMany()
                .HasForeignKey(foreignKey)
                .OnDelete(deleteBehavior)
                .IsRequired(isRequired);

            if (principalKey is not null)
                relation.HasPrincipalKey(principalKey);
        }

        /// <summary>
        /// Configures automatic inclusion of the specified navigation property
        /// for the entity of type <typeparamref name="TEntity"/> during query execution.
        /// </summary>
        /// 
        /// <typeparam name="TEntity">The type of the entity for which the navigation property is configured.</typeparam>
        /// <param name="navigation">The navigation property to be automatically included in queries.</param>
        ///
        /// <authors>Almighty-Shogun</authors>
        /// <since>1.0.0</since>
        public void ApplyAutoInclude<TEntity>(Expression<Func<TEntity, object?>> navigation) where TEntity : class
        {
            modelBuilder
                .Entity<TEntity>()
                .Navigation(navigation)
                .AutoInclude();
        }
    }
}
