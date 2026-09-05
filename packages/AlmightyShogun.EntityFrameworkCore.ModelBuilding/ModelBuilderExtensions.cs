using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlmightyShogun.EntityFrameworkCore.ModelBuilding;

/// <summary>
/// Collapses the fluent chain a relationship or an index normally takes into a single call, so a mapping in
/// <c>OnModelCreating</c> is one statement rather than a chain. The one-to-one, one-to-many and many-to-one families share
/// one parameter order, navigation then foreign key then inverse navigation; the many-to-many, index, enum and
/// auto-include helpers take what they need instead.
/// </summary>
///
/// <remarks>
/// The two-argument relationship overloads call nothing beyond <c>HasOne</c> or <c>HasMany</c>, <c>WithOne</c> or
/// <c>WithMany</c>, and <c>HasForeignKey</c>: requiredness and delete behavior are left to whatever EF Core infers
/// from the foreign key's nullability, so a mapping written through them is the fluent chain it expands to and nothing
/// more. The four-argument overloads add <c>HasPrincipalKey</c>, which EF Core documents as introducing a unique
/// constraint when the target property is not already the primary key or one. The enum, unique-index and many-to-many
/// helpers each configure what the fluent chain would not give them by convention: an enum is stored as text rather than as
/// its number and given a column width, a unique index is marked unique and optionally filtered, and a many-to-many names
/// its own join entity and key columns. An alternate principal key lives in a separate overload rather than in an argument
/// every caller has to read past.
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
        /// <typeparam name="TDependent">The dependent, which carries the foreign key.</typeparam>
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
        /// <exception cref="ArgumentException">
        /// A lambda is not a simple property or field access, such as one calling a method or walking more than one
        /// member. EF Core reports the offending expression.
        /// </exception>
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
        /// <typeparam name="TDependent">The dependent, which carries the foreign key.</typeparam>
        /// <param name="navigation">
        /// The property on the principal that reaches the dependent. Which side declares it is what makes that side
        /// the principal, so naming the wrong one puts the foreign key on the wrong table.
        /// </param>
        /// <param name="foreignKey">
        /// The property on the dependent holding the key. Its nullability is what decides whether the relationship is
        /// required.
        /// </param>
        /// <param name="inverseNavigation">The property on the dependent pointing back, or <c>null</c> when it has none.</param>
        /// <param name="principalKey">
        /// The property on the principal the foreign key targets. EF Core documents <c>HasPrincipalKey</c> as
        /// introducing a unique constraint over it when it is not already the primary key or one, so it wants no
        /// <c>ApplyUniqueIndex</c> call of its own; the values behind it still have to stay unique.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// A lambda is not a simple property or field access, such as one calling a method or walking more than one
        /// member. EF Core reports the offending expression.
        /// </exception>
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
        /// <exception cref="ArgumentException">
        /// A lambda is not a simple property or field access, such as one calling a method or walking more than one
        /// member. EF Core reports the offending expression.
        /// </exception>
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
        /// <param name="inverseNavigation">The property on the dependent pointing back, or <c>null</c> when it has none.</param>
        /// <param name="principalKey">
        /// The property on the principal the foreign key targets. EF Core documents <c>HasPrincipalKey</c> as
        /// introducing a unique constraint over it when it is not already the primary key or one, so it wants no
        /// <c>ApplyUniqueIndex</c> call of its own; the values behind it still have to stay unique.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// A lambda is not a simple property or field access, such as one calling a method or walking more than one
        /// member. EF Core reports the offending expression.
        /// </exception>
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
        /// The reference property on the dependent. The foreign key this produces is the one the collection-side helper
        /// produces, but the navigations are not: this is the only one defined unless
        /// <paramref name="inverseNavigation"/> is supplied as well.
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
        /// <exception cref="ArgumentException">
        /// A lambda is not a simple property or field access, such as one calling a method or walking more than one
        /// member. EF Core reports the offending expression.
        /// </exception>
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
        /// The reference property on the dependent. The foreign key this produces is the one the collection-side helper
        /// produces, but the navigations are not: this is the only one defined unless
        /// <paramref name="inverseNavigation"/> is non-null as well.
        /// </param>
        /// <param name="foreignKey">
        /// The property on the dependent holding the key. Its nullability is what decides whether the reference is
        /// optional.
        /// </param>
        /// <param name="inverseNavigation">The collection property on the principal, or <c>null</c> when it exposes none.</param>
        /// <param name="principalKey">
        /// The property on the principal the foreign key targets. EF Core documents <c>HasPrincipalKey</c> as
        /// introducing a unique constraint over it when it is not already the primary key or one, so it wants no
        /// <c>ApplyUniqueIndex</c> call of its own; the values behind it still have to stay unique.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship configured.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// A lambda is not a simple property or field access, such as one calling a method or walking more than one
        /// member. EF Core reports the offending expression.
        /// </exception>
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
        /// Marks a navigation to be loaded with its owner, so the property is not left silently empty because an
        /// <c>Include</c> was forgotten. A query that calls <c>IgnoreAutoIncludes</c> opts back out of it.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The entity the navigation is declared on.</typeparam>
        /// <param name="navigation">
        /// The navigation to load eagerly. Every query returning the entity loads it too unless that query calls
        /// <c>IgnoreAutoIncludes</c>, so reach for it on small related data rather than on a large collection.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the navigation set to load eagerly.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// <paramref name="navigation"/> names something the model does not hold as a navigation, such as a scalar
        /// property. Configure the relationship first, then mark it auto-included.
        /// </exception>
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
        /// <typeparam name="TEntity">
        /// The entity the index is declared on. That is the table it lands in unless the entity shares one, as a derived
        /// type does under EF Core's default inheritance mapping.
        /// </typeparam>
        /// <param name="index">
        /// The property to index, or an anonymous object of properties for a composite index. Column order in a
        /// composite index is the order the anonymous object gives.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the index configured.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// A lambda is not a simple property or field access, such as one calling a method or walking more than one
        /// member. EF Core reports the offending expression.
        /// </exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public ModelBuilder ApplyIndex<TEntity>(Expression<Func<TEntity, object?>> index) where TEntity : class
        {
            modelBuilder.Entity<TEntity>().HasIndex(index);

            return modelBuilder;
        }

        /// <summary>
        /// Adds a unique index over the selected properties, so the database rejects a duplicate rather than relying on the
        /// code that writes it. A filter narrows which rows the constraint covers.
        /// </summary>
        ///
        /// <typeparam name="TEntity">
        /// The entity the constraint is declared on. That is the table it lands in unless the entity shares one, as a
        /// derived type does under EF Core's default inheritance mapping.
        /// </typeparam>
        /// <param name="index">
        /// The property to index, or an anonymous object of properties for a composite index. A composite unique index
        /// constrains the combination, not each column on its own.
        /// </param>
        /// <param name="filter">
        /// A provider-specific SQL predicate limiting which rows the constraint covers, such as
        /// <c>"[Email] IS NOT NULL"</c>. It reaches <c>HasFilter</c> unchanged, which EF Core documents as configuring
        /// the index's filter expression, so the text has to be valid for whichever provider renders it. Left unset,
        /// no filter is applied.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the unique index configured.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// A lambda is not a simple property or field access, such as one calling a method or walking more than one
        /// member. EF Core reports the offending expression.
        /// </exception>
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
        /// The collection property on <typeparamref name="TRelated"/>. This helper's signature requires it, though EF
        /// Core does not: a many-to-many navigated from one side only is configured by calling <c>WithMany</c> with no
        /// argument, which is past what this helper covers and needs the fluent call written out.
        /// </param>
        /// <param name="joinTableName">
        /// The table holding the pairs. Named explicitly because EF Core's generated name concatenates the two entity
        /// names, which reads poorly in a migration and changes if either type is renamed.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the relationship and its join table configured.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// A lambda is not a simple property or field access, such as one calling a method or walking more than one
        /// member. EF Core reports the offending expression. Also thrown when <paramref name="joinTableName"/> is empty
        /// or only whitespace, which EF Core rejects with the same emptiness check.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="joinTableName"/> is <c>null</c>.</exception>
        ///
        /// <remarks>
        /// A model needing different column names or a join entity of its own is past what this hides and should call
        /// <c>UsingEntity</c> directly.
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
        /// <typeparam name="TProperty">
        /// The enum being stored. The <c>struct</c> constraint excludes <see cref="Nullable{T}"/>, so a nullable enum
        /// property cannot be configured through this helper and needs the fluent call written out.
        /// </typeparam>
        /// <param name="property">The property to convert, stored through EF Core's enum-to-string conversion.</param>
        /// <param name="maxLength">
        /// The column width. It should fit the longest member name, though nothing here or in EF Core checks a written
        /// value against it, so what happens to a longer one is left to the provider. EF Core documents <c>-1</c> as
        /// meaning no maximum length at all.
        /// </param>
        ///
        /// <returns>The <see cref="ModelBuilder"/> instance with the property stored as text.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// <paramref name="property"/> is not a simple property or field access.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxLength"/> is less than <c>-1</c>. <c>-1</c> itself is accepted rather than rejected as
        /// out of range.
        /// </exception>
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
