using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts;

public class StronglyTypedIdValueConverter(ValueConverterSelectorDependencies dependencies)
    : ValueConverterSelector(dependencies)
{
    private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo> _converters = [];

    public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type providerClrType)
    {
        var baseConverters = base.Select(modelClrType, providerClrType);
        foreach (var converter in baseConverters)
        {
            yield return converter;
        }

        var underlyingModelType = UnwrapNullableType(modelClrType);
        var underlyingProviderType = UnwrapNullableType(providerClrType);

        if (underlyingProviderType is not null && underlyingProviderType != typeof(Guid))
        {
            yield break;
        }

        var converterType = underlyingModelType.GetNestedType("EfCoreValueConverter");

        if (converterType != null)
        {
            yield return _converters.GetOrAdd((modelClrType, providerClrType), _ =>
            {
                return new ValueConverterInfo(modelClrType, typeof(Guid), Factory);

                ValueConverter Factory(ValueConverterInfo info) => (ValueConverter)Activator.CreateInstance(converterType, info.MappingHints);
            });
        }
    }

    private static Type UnwrapNullableType(Type type) => type is null ? null : Nullable.GetUnderlyingType(type) ?? type;
}