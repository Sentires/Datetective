using System;


namespace TheDates
{
    // Simple inspector-compatible class for identifying a value with a key
    // IE, easily loop and add to a dictionary
    [Serializable]
    public class Entry<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public TKey key;
        public TValue value;

        public Entry(TKey key, TValue value = default)
        {
            this.key = key;
            this.value = value;
        }

        public Entry() : this(default) { }
    }
}
