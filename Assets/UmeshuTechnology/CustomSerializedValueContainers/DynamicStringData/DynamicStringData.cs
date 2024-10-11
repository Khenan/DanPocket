using System;
using System.Collections;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Umeshu.Common
{
    [System.Serializable]
    public class DynamicStringData<T> where T : DynamicStringDatabase
    {
        public string value = "";
        public static implicit operator string(DynamicStringData<T> _dynamicStringData) => _dynamicStringData.value;
    }
}