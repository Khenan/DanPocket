using System;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.Utility
{

    [System.Serializable]
    public class EnumBasedSelector<TEnumKey, TValue> : ArrayBasedSelector<TEnumKey, TValue> where TEnumKey : Enum
    {
        public override TEnumKey[] GetCollection() => UfEnum.GetEnumArray<TEnumKey>();
        public override string ConvertKeyToString(TEnumKey _key) => _key.ToString();
    }

    [System.Serializable]
    public class EnumBasedGenericSelector<TEnumKey, TValue> : ArrayBasedGenericSelector<TEnumKey, TValue> where TEnumKey : Enum
    {
        public override TEnumKey[] GetCollection() => UfEnum.GetEnumArray<TEnumKey>();
        public override string ConvertKeyToString(TEnumKey _key) => _key.ToString();
    }

}
