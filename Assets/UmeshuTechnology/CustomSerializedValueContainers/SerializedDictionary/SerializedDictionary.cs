using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.Utility
{
    [System.Serializable]
    public class SerializedDictionary<T, K> : IDictionary<T, K>, ICustomSerializedProperty
    {
        [SerializeField]
        public List<SerializedDictionaryKeyValuePair> values = new();
        private Dictionary<T, K> dictionary;
        private Dictionary<T, K> Dictionary { get => dictionary ??= values.ToDictionary(_v => _v.key, _v => _v.value); }

        public K this[T _key] { get => ((IDictionary<T, K>)Dictionary)[_key]; set => ((IDictionary<T, K>)Dictionary)[_key] = value; }

        public ICollection<T> Keys => ((IDictionary<T, K>)Dictionary).Keys;

        public ICollection<K> Values => ((IDictionary<T, K>)Dictionary).Values;

        public int Count => ((ICollection<KeyValuePair<T, K>>)Dictionary).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<T, K>>)Dictionary).IsReadOnly;



        public void Add(T _key, K _value)
        {
            ((IDictionary<T, K>)Dictionary).Add(_key, _value);
#if UNITY_EDITOR

            Serialize();
#endif
        }

        public void Add(KeyValuePair<T, K> _item)
        {
            ((ICollection<KeyValuePair<T, K>>)Dictionary).Add(_item);
#if UNITY_EDITOR
            Serialize();
#endif
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<T, K>>)Dictionary).Clear();
#if UNITY_EDITOR
            Serialize();
#endif
        }

        public bool Contains(KeyValuePair<T, K> _item)
        {
            return ((ICollection<KeyValuePair<T, K>>)Dictionary).Contains(_item);
        }

        public bool ContainsKey(T _key)
        {
            return ((IDictionary<T, K>)Dictionary).ContainsKey(_key);
        }

        public void CopyTo(KeyValuePair<T, K>[] _array, int _arrayIndex)
        {
            ((ICollection<KeyValuePair<T, K>>)Dictionary).CopyTo(_array, _arrayIndex);
        }

        public IEnumerator<KeyValuePair<T, K>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<T, K>>)Dictionary).GetEnumerator();
        }

        public bool Remove(T _key)
        {
            bool _result = ((IDictionary<T, K>)Dictionary).Remove(_key);
#if UNITY_EDITOR
            Serialize();
#endif
            return _result;
        }

        public bool Remove(KeyValuePair<T, K> _item)
        {
            bool _result = ((ICollection<KeyValuePair<T, K>>)Dictionary).Remove(_item);
#if UNITY_EDITOR
            Serialize();
#endif
            return _result;
        }

        public bool TryGetValue(T _key, out K _value)
        {
            return ((IDictionary<T, K>)Dictionary).TryGetValue(_key, out _value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Dictionary).GetEnumerator();
        }

        [Serializable]
        public struct SerializedDictionaryKeyValuePair
        {
            public SerializedDictionaryKeyValuePair(T _key, K _value)
            {
                this.key = _key;
                this.value = _value;
            }
            public T key;
            public K value;
        }

#if UNITY_EDITOR
        public void Serialize()
        {
            if (Application.isPlaying) return;
            values = Dictionary.Select(_kvp => new SerializedDictionaryKeyValuePair(_kvp.Key, _kvp.Value)).ToList();
        }
#endif
    }
}