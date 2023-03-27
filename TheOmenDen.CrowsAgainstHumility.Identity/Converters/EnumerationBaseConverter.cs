using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;
using TheOmenDen.CrowsAgainstHumility.Core.Extensions;
using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Converters;
internal sealed class EnumerationBaseConverter<TKey, TValue> : ValueConverter<TKey, TValue>
    where TKey : EnumerationBase<TKey, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    private static Boolean CanConvertFrom(Type objectType) => objectType.IsDerivedFrom(typeof(EnumerationBase<,>));

    private static MethodInfo? GetMethodInfoFromBaseType(Type suppliedObjectType, Type valueType)
    {
        var currentType = suppliedObjectType.BaseType;

        while (currentType is not null or Object)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(EnumerationBase<,>))
            {
                return currentType.GetMethod("ParseFromValue", new [] { valueType });
            }

            currentType = currentType.BaseType;
        }

        return null;
    }

    public static TKey? GetFromValue(TValue value)
    {
        if (!CanConvertFrom(typeof(TKey)))
        {
            throw new NotImplementedException();
        }

        var method = GetMethodInfoFromBaseType(typeof(TKey), typeof(TValue));

        return method.Invoke(null, new[] { (object)value }) as TKey;
    }

    public EnumerationBaseConverter()
        : base(item => item.Value, key => GetFromValue(key), null)
    {
    }
}
