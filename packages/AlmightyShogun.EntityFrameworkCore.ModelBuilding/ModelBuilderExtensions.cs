using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlmightyShogun.EntityFrameworkCore.ModelBuilding;

/// <summary>
/// Provides convenience methods for configuring common Entity Framework Core mappings.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Provides model-building extensions for the specified builder.
    /// </summary>
    ///
    /// <param name="modelBuilder">The model builder to configure.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    extension(ModelBuilder modelBuilder)
    {
        /// <summary>
        /// Configures a one-to-one relationship.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal entity type.</typeparam>
        /// <typeparam name="TDependent">The dependent entity type.</typeparam>
        /// 
        /// <param name="navigation">The navigation from the principal to the dependent.</param>
        /// <param name="foreignKey">The foreign key on the dependent.</param>
        /// <param name="inverseNavigation">The navigation from the dependent to the principal, if any.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyOneToOne<TEntity, TDependent>(
            Expression<Func<TEntity, TDependent?>> navigation,
            Expression<Func<TDependent, object?>> foreignKey,
            Expression<Func<TDependent, TEntity?>>? inverseNavigation = null
        ) where TEntity : class where TDependent : class
        {
            modelBuilder.Entity<TEntity>()
                .HasOne(navigation)
                .WithOne(inverseNavigation)
                .HasForeignKey(foreignKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a one-to-one relationship using an alternate principal key.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal entity type.</typeparam>
        /// <typeparam name="TDependent">The dependent entity type.</typeparam>
        /// 
        /// <param name="navigation">The navigation from the principal to the dependent.</param>
        /// <param name="foreignKey">The foreign key on the dependent.</param>
        /// <param name="inverseNavigation">The navigation from the dependent to the principal, if any.</param>
        /// <param name="principalKey">The principal key targeted by the foreign key.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public ModelBuilder ApplyOneToOne<TEntity, TDependent>(
            Expression<Func<TEntity, TDependent?>> navigation,
            Expression<Func<TDependent, object?>> foreignKey,
            Expression<Func<TDependent, TEntity?>>? inverseNavigation,
            Expression<Func<TEntity, object?>> principalKey
        ) where TEntity : class where TDependent : class
        {
            modelBuilder.Entity<TEntity>()
                .HasOne(navigation)
                .WithOne(inverseNavigation)
                .HasForeignKey(foreignKey)
                .HasPrincipalKey(principalKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a one-to-many relationship.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal entity type.</typeparam>
        /// <typeparam name="TDependent">The dependent entity type.</typeparam>
        /// 
        /// <param name="navigation">The collection navigation from the principal to the dependents.</param>
        /// <param name="foreignKey">The foreign key on the dependent.</param>
        /// <param name="inverseNavigation">The navigation from the dependent to the principal, if any.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyOneToMany<TEntity, TDependent>(
            Expression<Func<TEntity, IEnumerable<TDependent>?>> navigation,
            Expression<Func<TDependent, object?>> foreignKey,
            Expression<Func<TDependent, TEntity?>>? inverseNavigation = null
        ) where TEntity : class where TDependent : class
        {
            modelBuilder.Entity<TEntity>()
                .HasMany(navigation)
                .WithOne(inverseNavigation)
                .HasForeignKey(foreignKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a one-to-many relationship using an alternate principal key.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal entity type.</typeparam>
        /// <typeparam name="TDependent">The dependent entity type.</typeparam>
        /// 
        /// <param name="navigation">The collection navigation from the principal to the dependents.</param>
        /// <param name="foreignKey">The foreign key on the dependent.</param>
        /// <param name="inverseNavigation">The navigation from the dependent to the principal, if any.</param>
        /// <param name="principalKey">The principal key targeted by the foreign key.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public ModelBuilder ApplyOneToMany<TEntity, TDependent>(
            Expression<Func<TEntity, IEnumerable<TDependent>?>> navigation,
            Expression<Func<TDependent, object?>> foreignKey,
            Expression<Func<TDependent, TEntity?>>? inverseNavigation,
            Expression<Func<TEntity, object?>> principalKey
        ) where TEntity : class where TDependent : class
        {
            modelBuilder.Entity<TEntity>()
                .HasMany(navigation)
                .WithOne(inverseNavigation)
                .HasForeignKey(foreignKey)
                .HasPrincipalKey(principalKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a many-to-one relationship.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal entity type.</typeparam>
        /// <typeparam name="TDependent">The dependent entity type.</typeparam>
        /// <param name="navigation">The navigation from the dependent to the principal.</param>
        /// <param name="foreignKey">The foreign key on the dependent.</param>
        /// <param name="inverseNavigation">The collection navigation from the principal to the dependents, if any.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        /// 
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyManyToOne<TEntity, TDependent>(
            Expression<Func<TDependent, TEntity?>> navigation,
            Expression<Func<TDependent, object?>> foreignKey,
            Expression<Func<TEntity, IEnumerable<TDependent>?>>? inverseNavigation = null
        ) where TEntity : class where TDependent : class
        {
            modelBuilder.Entity<TDependent>()
                .HasOne(navigation)
                .WithMany(inverseNavigation)
                .HasForeignKey(foreignKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a many-to-one relationship using an alternate principal key.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal entity type.</typeparam>
        /// <typeparam name="TDependent">The dependent entity type.</typeparam>
        /// 
        /// <param name="navigation">The navigation from the dependent to the principal.</param>
        /// <param name="foreignKey">The foreign key on the dependent.</param>
        /// <param name="inverseNavigation">The collection navigation from the principal to the dependents, if any.</param>
        /// <param name="principalKey">The principal key targeted by the foreign key.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public ModelBuilder ApplyManyToOne<TEntity, TDependent>(
            Expression<Func<TDependent, TEntity?>> navigation,
            Expression<Func<TDependent, object?>> foreignKey,
            Expression<Func<TEntity, IEnumerable<TDependent>?>>? inverseNavigation,
            Expression<Func<TEntity, object?>> principalKey
        ) where TEntity : class where TDependent : class
        {
            modelBuilder.Entity<TDependent>()
                .HasOne(navigation)
                .WithMany(inverseNavigation)
                .HasForeignKey(foreignKey)
                .HasPrincipalKey(principalKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a navigation to be automatically included in queries.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity containing the navigation.</typeparam>
        /// <param name="navigation">The navigation to automatically include.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the navigation configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyAutoInclude<TEntity>(Expression<Func<TEntity, object?>> navigation) where TEntity : class
        {
            modelBuilder.Entity<TEntity>().Navigation(navigation).AutoInclude();

            return modelBuilder;
        }

        /// <summary>
        /// Configures an index over the selected properties.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity containing the indexed properties.</typeparam>
        /// 
        /// <param name="index">The property or properties to index.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the index configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public ModelBuilder ApplyIndex<TEntity>(Expression<Func<TEntity, object?>> index) where TEntity : class
        {
            modelBuilder.Entity<TEntity>().HasIndex(index);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a unique index over the selected properties.
        /// </summary>
        ///
        /// <typeparam name="TEntity">
        /// The entity containing the indexed properties.
        /// </typeparam>
        /// <param name="index">The property or properties to index.</param>
        /// <param name="filter">An optional provider-specific SQL expression used to filter the index.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the unique index configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public ModelBuilder ApplyUniqueIndex<TEntity>(
            Expression<Func<TEntity, object?>> index,
            string? filter = null
        ) where TEntity : class
        {
            IndexBuilder<TEntity> indexBuilder = modelBuilder.Entity<TEntity>().HasIndex(index).IsUnique();

            if (filter is not null)
                indexBuilder.HasFilter(filter);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a many-to-many relationship using an explicitly named join entity.
        /// </summary>
        ///
        /// <typeparam name="TEntity">One side of the relationship.</typeparam>
        /// <typeparam name="TRelated">The other side of the relationship.</typeparam>
        /// <param name="navigation">The collection navigation to the related entities.</param>
        /// <param name="inverseNavigation">The inverse collection navigation.</param>
        /// <param name="joinTableName">The name of the join entity.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// <typeparamref name="TEntity"/> and <typeparamref name="TRelated"/> are the same type.
        /// </exception>
        /// 
        /// <remarks>
        /// Join foreign keys are named <c>{TypeName}Id</c>. Self-referencing relationships are not supported because
        /// this convention would produce identical foreign key names.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public ModelBuilder ApplyManyToMany<TEntity, TRelated>(
            Expression<Func<TEntity, IEnumerable<TRelated>?>> navigation,
            Expression<Func<TRelated, IEnumerable<TEntity>?>> inverseNavigation,
            string joinTableName
        ) where TEntity : class where TRelated : class
        {
            if (typeof(TEntity) == typeof(TRelated))
                throw new InvalidOperationException(
                    $"ApplyManyToMany cannot configure a self-referencing many-to-many on '{typeof(TEntity).Name}', "
                    + $"because both join columns would be named '{typeof(TEntity).Name}Id'. Configure the join entity "
                    + "with UsingEntity directly, giving each foreign key its own column name.");

            modelBuilder.Entity<TEntity>()
                .HasMany(navigation)
                .WithMany(inverseNavigation)
                .UsingEntity(
                    joinTableName,
                    left => left.HasOne(typeof(TRelated)).WithMany().HasForeignKey($"{typeof(TRelated).Name}Id"),
                    right => right.HasOne(typeof(TEntity)).WithMany().HasForeignKey($"{typeof(TEntity).Name}Id"));

            return modelBuilder;
        }

        /// <summary>
        /// Configures an enum property to be stored as a string.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity containing the property.</typeparam>
        /// <typeparam name="TProperty">The enum type.</typeparam>
        /// 
        /// <param name="property">The enum property to configure.</param>
        /// <param name="maxLength">The maximum length of the stored string.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the property configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public ModelBuilder ApplyEnumAsString<TEntity, TProperty>(
            Expression<Func<TEntity, TProperty>> property,
            int maxLength = 32
        ) where TEntity : class where TProperty : struct, Enum
        {
            modelBuilder.Entity<TEntity>().Property(property).HasConversion<string>().HasMaxLength(maxLength);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a nullable enum property to be stored as a string.
        /// </summary>
        /// 
        /// <typeparam name="TEntity">The entity containing the property.</typeparam>
        /// <typeparam name="TProperty">The enum type.</typeparam>
        /// <param name="property">The nullable enum property to configure.</param>
        /// <param name="maxLength">The maximum length of the stored string.</param>
        /// 
        /// <returns>The <see cref="ModelBuilder"/> instance with the property configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public ModelBuilder ApplyEnumAsString<TEntity, TProperty>(
            Expression<Func<TEntity, TProperty?>> property,
            int maxLength = 32
        ) where TEntity : class where TProperty : struct, Enum
        {
            modelBuilder.Entity<TEntity>().Property(property).HasConversion<string>().HasMaxLength(maxLength);

            return modelBuilder;
        }
    }
}
