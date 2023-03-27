using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TheOmenDen.CrowsAgainstHumility.Core.Extensions;
using TheOmenDen.CrowsAgainstHumility.Identity.Converters;
using TheOmenDen.Shared.Enumerations;
using TypeExtensions = TheOmenDen.CrowsAgainstHumility.Core.Extensions.TypeExtensions;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Extensions;
public static class EnumerationBaseConverterExtensions
{
    public static void ConfigureEnumerationBase(this ModelConfigurationBuilder configurationBuilder)
    {
        var modelBuilder = configurationBuilder.CreateModelBuilder(null);

        var propertyTypes = modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.ClrType.GetProperties())
            .Where(p => p.PropertyType.IsDerivedFrom(typeof(EnumerationBase<,>)))
            .Select(prop => prop.PropertyType)
            .Distinct();

        foreach (var propertyType in propertyTypes)
        {
            var (enumerationType, keyType) =
                TypeExtensions.GetEnumerationWithValueTypes(propertyType, typeof(EnumerationBase<,>));

            if (enumerationType != propertyType)
            {
                continue;
            }

            var converterType = typeof(EnumerationBaseConverter<,>).MakeGenericType(propertyType, keyType);

            configurationBuilder.Properties(propertyType).HaveConversion(converterType);
        }
    }

    public static void ConfigureEnumerationBase(this ModelBuilder modelBuilder)
    {
        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = mutableEntityType.ClrType.GetProperties()
                .Where(p => p.PropertyType.IsDerivedFrom(typeof(EnumerationBase<,>)));

            foreach (var propertyInfo in properties)
            {
                var (enumerationType, keyType) = propertyInfo.PropertyType.GetEnumWithValueTypes(typeof(EnumerationBase<,>));

                if (enumerationType != propertyInfo.PropertyType)
                {
                    continue;
                }

                var converterType =
                    typeof(EnumerationBaseConverter<,>).MakeGenericType(propertyInfo.PropertyType, keyType);

                var converter = Activator.CreateInstance(converterType) as ValueConverter;

                var propertyBuilder = GetPropertyBuilder(modelBuilder, mutableEntityType, propertyInfo.Name);

                if (propertyBuilder is null)
                {
                    continue;
                }

                propertyBuilder.HasConversion(converter);
            }
        }
    }

    private static PropertyBuilder? GetPropertyBuilder(ModelBuilder modelBuilder, IMutableEntityType entityType,
        String propertyName)
    {
        var ownershipPath = Enumerable.Empty<IMutableForeignKey>().ToList();

        var currentEntityType = entityType;

        while (currentEntityType.IsOwned())
        {
            var ownership = currentEntityType.FindOwnership();

            if (ownership is null)
            {
                return null;
            }

            ownershipPath.Add(ownership);
            currentEntityType = ownership.PrincipalEntityType;
        }

        var entityTypeBuilder = modelBuilder.Entity(currentEntityType.Name);

        if (ownershipPath.Count is 0)
        {
            return entityTypeBuilder.Property(propertyName);
        }

        var ownedNavigationBuilder = GetOwnedNavigationBuilder(entityTypeBuilder, ownershipPath);

        return ownedNavigationBuilder?.Property(propertyName);
    }

    private static OwnedNavigationBuilder? GetOwnedNavigationBuilder(EntityTypeBuilder entityTypeBuilder,
        List<IMutableForeignKey> ownershipPath)
    {
        OwnedNavigationBuilder? ownedNavigationBuilder = null;

        for (var i = ownershipPath.Count - 1; i >= 0; i--)
        {
            var ownership = ownershipPath[i];

            var navigation = ownership.GetNavigation(pointsToPrincipal: false);

            if (navigation is null)
            {
                return null;
            }

            ownedNavigationBuilder = ownership.IsUnique
                ? entityTypeBuilder.OwnsOne(ownership.DeclaringEntityType.Name, navigation.Name)
                : entityTypeBuilder.OwnsMany(ownership.DeclaringEntityType.Name, navigation.Name);
        }

        return ownedNavigationBuilder;
    }
}
