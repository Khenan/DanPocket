using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Umeshu.Uf
{
    public static class UfSave
    {
#if PLATFORM_SWITCH
                return;
#else
        private static readonly bool debug = false;
        private static readonly bool encrypt = false;
        private static string PersistentDataPath => Application.persistentDataPath;
        public static void Save(string _path, string _id, object _file, Formatting _formatting)
        {
            string _fileString = JsonConvert.SerializeObject(_file, _formatting);
            if (debug) Debug.Log("File Saved = " + _fileString);
            if (encrypt) _fileString = EncryptDecrypt(_fileString);

            string _completePath = PersistentDataPath + "/" + _path;
            Directory.CreateDirectory(_completePath);
            try
            {
                FileStream _stream = new(_completePath + "/" + _id + ".json", FileMode.Create);
                StreamWriter _writer = new(_stream);
                using (_writer)
                {
                    _writer.Write(_fileString);
                }
                _stream.Close();
                _writer.Close();
            }
            catch (Exception _e)
            {
                if (debug) Debug.LogError($"Erreur lors de la sauvegarde ! Error : {_e.Message}");
            }
        }

        public static T Load<T>(string _path, string _id)
        {
            _path.Log("SaveFile");
            string _data = GetFileFromPersistent(_path, _id);
            if (_data != "")
            {
                if (encrypt) _data = EncryptDecrypt(_data);
                if (debug) Debug.Log("Data = " + _data);
                T _returnedValue = JsonConvert.DeserializeObject<T>(_data);
                if (!_returnedValue.Equals(default(T)))
                {
                    if (debug) Debug.Log("Found in persistent data path");
                    return _returnedValue;
                }
                else if (debug) Debug.Log("Failed to cast");
            }
            return default;
        }

        static string GetFileFromPersistent(string _path, string _id)
        {
            string _json = "";
            string _persistentPath = "/" + _path;
            string _truePath = Application.persistentDataPath + _persistentPath + "/" + _id + ".json";
            _truePath.Log("SaveFile");
            if (debug) Debug.Log("File exist in path = " + debug);
            if (File.Exists(_truePath))
            {
                StreamReader _reader = new(_truePath);
                using (_reader) { _json = _reader.ReadToEnd(); }
                _reader.Close();
                if (debug) Debug.Log("File = " + _json);
            }
            return _json;
        }

        private const int ENCRYPT_KEY = 129;
        static string EncryptDecrypt(string _textToEncrypt)
        {
            StringBuilder _inSb = new(_textToEncrypt);
            StringBuilder _outSb = new(_textToEncrypt.Length);
            for (int _i = 0; _i < _textToEncrypt.Length; _i++)
            {
                char _c = _inSb[_i];
                _c = (char)(_c ^ ENCRYPT_KEY);
                _outSb.Append(_c);
            }
            return _outSb.ToString();
        }

#if UNITY_EDITOR
        [MenuItem("Save Folder/OPEN")]
        public static void OpenSaveFolder()
        {
            string _itemPath = Application.persistentDataPath.Replace(@"/", @"\");
            System.Diagnostics.Process.Start(_itemPath + "/Save/Editor");
        }
        [MenuItem("Save Folder/DELETE/Editor Save")]
        public static void DeleteEditorSave()
        {
            string _itemPath = Application.persistentDataPath.Replace(@"/", @"\");
            File.Delete(_itemPath + "/Save/Editor/GameData.json");
        }
        [MenuItem("Save Folder/DELETE/Player Save")]
        public static void DeletePlayerSave()
        {
            string _itemPath = Application.persistentDataPath.Replace(@"/", @"\");
            File.Delete(_itemPath + "/Save/WindowsPlayer/GameData.json");
        }
        [MenuItem("Save Folder/DELETE/All Saves")]
        public static void DeleteAllSaves()
        {
            DeleteEditorSave();
            DeletePlayerSave();
        }
#endif

#endif

    }
}
