using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Converters;
public abstract class GuidCollectionConverter: ValueConverter<ICollection<Guid>, string>
{
    public GuidCollectionConverter(char delimiter) : base(
        v => string.Join(delimiter, v),
        v => ConvertFromSuppliedStringToGuidArray(v, delimiter))
    {
    }

    private static ICollection<Guid> ConvertFromSuppliedStringToGuidArray(string suppliedString, char delimiter)
    {
        var delimitedStrings = suppliedString.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

        var resolvedGuids = new HashSet<Guid>();

        Array.ForEach(delimitedStrings, delimitedString =>
        {
            if (Guid.TryParse(delimitedString, out var guid))
            {
                resolvedGuids.Add(guid);
            }
        });

        return resolvedGuids;
    }
}
