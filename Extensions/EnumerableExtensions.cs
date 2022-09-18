namespace TheOmenDen.CrowsAgainstHumility.Extensions
{
    public static class EnumerableExtensions
    {
        public static T? GetRandomElement<T>(this IEnumerable<T> source)
        {
            var sourceAsArray = source?.ToArray() ?? Array.Empty<T>();
            if (!sourceAsArray.Any())
            {
                return default;
            }

            var random = new Random();
            var nextInt = random.Next(1, sourceAsArray.Length);

            return sourceAsArray[nextInt];
        }

        public static IEnumerable<T> GetRandomElements<T>(this IEnumerable<T> source)
        {

        }
    }
}
