using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Umeshu.USystem
{
    public abstract class EnumBasedGameSystem<TClass, TEnum, TEnumClassKey> : GameSystem<TClass> where TClass : GameSystem<TClass> where TEnum : Enum where TEnumClassKey : EnumBasedKey, new()
    {
        private readonly Dictionary<TEnum, TEnumClassKey> enumToKey = new();
        private readonly Dictionary<TEnumClassKey, TEnum> keyToEnum = new();

        private bool dictionaryAreInitialized = false;

        public TEnum GetEnumFromKey(TEnumClassKey _key)
        {
            CheckDictionaries();
            return keyToEnum[_key];
        }

        public TEnumClassKey GetKeyFromEnum(TEnum _enum)
        {
            CheckDictionaries();
            return enumToKey[_enum];
        }

        public TEnum this[TEnumClassKey _key] => GetEnumFromKey(_key);
        public TEnumClassKey this[TEnum _enum] => GetKeyFromEnum(_enum);

        private void CheckDictionaries()
        {
            if (dictionaryAreInitialized) return;
            dictionaryAreInitialized = true;

            enumToKey.Clear();
            keyToEnum.Clear();
            foreach (TEnum _enumValue in UfEnum.GetEnumArray<TEnum>())
            {
                TEnumClassKey _key = new();
                _key.SetName(_enumValue.ToString());
                enumToKey.SetValue(_enumValue, _key);
                keyToEnum.SetValue(_key, _enumValue);
            }
        }
    }
}