using UnityEngine;

namespace Umeshu.USystem.Text
{
    using Umeshu.Uf;
    using Umeshu.USystem.TSV;

    //[CreateAssetMenu(fileName = "LocalizationData_Translations_SharedData", menuName = "ScriptableObjects/UmeshuTechnology/TsvBasedData/LocalizationData/LocalizationData_Translations_SharedData")]
    public class LocalizationData_Translations : TsvDatabase
    {
        internal string GetValueAtLanguage(string _key, LocalizationManager.Language _language) => GetValueAtLanguage(_key, _language.ToString());
        internal string GetValueAtLanguage(string _key, string _language)
        {
            string _errorReturnValue = $"Error on key {_key} at language {_language}";
            return
                UfLogger.LogErrorIfTrue(string.IsNullOrEmpty(_language), $"null or empty parameters - _language : {_language.Quote()}", _context: this) ? _errorReturnValue :
                UfLogger.LogErrorIfTrue(string.IsNullOrEmpty(_key), $"null or empty parameters - _key : {_key.Quote()}", _context: this) ? _errorReturnValue :
                UfLogger.LogErrorIfTrue(!data.ContainsKey(_key), $"key : {_key.Quote()} do not exist", _context: this) ? _errorReturnValue :
                UfLogger.LogErrorIfTrue(!data[_key].ContainsKey(_language), $"language {_language.Quote()} doesn't exist on key : {_key.Quote()}", _context: this) ? _errorReturnValue :
                string.IsNullOrEmpty(data[_key][_language]) ? $"The key {_key.Quote()} at language {_language.Quote()} is empty" :
                data[_key][_language];
        }
    }
}