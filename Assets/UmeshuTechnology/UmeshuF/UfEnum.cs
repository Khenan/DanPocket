using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Umeshu.Uf
{
    public static class UfEnum
    {
        public static T[] GetEnumArray<T>() where T : Enum => (T[])Enum.GetValues(typeof(T));

        public static int GetEnumIndex<T>(this T _value) where T : Enum
        {
            T[] _array = GetEnumArray<T>();
            for (int _i = 0; _i < _array.Length; _i++)
            {
                T _item = _array[_i];
                if (_item.Equals(_value)) return _i;
            }
            return 0;
        }

        public static List<T> GetEnumList<T>() where T : Enum => GetEnumArray<T>().ToList();

        public static void ExecuteMethodForEnumArray<T>(this Action<T> _method) where T : Enum
        {
            foreach (T _item in GetEnumArray<T>()) _method.Invoke(_item);
        }

        public static T GetEnumFromString<T>(this string _key) where T : Enum => TryGetEnumFromString<T>(_key, out T _value) ? _value : default;

        public static bool TryGetEnumFromString<T>(this string _key, out T _value) where T : Enum
        {
            _value = default;
            foreach (T _testEnum in GetEnumArray<T>())
            {
                if (_key.ToUpper() == _testEnum.ToString().ToUpper())
                {
                    _value = _testEnum;
                    return true;
                }
            }
            return false;
        }

        public static T GetRandomEnum<T>() where T : Enum => GetEnumArray<T>().RandomInside();

        public static string[] GetEnumStrings<T>() where T : Enum => GetEnumArray<T>().Convert<T, string>();
    }
}