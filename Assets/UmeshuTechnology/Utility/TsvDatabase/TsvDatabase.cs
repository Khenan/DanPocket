using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umeshu.Uf;
using Umeshu.Utility;
using UnityEngine;

namespace Umeshu.USystem.TSV
{
    [CreateAssetMenu(fileName = "Database", menuName = "ScriptableObjects/UmeshuTechnology/TsvBasedData/Database")]
    public class TsvDatabase : ScriptableObject, IPickableStringDatabase
    {
        public string fileNameFilter = "";
        [HideInInspector] public SerializedDictionary<string, TsvDatabase_File> serializedDataByFile = new();
        [HideInInspector] public SerializedDictionary<string, SerializedDictionary<string, string>> data = new();

#if UNITY_EDITOR
        public static void TsvGotImported(TextAsset _textAsset, string _assetName)
        {
            List<TsvDatabase> _scriptableDatas = UfEditor.GetAllAssetsOfType<TsvDatabase>();
            bool _gotData = false;
            foreach (TsvDatabase _scriptableData in _scriptableDatas)
                _gotData = _gotData || _scriptableData.TryGetDataFromTSV(_textAsset, _assetName);

            if (!_gotData) ("No data scriptable was found for " + _assetName).LogError();
        }
#endif

        private bool TryGetDataFromTSV(TextAsset _textAsset, string _assetName)
        {
            if (fileNameFilter == "" || !_assetName.RemoveWhiteSpace().ToLower().Contains(fileNameFilter.RemoveWhiteSpace().ToLower())) return false;

            Dictionary<string, Dictionary<string, string>> _tsvData = UfText.TreatTSV(_textAsset.text, _linePreventer: "//", _removeWhiteSpaceOnKeys: true);

#if UNITY_EDITOR
            $"Imported TSV: {_assetName.Quote().Bold().Color(Color.green)} ({_textAsset.GetHyperLink()}) on data named {name.Quote().Bold().Color(Color.green)} ({this.GetHyperLink()}) - {this.GetHyperLink()}".Log();

            UnityEditor.EditorUtility.SetDirty(this);

            async void SaveData()
            {
                await Task.Delay(1);
                SerializeData(_assetName, _tsvData);
                HandleData(_tsvData);
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            SaveData();
#else
            SerializeData(_assetName, _tsvData);
            HandleData(_tsvData);
#endif

            return true;
        }

        private void SerializeData(string _assetName, Dictionary<string, Dictionary<string, string>> _tsvData)
        {
            serializedDataByFile ??= new SerializedDictionary<string, TsvDatabase_File>();
            data ??= new SerializedDictionary<string, SerializedDictionary<string, string>>();
#if UNITY_EDITOR
            if (serializedDataByFile.ContainsKey(_assetName)) serializedDataByFile[_assetName].data.Clear();
            else serializedDataByFile.Add(_assetName, this.AddSubScriptableObject<TsvDatabase_File>(_assetName));
#else
            if (serializedDataByFile.ContainsKey(_assetName)) serializedDataByFile[_assetName].data.Clear();
            else serializedDataByFile.Add(_assetName, CreateInstance<TsvDatabase_File>());
#endif

            foreach (KeyValuePair<string, Dictionary<string, string>> _entry in _tsvData)
            {
                SerializedDictionary<string, string> _languageEntries = new();
                foreach (KeyValuePair<string, string> _languageEntry in _entry.Value)
                    _languageEntries.Add(_languageEntry.Key, _languageEntry.Value);
                serializedDataByFile[_assetName].data.Add(_entry.Key, _languageEntries);
            }

            data.Clear();
            foreach (TsvDatabase_File _fileData in serializedDataByFile.Values)
                foreach (KeyValuePair<string, SerializedDictionary<string, string>> _entry in _fileData.data)
                {
                    if (!data.ContainsKey(_entry.Key)) data.Add(_entry.Key, _entry.Value);
                    else $"Key {_entry.Key} already exists".LogError();
                }

#if UNITY_EDITOR
            data.Serialize();
            serializedDataByFile.Serialize();
            foreach (TsvDatabase_File _file in serializedDataByFile.Values)
            {
                _file.data.Serialize();
                foreach (SerializedDictionary<string, string> _fileDataEntryDic in _file.data.Values)
                    _fileDataEntryDic.Serialize();
            }
#endif
        }


        protected virtual void HandleData(Dictionary<string, Dictionary<string, string>> _tsvData) { }

        public string[] GetCollection() => data.Keys.ToArray();
    }
}