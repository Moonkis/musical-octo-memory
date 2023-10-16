using System.Collections.Concurrent;

namespace octo
{
    /*
     * For transparancy I didn't come up with this on my own.
     * Source: https://stackoverflow.com/a/77000993
     */
    public class ConcurrentHashSet<T>
        where T : notnull
    {
        private readonly ConcurrentDictionary<T, byte> m_InternalDictionary;

        public ConcurrentHashSet() 
        {
            m_InternalDictionary = new ConcurrentDictionary<T, byte>();
        }

        public bool Contains(T item) => m_InternalDictionary.ContainsKey(item);
        public bool Add(T item) => m_InternalDictionary.TryAdd(item, byte.MinValue);
        public bool Remove(T item) => m_InternalDictionary.TryRemove(item, out _);
        public int Count => m_InternalDictionary.Count;
    }
}
