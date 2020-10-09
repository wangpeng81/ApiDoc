using ApiDoc.Models; 
using System;
using System.Collections;
using System.Collections.Generic; 　
using System.Runtime.CompilerServices;　
using System.Collections.Concurrent;
using System.Reflection;

namespace ApiDoc.Middleware
{
    public class DBRouteValueDictionary : IDictionary<string, DBInterfaceModel>, IReadOnlyDictionary<string, DBInterfaceModel>
    {
       
        private const int DefaultCapacity = 4;

        internal KeyValuePair<string, DBInterfaceModel>[] _arrayStorage;　
        private int _count;

        public DBRouteValueDictionary()
        {
            _arrayStorage = Array.Empty<KeyValuePair<string, DBInterfaceModel>>();
        }


        //public DBRouteValueDictionary(object values)
        //{
        //    if (values is DBRouteValueDictionary dictionary)
        //    {
        //        if (dictionary._propertyStorage != null)
        //        {
        //            // PropertyStorage is immutable so we can just copy it.
        //            _propertyStorage = dictionary._propertyStorage;
        //            _count = dictionary._count;
        //            return;
        //        }

        //        var count = dictionary._count;
        //        if (count > 0)
        //        {
        //            var other = dictionary._arrayStorage;
        //            var storage = new KeyValuePair<string, DBInterfaceModel>[count];
        //            Array.Copy(other, 0, storage, 0, count);
        //            _arrayStorage = storage;
        //            _count = count;
        //        }
        //        else
        //        {
        //            _arrayStorage = Array.Empty<KeyValuePair<string, DBInterfaceModel>>();
        //        }

        //        return;
        //    }

        //    if (values is IEnumerable<KeyValuePair<string, DBInterfaceModel>> keyValueEnumerable)
        //    {
        //        _arrayStorage = Array.Empty<KeyValuePair<string, DBInterfaceModel>>();

        //        foreach (var kvp in keyValueEnumerable)
        //        {
        //            Add(kvp.Key, kvp.Value);
        //        }

        //        return;
        //    }

        //    //if (values is IEnumerable<KeyValuePair<string, string>> stringValueEnumerable)
        //    //{
        //    //    _arrayStorage = Array.Empty<KeyValuePair<string, object>>();

        //    //    foreach (var kvp in stringValueEnumerable)
        //    //    {
        //    //        Add(kvp.Key, kvp.Value);
        //    //    }

        //    //    return;
        //    //}

        //    if (values != null)
        //    {
        //        var storage = new PropertyStorage(values);
        //        _propertyStorage = storage;
        //        _count = storage.Properties.Length;
        //    }
        //    else
        //    {
        //        _arrayStorage = Array.Empty<KeyValuePair<string, DBInterfaceModel>>();
        //    }
        //}


        #region IDictionary

        public DBInterfaceModel this[string key]
        {
            get
            {
                if (key == null)
                {
                    ThrowArgumentNullExceptionForKey();
                }

                DBInterfaceModel value;
                TryGetValue(key, out value);
                return value;
            }

            set
            {
                if (key == null)
                {
                    ThrowArgumentNullExceptionForKey();
                }

                EnsureCapacity(_count);

                var index = FindIndex(key);
                if (index < 0)
                {
                    EnsureCapacity(_count + 1);
                    _arrayStorage[_count++] = new KeyValuePair<string, DBInterfaceModel>(key, value);
                }
                else
                {
                    _arrayStorage[index] = new KeyValuePair<string, DBInterfaceModel>(key, value);
                }
            }
        }

        public ICollection<DBInterfaceModel> Values
        {
            get
            {
                EnsureCapacity(_count);

                var array = _arrayStorage;
                var values = new DBInterfaceModel[_count];
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = array[i].Value;
                }

                return values;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                EnsureCapacity(_count);

                var array = _arrayStorage;
                var keys = new string[_count];
                for (var i = 0; i < keys.Length; i++)
                {
                    keys[i] = array[i].Key;
                }

                return keys;
            }
        }

        public void Add(string key, DBInterfaceModel value)
        {
            if (key == null)
            {
                ThrowArgumentNullExceptionForKey();
            }

            EnsureCapacity(_count + 1);

            if (ContainsKeyArray(key))
            {
                var message = "已经存在";
                throw new ArgumentException(message, nameof(key));
            }

            _arrayStorage[_count] = new KeyValuePair<string, DBInterfaceModel>(key, value);
            _count++;
        }

        public bool ContainsKey(string key)
        {
            if (key == null)
            {
                ThrowArgumentNullExceptionForKey();
            }

            return ContainsKeyCore(key);
        }

        public bool Remove(string key)
        {
            if (key == null)
            {
                ThrowArgumentNullExceptionForKey();
            }

            if (Count == 0)
            {
                return false;
            }

            EnsureCapacity(_count);

            var index = FindIndex(key);
            if (index >= 0)
            {
                _count--;
                var array = _arrayStorage;
                Array.Copy(array, index + 1, array, index, _count - index);
                array[_count] = default;

                return true;
            }

            return false;
        }

        public bool Remove(string key, out object value)
        {
            if (key == null)
            {
                ThrowArgumentNullExceptionForKey();
            }

            if (_count == 0)
            {
                value = default;
                return false;
            }


            EnsureCapacity(_count);

            var index = FindIndex(key);
            if (index >= 0)
            {
                _count--;
                var array = _arrayStorage;
                value = array[index].Value;
                Array.Copy(array, index + 1, array, index, _count - index);
                array[_count] = default;

                return true;
            }

            value = default;
            return false;
        }

        public bool TryGetValue(string key, out DBInterfaceModel value)
        {
            if (key == null)
            {
                ThrowArgumentNullExceptionForKey();
            }

            return TryFindItem(key, out value);

        }


        IEnumerable<string> IReadOnlyDictionary<string, DBInterfaceModel>.Keys => Keys;

        IEnumerable<DBInterfaceModel> IReadOnlyDictionary<string, DBInterfaceModel>.Values => Values;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int capacity)
        {
            if (_arrayStorage.Length < capacity)
            {
                EnsureCapacitySlow(capacity);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryFindItem(string key, out DBInterfaceModel value)
        {
            var array = _arrayStorage;
            var count = _count;

            // Elide bounds check for indexing.
            if ((uint)count <= (uint)array.Length)
            {
                for (var i = 0; i < count; i++)
                {
                    if (string.Equals(array[i].Key, key, StringComparison.OrdinalIgnoreCase))
                    {
                        value = array[i].Value;
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }


        #endregion


        public static DBRouteValueDictionary FromArray(KeyValuePair<string, DBInterfaceModel>[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
 
            var start = 0;
            var end = items.Length - 1;
 
            while (start <= end)
            {
                if (items[start].Key != null)
                {
                    start++;
                }
                else if (items[end].Key != null)
                {
                    // Swap this item into start and advance
                    items[start] = items[end];
                    items[end] = default;
                    start++;
                    end--;
                }
                else
                { 
                    end--;
                }
            }

            return new DBRouteValueDictionary()
            {
                _arrayStorage = items,
                _count = start,
            };
        }
  
       
        
        public IEqualityComparer<string> Comparer => StringComparer.OrdinalIgnoreCase;
         
        public int Count => _count;
         
        bool ICollection<KeyValuePair<string, DBInterfaceModel>>.IsReadOnly => false;
       
        
        void ICollection<KeyValuePair<string, DBInterfaceModel>>.Add(KeyValuePair<string, DBInterfaceModel> item)
        {
            Add(item.Key, item.Value);
        }
       
        public void Clear()
        {
            if (_count == 0)
            {
                return;
            }
 
            Array.Clear(_arrayStorage, 0, _count);
            _count = 0;
        }
         
        bool ICollection<KeyValuePair<string, DBInterfaceModel>>.Contains(KeyValuePair<string, DBInterfaceModel> item)
        {
            return TryGetValue(item.Key, out var value) && EqualityComparer<object>.Default.Equals(value, item.Value);
        }

       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ContainsKeyCore(string key)
        {
            
                return ContainsKeyArray(key);
           
        }
         
        void ICollection<KeyValuePair<string, DBInterfaceModel>>.CopyTo(
            KeyValuePair<string, DBInterfaceModel>[] array,
            int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length || array.Length - arrayIndex < this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (Count == 0)
            {
                return;
            }

            EnsureCapacity(Count);

            var storage = _arrayStorage;
            Array.Copy(storage, 0, array, arrayIndex, _count);
        }
         
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
         
        IEnumerator<KeyValuePair<string, DBInterfaceModel>> IEnumerable<KeyValuePair<string, DBInterfaceModel>>.GetEnumerator()
        {
            return GetEnumerator();
        }
         
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        bool ICollection<KeyValuePair<string, DBInterfaceModel>>.Remove(KeyValuePair<string, DBInterfaceModel> item)
        {
            if (Count == 0)
            {
                return false;
            }

            EnsureCapacity(Count);

            var index = FindIndex(item.Key);
            var array = _arrayStorage;
            if (index >= 0 && EqualityComparer<object>.Default.Equals(array[index].Value, item.Value))
            {
                Array.Copy(array, index + 1, array, index, _count - index);
                _count--;
                array[_count] = default;
                return true;
            }

            return false;
        }
       
        public bool TryAdd(string key, DBInterfaceModel value)
        {
            if (key == null)
            {
                ThrowArgumentNullExceptionForKey();
            }

            if (ContainsKeyCore(key))
            {
                return false;
            }

            EnsureCapacity(Count + 1);
            _arrayStorage[Count] = new KeyValuePair<string, DBInterfaceModel>(key, value);
            _count++;
            return true;
        }
 
      
        private static void ThrowArgumentNullExceptionForKey()
        {
            throw new ArgumentNullException("key");
        }

       
        private void EnsureCapacitySlow(int capacity)
        { 
            if (_arrayStorage.Length < capacity)
            {
                capacity = _arrayStorage.Length == 0 ? DefaultCapacity : _arrayStorage.Length * 2;
                var array = new KeyValuePair<string, DBInterfaceModel>[capacity];
                if (_count > 0)
                {
                    Array.Copy(_arrayStorage, 0, array, 0, _count);
                }

                _arrayStorage = array;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindIndex(string key)
        {
            // Generally the bounds checking here will be elided by the JIT because this will be called
            // on the same code path as EnsureCapacity.
            var array = _arrayStorage;
            var count = _count;

            for (var i = 0; i < count; i++)
            {
                if (string.Equals(array[i].Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ContainsKeyArray(string key)
        {
            var array = _arrayStorage;
            var count = _count;
            　
            if ((uint)count <= (uint)array.Length)
            {
                for (var i = 0; i < count; i++)
                {
                    if (string.Equals(array[i].Key, key, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
 
        public struct Enumerator : IEnumerator<KeyValuePair<string, DBInterfaceModel>>
        {
            private readonly DBRouteValueDictionary _dictionary;
            private int _index;

            public Enumerator(DBRouteValueDictionary dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException();
                }

                _dictionary = dictionary;

                Current = default;
                _index = 0;
            }

            public KeyValuePair<string, DBInterfaceModel> Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            // Similar to the design of List<T>.Enumerator - Split into fast path and slow path for inlining friendliness
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                var dictionary = _dictionary;

                // The uncommon case is that the propertyStorage is in use
                if (((uint)_index < (uint)dictionary._count))
                {
                    Current = dictionary._arrayStorage[_index];
                    _index++;
                    return true;
                }

                return false;
            }
 
            public void Reset()
            {
                Current = default;
                _index = 0;
            }
        }
 
    }
}
