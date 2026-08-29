using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlmightyShogun.EntityFrameworkCore.ModelBuilding;

/// <summary>
/// Collapses the fluent chain a relationship or an index normally takes into a single call, so every mapping
/// in <c>OnModelCreating</c> is one statement taking the same parameters in the same order whatever its kind.
/// </summary>
///
/// <remarks>
/// Nothing here overrides an EF Core convention. Requiredness and delete behavior are left to be inferred from the
/// foreign key's nullability, so a mapping written through these helpers behaves exactly as the fluent equivalent
/// without the matching call would. What is not conventional, such as an alternate principal key, is a separate
/// overload rather than an argument every caller has to read past.
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
        /// <typeparam name="TDependent">The dependent, which carries the foreign key and cannot exist alone.</typeparam>
        /// <param name="navigation">
        /// The property on the principal that reaches the dependent. Which side declares it is what makes that side
        /// the principal, so naming the wrong one puts the foreign key on the wrong table.
        /// </param>
        /// <param name="foreignKey">
        /// The property on the dependent holding the key. Its nullability is what decides whether the relationship is
        /// required, so make it non-nullable for a dependent that must always have a principal.
        /// </param>
        /// <param name="inverseNavigation">
        /// The property on the dependent pointing back. Leave it unset when the dependent has no such property, which
        /// EF Core maps as a one-directional relationship rather than as an error.
        /// </param>
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
        /// Configures a one-to-one relationship whose foreign key points at an alternate key rather than the principal's
        /// primary key.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal, whose alternate key the foreign key points at.</typeparam>
        /// <typeparam name="TDependent">The dependent, which carries the foreign key and cannot exist alone.</typeparam>
        /// <param name="navigation">
        /// The property on the principal that reaches the dependent. Which side declares it is what makes that side
        /// the principal, so naming the wrong one puts the foreign key on the wrong table.
        /// </param>
        /// <param name="foreignKey">
        /// The property on the dependent holding the key. Its nullability is what decides whether the relationship is
        /// required.
        /// </param>
        /// <param name="inverseNavigation">
        /// The property on the dependent pointing back, or <c>null</c> when it has none. Required here only so this
        /// overload is told apart from the conventional one.
        /// </param>
        /// <param name="principalKey">
        /// The property on the principal the foreign key targets. EF Core promotes it to an alternate key, so it needs
        /// a unique index of its own and the values behind it have to stay unique.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
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
        /// Configures a one-to-many relationship in which <typeparamref name="TEntity"/> owns a collection of
        /// <typeparamref name="TDependent"/>.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal, holding the collection.</typeparam>
        /// <typeparam name="TDependent">The dependent, one row per item in that collection.</typeparam>
        /// <param name="navigation">
        /// The collection property on the principal. Its element type decides which entity is expected to carry the
        /// foreign key, which is the one held in the collection rather than the one holding it.
        /// </param>
        /// <param name="foreignKey">
        /// The property on the dependent holding the key. Its nullability is what decides whether a dependent may exist
        /// without a principal, and with it whether deleting the principal cascades or orphans the rows.
        /// </param>
        /// <param name="inverseNavigation">
        /// The property on the dependent pointing back at its principal. Leave it unset when the dependent has none.
        /// </param>
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
        /// Configures a one-to-many relationship whose foreign key points at an alternate key rather than the
        /// principal's primary key.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal, holding the collection.</typeparam>
        /// <typeparam name="TDependent">The dependent, one row per item in that collection.</typeparam>
        /// <param name="navigation">
        /// The collection property on the principal. Its element type decides which entity is expected to carry the
        /// foreign key, which is the one held in the collection rather than the one holding it.
        /// </param>
        /// <param name="foreignKey">
        /// The property on the dependent holding the key. Its nullability is what decides whether a dependent may exist
        /// without a principal.
        /// </param>
        /// <param name="inverseNavigation">
        /// The property on the dependent pointing back, or <c>null</c> when it has none. Required here only so this
        /// overload is told apart from the conventional one.
        /// </param>
        /// <param name="principalKey">
        /// The property on the principal the foreign key targets. EF Core promotes it to an alternate key, so it needs
        /// a unique index of its own and the values behind it have to stay unique.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
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
        /// Configures the same shape as a one-to-many, written from the dependent's side, for a model where the
        /// reference reads better than the collection.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal, at the single end.</typeparam>
        /// <typeparam name="TDependent">The dependent, at the many end, which carries the foreign key.</typeparam>
        /// <param name="navigation">
        /// The reference property on the dependent. Declaring it on the dependent is what puts the foreign key there,
        /// which is the difference between this and writing the same relationship from the collection side.
        /// </param>
        /// <param name="foreignKey">
        /// The property on the dependent holding the key. Its nullability is what decides whether the reference is
        /// optional, so a nullable key is how a dependent is allowed to stand alone.
        /// </param>
        /// <param name="inverseNavigation">
        /// The collection property on the principal. Leave it unset when the principal exposes no collection.
        /// </param>
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
        /// Configures a many-to-one relationship whose foreign key points at an alternate key rather than the
        /// principal's primary key.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The principal, at the single end.</typeparam>
        /// <typeparam name="TDependent">The dependent, at the many end, which carries the foreign key.</typeparam>
        /// <param name="navigation">
        /// The reference property on the dependent. Declaring it on the dependent is what puts the foreign key there,
        /// which is the difference between this and writing the same relationship from the collection side.
        /// </param>
        /// <param name="foreignKey">
        /// The property on the dependent holding the key. Its nullability is what decides whether the reference is
        /// optional.
        /// </param>
        /// <param name="inverseNavigation">
        /// The collection property on the principal, or <c>null</c> when it exposes none. Required here only so this
        /// overload is told apart from the conventional one.
        /// </param>
        /// <param name="principalKey">
        /// The property on the principal the foreign key targets. EF Core promotes it to an alternate key, so it needs
        /// a unique index of its own and the values behind it have to stay unique.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
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
        /// Marks a navigation to be loaded with its owner on every query, so the property is never silently empty
        /// because an <c>Include</c> was forgotten.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity the navigation is declared on.</typeparam>
        /// <param name="navigation">
        /// The navigation to load eagerly. It is loaded by every query against the entity, including ones that only
        /// need a projection, so reach for it on small related data rather than on a large collection.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the navigation set to load eagerly.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public ModelBuilder ApplyAutoInclude<TEntity>(Expression<Func<TEntity, object?>> navigation) where TEntity : class
        {
            modelBuilder.Entity<TEntity>().Navigation(navigation).AutoInclude();

            return modelBuilder;
        }

        /// <summary>
        /// Adds an index over one or more properties, which is what a column filtered or sorted on regularly needs.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity the index is created on, which is also the table it lands in.</typeparam>
        /// <param name="index">
        /// The property to index, or an anonymous object of properties for a composite index. Column order in a
        /// composite index is the order given, and only a leading subset of it can be used by a query.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the index configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public ModelBuilder ApplyIndex<TEntity>(Expression<Func<TEntity, object?>> index) where TEntity : class
        {
            modelBuilder.Entity<TEntity>().HasIndex(index);

            return modelBuilder;
        }

        /// <summary>
        /// Adds a unique index, which is how a value is kept unique in the database rather than only in the code that
        /// writes it.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity the constraint is created on, which is also the table it lands in.</typeparam>
        /// <param name="index">
        /// The property to index, or an anonymous object of properties for a composite index. A composite unique index
        /// constrains the combination, not each column on its own.
        /// </param>
        /// <param name="filter">
        /// A provider-specific predicate limiting which rows the constraint covers, such as
        /// <c>"[Email] IS NOT NULL"</c>. Worth setting over a nullable column, because several providers treat two
        /// nulls as equal and refuse the second row without it.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the unique index configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
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
        /// Configures a many-to-many relationship over an explicitly named join table, whose columns are named
        /// <c>{TypeName}Id</c> after the two entities.
        /// </summary>
        ///
        /// <typeparam name="TEntity">One side of the relationship.</typeparam>
        /// <typeparam name="TRelated">The other side, which the relationship treats no differently.</typeparam>
        /// <param name="navigation">
        /// The collection property on <typeparamref name="TEntity"/>, whose join column is named after that type.
        /// </param>
        /// <param name="inverseNavigation">
        /// The collection property on <typeparamref name="TRelated"/>. Both sides are required, because a many-to-many
        /// with a navigation on one side only has no second collection for EF Core to pair the join rows with.
        /// </param>
        /// <param name="joinTableName">
        /// The table holding the pairs. Named explicitly because EF Core's generated name concatenates the two entity
        /// names, which reads poorly in a migration and changes if either type is renamed.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship and its join table configured.</returns>
        ///
        /// <remarks>
        /// Both join columns are non-nullable, so EF Core cascades from either side by convention and a row disappears
        /// with whichever entity it referenced. A model needing different column names or a join entity of its own is
        /// past what this hides and should call <c>UsingEntity</c> directly.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public ModelBuilder ApplyManyToMany<TEntity, TRelated>(
            Expression<Func<TEntity, IEnumerable<TRelated>?>> navigation,
            Expression<Func<TRelated, IEnumerable<TEntity>?>> inverseNavigation,
            string joinTableName
        ) where TEntity : class where TRelated : class
        {
            modelBuilder.Entity<TEntity>()
                .HasMany(navigation)
                .WithMany(inverseNavigation)
                .UsingEntity(
                    joinTableName,
                    left => left.HasOne(typeof(TRelated)).WithMany().HasForeignKey($"{typeof(TRelated).Name}Id"),
                    right => right.HasOne(typeof(TEntity)).WithMany().HasForeignKey($"{typeof(TEntity).Name}Id")
                );

            return modelBuilder;
        }

        /// <summary>
        /// Stores an enum as its name rather than its underlying number, so a row stays readable and reordering the
        /// enum cannot silently repoint existing data.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity the property is declared on.</typeparam>
        /// <typeparam name="TProperty">The enum being stored, constrained to a value type so a nullable column still works.</typeparam>
        /// <param name="property">The property to convert. A value with no matching member fails on read, not on write.</param>
        /// <param name="maxLength">
        /// The column width. It has to fit the longest member name, so raise it before adding a longer one rather than
        /// after a write has already been truncated or refused.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the property stored as text.</returns>
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
    }
}
