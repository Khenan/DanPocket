using UnityEngine;

namespace Umeshu.USystem.Text
{
    using System.Collections.Generic;
    using TMPro;
    using Umeshu.Uf;
    using Umeshu.USystem.TSV;
    using Umeshu.Utility;

    //[CreateAssetMenu(fileName = "LocalizationData_InstanceStyleLinks_SharedData", menuName = "ScriptableObjects/UmeshuTechnology/TsvBasedData/LocalizationData/LocalizationData_InstanceStyleLinks_SharedData")]
    public class LocalizationData_InstanceStyleLinks : TsvDatabase
    {
        public string GetStyleLink(string _key)
        {
            if (!data.ContainsKey(_key))
            {
                if (!string.IsNullOrEmpty(_key)) UfLogger.LogError($"key : {_key.Quote()} do not exist");
                return "";
            }
            return data[_key]["Style"].RemoveWhiteSpace();
        }
    }
}