using System;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.Utility
{
    [System.Serializable]
    public abstract class AbstractArrayBasedSelector<TKey, TValue> : IArrayBasedSelector
    {
        public AbstractArrayBasedSelector() { }

        public abstract List<ArrayBasedSelectorEntry<string, TValue>> Entries { get; }
        private string[] GetStringCollection() => GetCollection().ExtractArray(_key => ConvertKeyToString(_key));

        public TValue this[TKey _key] => GetValue(_key);
        public TValue GetValue(TKey _key)
        {
            ArrayBasedSelectorEntry<string, TValue> _entry = Entries.Find(_entry => _entry.key == ConvertKeyToString(_key));
            return UfLogger.LogErrorIfFalse(_entry != null, $"Non existing key {ConvertKeyToString(_key)}") ? _entry.value : default;
        }

        public void DoMethodOnAll(TKey _key, Action<bool, TValue> _method)
        {
            string _keyString = ConvertKeyToString(_key);
            foreach (ArrayBasedSelectorEntry<string, TValue> _entry in Entries)
                _method?.Invoke(_entry.key == _keyString, _entry.value);
        }

        public void DoMethodOnValue(TKey _key, Action<TValue> _method) => _method?.Invoke(GetValue(_key));

        public void UpdateDictionaryKeys()
        {
            string[] _collection = GetStringCollection();
            foreach (ArrayBasedSelectorEntry<string, TValue> _entry in Entries)
                _entry.error = !_collection.Contains(_entry.key);

            foreach (string _collectionKey in _collection)
            {
                ArrayBasedSelectorEntry<string, TValue> _existingEntry = Entries.FirstOrDefault(_entry => _entry.key == _collectionKey);
                bool _entryExists = _existingEntry != null;
                if (!_entryExists)
                    Entries.Add(new(_collectionKey, default, false));
            }
        }

        public bool AreKeysInSameOrderAsCollection()
        {
            string[] _collection = GetStringCollection();
            for (int _i = 0; _i < _collection.Length; _i++)
                if (Entries[_i].key != _collection[_i]) return false;
            return true;
        }

        public void MatchCollectionOrder()
        {
            UpdateDictionaryKeys();
            string[] _collection = GetStringCollection();
            for (int _i = 0; _i < _collection.Length; _i++)
            {
                string _collectionKey = _collection[_i];
                ArrayBasedSelectorEntry<string, TValue> _existingEntry = Entries.FirstOrDefault(_entry => _entry.key == _collectionKey);
                bool _entryExists = _existingEntry != null;
                if (_entryExists)
                {
                    Entries.Remove(_existingEntry);
                    Entries.Insert(_i, _existingEntry);
                }
            }
        }

        public void RemoveUnlinkedKeys()
        {
            string[] _collection = GetStringCollection();
            foreach (ArrayBasedSelectorEntry<string, TValue> _entry in Entries.ToArray())
                if (!_collection.Contains(_entry.key)) Entries.Remove(_entry);
        }

        public bool HasUnlinkedKeys()
        {
            string[] _collection = GetStringCollection();
            foreach (ArrayBasedSelectorEntry<string, TValue> _entry in Entries.ToArray())
                if (!_collection.Contains(_entry.key)) return true;
            return false;
        }

        public abstract string ConvertKeyToString(TKey _key);
        public abstract TKey[] GetCollection();

        internal void SetValueAt(TKey _key, TValue _value)
        {
            ArrayBasedSelectorEntry<string, TValue> _entry = Entries.Find(_entry => _entry.key == ConvertKeyToString(_key));
            if (UfLogger.LogErrorIfFalse(_entry != null, $"Non existing key {ConvertKeyToString(_key)}")) _entry.value = _value;

        }
    }

    [System.Serializable]
    public class ArrayBasedSelectorEntry<TKey, TValue>
    {
        public TKey key;
        public TValue value;
        public bool error;

        public ArrayBasedSelectorEntry(TKey _key, TValue _value, bool _error)
        {
            key = _key;
            value = _value;
            error = _error;
        }
    }

    public interface IArrayBasedSelector
    {
        void UpdateDictionaryKeys();
        bool HasUnlinkedKeys();
        void RemoveUnlinkedKeys();
        bool AreKeysInSameOrderAsCollection();
        void MatchCollectionOrder();
    }

    public abstract class ArrayBasedSelector<TKey, TValue> : AbstractArrayBasedSelector<TKey, TValue>
    {
        [SerializeField]
        public List<ArrayBasedSelectorEntry<string, TValue>> entries = new();
        public override List<ArrayBasedSelectorEntry<string, TValue>> Entries { get => entries; }
    }

    public abstract class ArrayBasedGenericSelector<TKey, TValue> : AbstractArrayBasedSelector<TKey, TValue>
    {
        [GenericField, SerializeReference]
        public List<ArrayBasedSelectorEntry<string, TValue>> entries = new();
        public override List<ArrayBasedSelectorEntry<string, TValue>> Entries { get => entries; }
    }

}
