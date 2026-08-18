using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlmightyShogun.EntityFrameworkCore.Utils;

/// <summary>
/// Collapses the fluent chain a relationship, index, or owned value normally takes into a single call, so every mapping
/// in <c>OnModelCreating</c> is one statement taking the same parameters in the same order whatever its kind.
/// </summary>
///
/// <remarks>
/// The defaults are the helpers' own rather than EF Core's conventions: a relationship is required and cascades unless
/// told otherwise, where a convention-driven mapping infers both from the foreign key's nullability instead.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Provides the mapping helpers as extensions on the builder handed to <c>OnModelCreating</c>.
    /// </summary>
    ///
    /// <param name="modelBuilder">
    /// The builder the configuration is applied to. Every helper returns it, so mappings can be chained or written as
    /// separate statements without difference.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    extension(ModelBuilder modelBuilder)
    {
        /// <summary>
        /// Configures a one-to-one relationship in which <typeparamref name="TEntity"/> holds the key and
        /// <typeparamref name="TDependent"/> carries the foreign key.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal, whose key the foreign key points at.</typeparam>
        /// <typeparam name="TDependent">The dependent, which carries the foreign key column.</typeparam>
        /// <param name="navigation">The reference navigation on the principal that reaches the dependent.</param>
        /// <param name="foreignKey">
        /// The property on the dependent holding the key value. Use an anonymous-object expression for a composite key.
        /// </param>
        /// <param name="principalKey">
        /// The property the foreign key points at. Left unset, the principal's primary key is used; set it only to target an
        /// alternate key.
        /// </param>
        /// <param name="isRequired">
        /// Whether a dependent must have a principal. Passing <c>false</c> makes the foreign key column nullable.
        /// </param>
        /// <param name="deleteBehavior">
        /// What happens to the dependent when its principal is deleted. <see cref="DeleteBehavior.Cascade"/> deletes it,
        /// <see cref="DeleteBehavior.Restrict"/> blocks the delete, <see cref="DeleteBehavior.ClientSetNull"/> orphans it.
        /// </param>
        /// <param name="inverseNavigation">
        /// The navigation on the dependent pointing back at its principal. Left unset, the relationship has no inverse and the
        /// dependent cannot reach its principal in code.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the one-to-one relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyOneToOne<TEntity, TDependent>(
            Expression<Func<TEntity, TDependent?>> navigation,
            Expression<Func<TDependent, object?>> foreignKey,
            Expression<Func<TEntity, object?>>? principalKey = null,
            bool isRequired = true,
            DeleteBehavior deleteBehavior = DeleteBehavior.Cascade,
            Expression<Func<TDependent, TEntity?>>? inverseNavigation = null
        ) where TEntity : class where TDependent : class
        {
            ReferenceReferenceBuilder<TEntity, TDependent> relationship = modelBuilder.Entity<TEntity>()
                .HasOne(navigation)
                .WithOne(inverseNavigation)
                .HasForeignKey(foreignKey)
                .IsRequired(isRequired)
                .OnDelete(deleteBehavior);

            if (principalKey is not null)
                relationship.HasPrincipalKey(principalKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a one-to-many relationship, declared from the principal side that owns the collection.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal, holding the collection.</typeparam>
        /// <typeparam name="TDependent">The dependent, one row per collection item, carrying the foreign key.</typeparam>
        /// <param name="navigation">The collection navigation on the principal.</param>
        /// <param name="foreignKey">The property on each dependent pointing back at the principal.</param>
        /// <param name="principalKey">
        /// The property the foreign key points at. Left unset, the principal's primary key is used.
        /// </param>
        /// <param name="isRequired">
        /// Whether a dependent must belong to a principal. Passing <c>true</c> makes the foreign key column non-nullable, so
        /// an orphan cannot be saved.
        /// </param>
        /// <param name="deleteBehavior">
        /// What happens to the dependents when the principal is deleted. <see cref="DeleteBehavior.Cascade"/> deletes them
        /// with it; <see cref="DeleteBehavior.ClientSetNull"/> only works while the foreign key is nullable.
        /// </param>
        /// <param name="inverseNavigation">
        /// The reference navigation on the dependent pointing back at its principal, or unset for none.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the one-to-many relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyOneToMany<TEntity, TDependent>(
            Expression<Func<TEntity, IEnumerable<TDependent>?>> navigation,
            Expression<Func<TDependent, object?>> foreignKey,
            Expression<Func<TEntity, object?>>? principalKey = null,
            bool isRequired = false,
            DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull,
            Expression<Func<TDependent, TEntity?>>? inverseNavigation = null
        ) where TEntity : class where TDependent : class
        {
            ReferenceCollectionBuilder<TEntity, TDependent> relationship = modelBuilder.Entity<TEntity>()
                .HasMany(navigation)
                .WithOne(inverseNavigation)
                .HasForeignKey(foreignKey)
                .IsRequired(isRequired)
                .OnDelete(deleteBehavior);

            if (principalKey is not null)
                relationship.HasPrincipalKey(principalKey);

            return modelBuilder;
        }

        /// <summary>
        /// Configures the same shape as <c>ApplyOneToMany</c> from the dependent side, for a mapping written where the
        /// foreign key lives rather than where the collection does.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal, referenced by many dependents.</typeparam>
        /// <typeparam name="TDependent">The dependent, carrying the foreign key and the navigation being configured.</typeparam>
        /// <param name="navigation">The reference navigation on the dependent that reaches its principal.</param>
        /// <param name="foreignKey">The property on the dependent holding the principal's key value.</param>
        /// <param name="principalKey">
        /// The property the foreign key points at. Left unset, the principal's primary key is used.
        /// </param>
        /// <param name="isRequired">
        /// Whether a dependent must have a principal. Passing <c>true</c> makes the foreign key column non-nullable.
        /// </param>
        /// <param name="deleteBehavior">
        /// What happens to the dependents when the principal is deleted. <see cref="DeleteBehavior.ClientSetNull"/> requires
        /// a nullable foreign key, so pair it with <paramref name="isRequired"/> left off.
        /// </param>
        /// <param name="inverseNavigation">
        /// The collection navigation on the principal holding its dependents, or unset for none.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the many-to-one relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyManyToOne<TEntity, TDependent>(
            Expression<Func<TDependent, TEntity?>> navigation,
            Expression<Func<TDependent, object?>> foreignKey,
            Expression<Func<TEntity, object?>>? principalKey = null,
            bool isRequired = false,
            DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull,
            Expression<Func<TEntity, IEnumerable<TDependent>?>>? inverseNavigation = null
        ) where TEntity : class where TDependent : class
        {
            ReferenceCollectionBuilder<TEntity, TDependent> relationship = modelBuilder.Entity<TDependent>()
                .HasOne(navigation)
                .WithMany(inverseNavigation)
                .HasForeignKey(foreignKey)
                .IsRequired(isRequired)
                .OnDelete(deleteBehavior);

            if (principalKey is not null)
                relationship.HasPrincipalKey(principalKey);

            return modelBuilder;
        }

        /// <summary>
        /// Loads a navigation on every query for <typeparamref name="TEntity"/> without an explicit <c>Include</c>, for a
        /// relationship the entity is rarely useful without.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity whose queries gain the include.</typeparam>
        /// <param name="navigation">The navigation to load. Applies to every query for the entity, not just the ones nearby.</param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the navigation set to load automatically.</returns>
        ///
        /// <remarks>
        /// This affects reads the caller cannot see from the query, and the cost is paid on all of them. A query that does not
        /// want it must opt out with <c>IgnoreAutoIncludes</c>, so prefer it only where the navigation is genuinely always
        /// needed.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyAutoInclude<TEntity>(Expression<Func<TEntity, object?>> navigation) where TEntity : class
        {
            modelBuilder.Entity<TEntity>().Navigation(navigation).AutoInclude();

            return modelBuilder;
        }

        /// <summary>
        /// Adds an index, optionally unique, named, and filtered.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity the index is created on.</typeparam>
        /// <param name="index">
        /// The property to index, or an anonymous-object expression for a composite index, where column order matters.
        /// </param>
        /// <param name="isUnique">
        /// Whether the database rejects duplicate values, enforcing the constraint rather than only speeding up lookups.
        /// </param>
        /// <param name="databaseName">
        /// The index name in the database. Left unset, the generated name is used, which changes if the columns do; set it to
        /// keep the name stable across migrations.
        /// </param>
        /// <param name="filter">
        /// A raw SQL predicate limiting which rows are indexed. This is how a unique index tolerates many nulls, since most
        /// providers treat nulls as equal without one.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the index configured.</returns>
        ///
        /// <remarks>
        /// The filter is passed through verbatim and is provider-specific, so a value written for one database may not be
        /// valid on another.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public ModelBuilder ApplyIndex<TEntity>(
            Expression<Func<TEntity, object?>> index,
            bool isUnique = false,
            string? databaseName = null,
            string? filter = null
        ) where TEntity : class
        {
            IndexBuilder<TEntity> indexBuilder = modelBuilder.Entity<TEntity>().HasIndex(index).IsUnique(isUnique);

            if (databaseName is not null)
                indexBuilder.HasDatabaseName(databaseName);

            if (filter is not null)
                indexBuilder.HasFilter(filter);

            return modelBuilder;
        }

        /// <summary>
        /// Configures a many-to-many relationship over a join table that holds nothing but the two keys. Map the join entity
        /// yourself instead when the link needs its own columns, such as a timestamp or a role.
        /// </summary>
        ///
        /// <typeparam name="TEntity">One side of the relationship. Neither side is principal; the two are symmetric.</typeparam>
        /// <typeparam name="TRelated">The other side.</typeparam>
        /// <param name="navigation">The collection navigation on <typeparamref name="TEntity"/>.</param>
        /// <param name="inverseNavigation">
        /// The collection navigation on <typeparamref name="TRelated"/>. Required, because a join table cannot be built from
        /// one side alone.
        /// </param>
        /// <param name="joinTableName">
        /// The table name to create. Chosen explicitly, since a generated name is rarely what a schema wants.
        /// </param>
        /// <param name="foreignKey">
        /// The join column pointing at <typeparamref name="TEntity"/>. Left unset, it is named after the type with an
        /// <c>Id</c> suffix.
        /// </param>
        /// <param name="relatedForeignKey">
        /// The join column pointing at <typeparamref name="TRelated"/>, named the same way when left unset.
        /// </param>
        /// <param name="deleteBehavior">
        /// What happens to a join row when either entity it links is deleted. Applied to both sides, so the link cannot
        /// outlive one end of it.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the many-to-many relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public ModelBuilder ApplyManyToMany<TEntity, TRelated>(
            Expression<Func<TEntity, IEnumerable<TRelated>?>> navigation,
            Expression<Func<TRelated, IEnumerable<TEntity>?>> inverseNavigation,
            string joinTableName,
            string? foreignKey = null,
            string? relatedForeignKey = null,
            DeleteBehavior deleteBehavior = DeleteBehavior.Cascade
        ) where TEntity : class where TRelated : class
        {
            modelBuilder.Entity<TEntity>()
                .HasMany(navigation)
                .WithMany(inverseNavigation)
                .UsingEntity(
                    joinTableName,
                    left => left.HasOne(typeof(TRelated))
                        .WithMany()
                        .HasForeignKey(relatedForeignKey ?? $"{typeof(TRelated).Name}Id")
                        .OnDelete(deleteBehavior),
                    right => right.HasOne(typeof(TEntity))
                        .WithMany()
                        .HasForeignKey(foreignKey ?? $"{typeof(TEntity).Name}Id")
                        .OnDelete(deleteBehavior)
                );

            return modelBuilder;
        }

        /// <summary>
        /// Stores an enum as its member name, so the column is readable in the database and survives the members being
        /// renumbered or reordered.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity owning the property.</typeparam>
        /// <typeparam name="TProperty">The enum type, constrained so a non-enum cannot be passed by mistake.</typeparam>
        /// <param name="property">The property to store as text.</param>
        /// <param name="maxLength">
        /// The column length. It must fit the longest member name, otherwise the value is rejected at write time rather than
        /// at model build.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the enum property stored as text.</returns>
        ///
        /// <remarks>
        /// Renaming a member becomes a data change rather than a code change, since existing rows still hold the old name.
        /// Applying this to a column that already holds numbers needs a migration that converts the stored values.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public ModelBuilder ApplyEnumAsString<TEntity, TProperty>(
            Expression<Func<TEntity, TProperty>> property,
            int maxLength = 32
        ) where TEntity : class where TProperty : struct, Enum
        {
            modelBuilder.Entity<TEntity>().Property(property).HasConversion<string>().HasMaxLength(maxLength);

            return modelBuilder;
        }

        /// <summary>
        /// Maps a complex value into the owner's own table, for something like an address or a money amount that has no
        /// identity of its own and is never queried separately.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity whose table receives the columns.</typeparam>
        /// <typeparam name="TOwned">The owned type. It has no key and cannot be loaded on its own.</typeparam>
        /// <param name="navigation">The navigation to the owned value. Nullable, in which case every owned column is nullable.</param>
        /// <param name="columnPrefix">
        /// A prefix applied to each owned column name. Required when the owner holds two values of the same owned type, since
        /// their columns would otherwise collide.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the owned type mapped into the owner's table.</returns>
        ///
        /// <remarks>
        /// The prefix is applied to every property except keys, which are left alone because they are the link back to the
        /// owner and renaming them would break it.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public ModelBuilder ApplyOwned<TEntity, TOwned>(
            Expression<Func<TEntity, TOwned?>> navigation,
            string? columnPrefix = null
        ) where TEntity : class where TOwned : class
        {
            modelBuilder.Entity<TEntity>().OwnsOne(navigation, owned =>
                {
                    if (columnPrefix is null) return;

                    foreach (IMutableProperty ownedProperty in owned.OwnedEntityType.GetProperties())
                    {
                        if (ownedProperty.IsKey()) continue;

                        ownedProperty.SetColumnName($"{columnPrefix}{ownedProperty.Name}");
                    }
                }
            );

            return modelBuilder;
        }
    }
}
