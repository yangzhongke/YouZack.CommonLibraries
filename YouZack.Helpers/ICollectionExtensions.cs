namespace System.Collections.Generic
{
    public static class ICollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToAdd)
        {
            foreach (var item in itemsToAdd)
            {
                collection.Add(item);
            }
        }
    }
}
