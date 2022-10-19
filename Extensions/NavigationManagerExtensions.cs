using Microsoft.AspNetCore.WebUtilities;
using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Extensions;
#nullable disable
public static class NavigationManagerExtensions
{
    public static (bool isSuccessful, T result) TryGetQueryString<T>(this NavigationManager navigationManager,
        String key)
    {
        var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

        if (!QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
        {
            return new(false, default);
        }


        var conversionType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (conversionType.IsEnum)
        {
            var (success, result) = EnumTryParse<T>(valueFromQueryString, conversionType);

            if (success)
            {
                var value = result;

                return (true, value);
            }
        }
        if (conversionType == typeof(int)
            && Int32.TryParse(valueFromQueryString, out var valueAsInt))
        {
            var value = (T)(object)valueAsInt;
            return new(true, value);
        }

        if (conversionType == typeof(decimal)
            && Decimal.TryParse(valueFromQueryString, out var valueAsDecimal))
        {
            var value = (T)(object)valueAsDecimal;
            return new(true, value);
        }

        if (conversionType == typeof(bool)
            && Boolean.TryParse(valueFromQueryString, out var valueAsBoolean))
        {
            var value = (T)(object)valueAsBoolean;
            return new(true, value);
        }

        if (conversionType == typeof(string))
        {
            var value = (T)(object)valueFromQueryString.ToString();
            return new(true, value);
        }

        return new(false, default);
    }

    public static (bool isSuccessful, TValue result) EnumTryParse<TValue>(string? input, Type conversionType)
    {
        if (input is null || !Enum.GetNames(conversionType)
                .Any(en => en.Equals(input, StringComparison.InvariantCultureIgnoreCase)))
        {
            return new(false, default);
        }

        var theEnum = (TValue)Enum.Parse(conversionType, input, true);
        return new(true, theEnum);
    }

    public static (bool isSuccessful, TValue result) EnumerationBaseTryParse<TValue>(string input)
    where TValue: EnumerationBase<TValue, Int32>
    => EnumerationBase<TValue, Int32>.TryParse(input, true);
    
}
