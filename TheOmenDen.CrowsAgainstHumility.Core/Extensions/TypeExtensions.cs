namespace TheOmenDen.CrowsAgainstHumility.Core.Extensions;
public static class TypeExtensions
{
    /// <summary>
    /// Checks the supplied <paramref name="type"/> to ensure it's derived from <paramref name="baseType"/>
    /// </summary>
    /// <param name="type">The type we're checking</param>
    /// <param name="baseType">The base type we want to see if our supplied <paramref name="type"/> inherits/is derived</param>
    /// <returns><see langword="true"/> When <paramref name="baseType"/> is an ancestor; <see langword="false"/> otherwise </returns>
    public static Boolean IsDerivedFrom(this Type type, Type baseType)
    {
        var currentType = type.BaseType;

        while (currentType is not null or object)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == baseType)
            {
                return true;
            }

            currentType = currentType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="typeToTest"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public static Boolean IsAnAncestor(Type typeToTest, Type baseType)
    {
        var currentType = typeToTest.BaseType;

        while (currentType is not null or object)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == baseType)
            {
                return true;
            }

            currentType = currentType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objectType"></param>
    /// <param name="mainType"></param>
    /// <returns></returns>
    public static Type? GetValueType(Type objectType, Type mainType)
    {
        var currentType = objectType.BaseType;

        while (currentType is not null or object)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == mainType)
            {
                return currentType.GenericTypeArguments[1];
            }

            currentType = currentType.BaseType;
        }

        return default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objectType"></param>
    /// <param name="mainType"></param>
    /// <returns></returns>
    public static Type? GetBaseType(this Type objectType, Type mainType)
    {
        var currentType = objectType.BaseType;

        while (currentType is not null or object)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == mainType)
            {
                return currentType.GenericTypeArguments[1];
            }

            currentType = currentType.BaseType;
        }

        return null;
    }

    public static (Type? KeyType, Type? ValueType) GetEnumWithValueTypes(this Type objectType, Type mainType)
    {
        var currentType = objectType.BaseType;

        if (currentType is null)
        {
            return (null, null);
        }

        while (currentType is not null or object)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == mainType)
            {
                return (currentType.GenericTypeArguments[0], currentType.GenericTypeArguments[1]);
            }

            currentType = currentType.BaseType;
        }

        return (null, null);
    }

    public static (Type? KeyType, Type? ValueType) GetEnumerationWithValueTypes(Type objectType, Type mainType)
    {
        var currentType = objectType.BaseType;

        if (currentType is null)
        {
            return (null, null);
        }

        while (currentType is not null or object)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == mainType)
            {
                return (currentType.GenericTypeArguments[0], currentType.GenericTypeArguments[1]);
            }

            currentType = currentType.BaseType;
        }

        return (null, null);
    }
}