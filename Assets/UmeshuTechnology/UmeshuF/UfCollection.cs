using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.LowLevel;

namespace Umeshu.Uf
{
    /// <summary>
    /// Provides utility methods for working with collections.
    /// </summary>
    public static class UfCollection
    {
        #region Shuffle
        /// <summary>
        /// Shuffles the elements of a list.
        /// </summary>
        public static List<T> GetShuffled<T>(this List<T> _original)
        {
            List<T> _shuffledList = _original.GetCopy();
            for (int _i = 0; _i < _shuffledList.Count; _i++)
            {
                T _temp = _shuffledList[_i];
                int _randomIndex = UnityEngine.Random.Range(_i, _shuffledList.Count);
                _shuffledList[_i] = _shuffledList[_randomIndex];
                _shuffledList[_randomIndex] = _temp;
            }
            return _shuffledList;
        }

        /// <summary>
        /// Shuffles the elements of an array.
        /// </summary>
        public static T[] GetShuffled<T>(this T[] _original) => _original.ToList().GetShuffled().ToArray();

        /// <summary>
        /// Shuffles a list based on a seed.
        /// </summary>
        public static List<T> ShuffleListFromSeed<T>(List<T> _original, List<int> _seed)
        {
            List<T> _shuffledList = new();
            for (int _i = 0; _i < _original.Count; _i++) { _shuffledList.Add(_original[_seed[_i]]); }
            return _shuffledList;
        }
        #endregion

        public static void RemoveRangeFrom<T>(this List<T> _list, int _index)
        {
            if (_index >= _list.Count) return;
            _list.RemoveRange(_index, _list.Count - _index);
        }

        /// <summary>
        /// Inverts the order of elements in a list.
        /// </summary>
        public static List<T> GetInverted<T>(this List<T> _original)
        {
            List<T> _invertedList = new();
            for (int _i = 0; _i < _original.Count; _i++)
            {
                _invertedList.Add(_original[_original.Count - 1 - _i]);
            }
            return _invertedList;
        }

        /// <summary>
        /// Adds an object to a list in a dictionary.
        /// </summary>
        public static void AddObjectToListDictionnary<T, U>(this Dictionary<T, List<U>> _dic, T _key, U _obj, bool _canBeTwiceInList)
        {
            if (_dic.ContainsKey(_key))
            {
                if (!_dic[_key].Contains(_obj) || _canBeTwiceInList) _dic[_key].Add(_obj);
            }
            else _dic.Add(_key, new() { _obj });
        }

        /// <summary>
        /// Removes an object from a list in a dictionary.
        /// </summary>
        public static void RemoveObjectToListDictionnary<T, U>(this Dictionary<T, List<U>> _dic, T _key, U _obj)
        {
            if (_dic.ContainsKey(_key)) _dic[_key].Remove(_obj);
        }

        /// <summary>
        /// Sets the value for a key in a dictionary.
        /// </summary>
        public static void SetValue<T, U>(this Dictionary<T, U> _dic, T _key, U _value)
        {
            if (!_dic.ContainsKey(_key)) _dic.Add(_key, _value);
            else _dic[_key] = _value;
        }

        public static bool AddValueIfNotExisting<T, U>(this Dictionary<T, U> _dic, T _key, U _value, bool _logIfAlreadyAdded)
        {
            bool _mustAddValue = !_dic.ContainsKey(_key);
            if (_mustAddValue) _dic.Add(_key, _value);
            else if (_logIfAlreadyAdded) UnityEngine.Debug.LogError($"Key {_key} already exists in dictionary");
            return _mustAddValue;
        }

        #region Copy
        /// <summary>
        /// Returns a copy of a list.
        /// </summary>
        public static List<T> GetCopy<T>(this List<T> _original) => new(_original);

        /// <summary>
        /// Returns a copy of an array.
        /// </summary>
        public static T[] GetCopy<T>(this T[] _original)
        {
            T[] _copiedArray = new T[_original.Length];
            _original.CopyTo(_copiedArray, 0);
            return _copiedArray;
        }
        #endregion

        #region Merge
        /// <summary>
        /// Merges an array with another array.
        /// </summary>
        public static T[] GetMergedWith<T>(this T[] _originalArray, T[] _otherArray) => MergeArrays(false, _originalArray, _otherArray);
        /// <summary>
        /// Merges multiple arrays into one.
        /// </summary>
        public static T[] MergeArrays<T>(params T[][] _arrays) => MergeArrays(false, _arrays);
        /// <summary>
        /// Merges multiple arrays into one, with an option to add only if non-existing.
        /// </summary>
        public static T[] MergeArrays<T>(bool _addOnlyIfNonExisting, params T[][] _arrays)
        {
            List<T> _returnedList = new();
            foreach (T[] _array in _arrays)
            {
                if (_array == null)
                {
                    continue;
                }

                foreach (T _item in _array)
                {
                    if (!_addOnlyIfNonExisting || !_returnedList.Contains(_item)) _returnedList.Add(_item);
                }
            }
            return _returnedList.ToArray();
        }

        /// <summary>
        /// Merges a list with another list.
        /// </summary>
        public static List<T> GetMergedWith<T>(this List<T> _originalList, List<T> _otherList) => MergeList(false, _originalList, _otherList);
        /// <summary>
        /// Merges multiple lists into one.
        /// </summary>
        public static List<T> MergeList<T>(params List<T>[] _lists) => MergeList(false, _lists);
        /// <summary>
        /// Merges multiple lists into one, with an option to add only if non-existing.
        /// </summary>
        public static List<T> MergeList<T>(bool _addOnlyIfNonExisting, params List<T>[] _lists)
        {
            List<T> _finalList = new();
            foreach (List<T> _list in _lists)
            {
                foreach (T _item in _list)
                {
                    if (!_addOnlyIfNonExisting || !_finalList.Contains(_item)) _finalList.Add(_item);
                }
            }
            return _finalList;
        }
        #endregion

        /// <summary>
        /// Filters a list by item name.
        /// </summary>
        public static List<T> FilterListItemByName<T>(List<T> _list, string _filter) where T : UnityEngine.Object
        {
            List<T> _finalList = new();
            foreach (T _item in _list)
            {
                if (_item.name.Contains(_filter)) _finalList.Add(_item);
            }
            return _finalList;
        }

        /// <summary>
        /// Executes a method on each item in an array.
        /// </summary>
        public static T[] DoMethod<T>(this T[] _array, Action<T> _action)
        {
            foreach (T _item in _array)
                _action.Invoke(_item);
            return _array;
        }

        /// <summary>
        /// Executes a method on each item in a list.
        /// </summary>
        public static List<T> DoMethod<T>(this List<T> _list, Action<T> _action)
        {
            _list.ToArray().DoMethod(_action);
            return _list;
        }

        /// <summary>
        /// Returns the index of an item in an array.
        /// </summary>
        public static int IndexOf<T>(this T[] _array, T _item)
        {
            for (int _index = 0; _index < _array.Length; _index++)
                if (_array[_index] != null && _array[_index].Equals(_item))
                    return _index;
            return -1;
        }

        /// <summary>
        /// Adds an item to a list if it's not already in the list.
        /// </summary>
        public static List<T> AddIfNotInside<T>(this List<T> _list, T _object)
        {
            if (!_list.Contains(_object)) _list.Add(_object);
            return _list;
        }

        #region Extract

        public static List<T2> ExtractList<T1, T2>(this IEnumerable<T1> _array, Func<T1, T2> _extractMethod, Predicate<T1> _validationMethod = null)
        {
            List<T2> _result = new();
            foreach (T1 _item in _array)
            {
                if (_validationMethod == null || _validationMethod.Invoke(_item))
                    _result.Add(_extractMethod.Invoke(_item));
            }
            return _result;
        }
        public static T2[] ExtractArray<T1, T2>(this IEnumerable<T1> _array, Func<T1, T2> _extractMethod, Predicate<T1> _validationMethod = null)
            => _array.ExtractList(_extractMethod, _validationMethod).ToArray();

        #endregion

        public static void ForEach<T>(this T[] _array, Action<T> _action)
        {
            if (_array == null) return;
            for (int _index = 0; _index < _array.Length; _index++)
                _action.Invoke(_array[_index]);
        }

#if UNITY_EDITOR
        public static bool ChecksConditionWithWindowLog<T>(this IEnumerable<T> _collection, Func<T, bool> _condition, string _errorMsg)
        {
            bool _successfulCondition = _collection.All(_condition);
            if (!_successfulCondition) _errorMsg.LogErrorWindow();
            return _successfulCondition;
        }

#endif

        public static bool ChecksConditionWithLog<T>(this IEnumerable<T> _collection, Func<T, bool> _condition, string _errorMsg)
        {
            bool _successfulCondition = _collection.All(_condition);
            if (!_successfulCondition) _errorMsg.LogError();
            return _successfulCondition;
        }

        public static T[] GetArrayWithInserted<T>(this T[] _array, int _index, T _item)
        {
            T[] _values = new T[_array.Length + 1];
            for (int _i = 0; _i < _values.Length; _i++) _values[_i] = _i < _index ? _array[_i] : _i == _index ? _item : _array[_i - 1];
            return _values;
        }

        public static void AdjustListSizeToAnother<T>(List<T> _original, List<T> _copy) where T : new() => AdjustListSizeToAnother(_original, _copy, () => new T());
        public static void AdjustListSizeToAnother<T>(List<T> _original, List<T> _copy, Func<T> _createObjectMethod)
        {
            while (_copy.Count < _original.Count)
                _copy.Add(_createObjectMethod());
            while (_copy.Count > _original.Count)
                _copy.RemoveAt(_copy.Count - 1);
        }
    }
}
