using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
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
