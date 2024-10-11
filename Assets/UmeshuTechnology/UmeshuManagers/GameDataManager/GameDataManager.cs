using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.USystem.GameData
{
    using Application = UnityEngine.Application;

    public sealed class GameDataManager : GameSystem<GameDataManager>
    {
        public static Dictionary<string, IRunTimeGameData> runtimeGameData = new();
        public static Dictionary<string, ISaveableGameData> saveableGameData = new();
        public static string SavePath => Path.Combine("Save", Application.isEditor ? "Editor" : Application.platform.ToString());
        public static string SaveFileName => "GameData";

        public static bool loadedExistingData = false;


        #region Game Element Methods

        protected override void SystemFirstInitialize() => LoadSaveableGameDatas();
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate() { }

        #endregion

        public override void OnEnterGameMode(GameModeKey _gameMode)
        {
            base.OnEnterGameMode(_gameMode);
            SaveSaveableGameDatas();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SaveSaveableGameDatas();
        }

        private bool LoadSaveableGameDatas()
        {
            List<SavedData> _loadedData = UfSave.Load<List<SavedData>>(SavePath, SaveFileName);
            $"Trying to load saved data from {SavePath}/{SaveFileName}, result is {_loadedData.ToNullableString()}".Log("SaveFile");
            loadedExistingData = true;
            if (_loadedData == null) return false;
            foreach (SavedData _data in _loadedData)
            {
                object _object = JsonConvert.DeserializeObject(_data.data, _data.type);
                if (_object is ISaveableGameData _saveableData)
                    saveableGameData.AddValueIfNotExisting(_saveableData.GetType().FullName, _saveableData, false);
            }
            _loadedData.LogCollection("SaveFile");
            return true;
        }

        private void SaveSaveableGameDatas()
        {
            if (!loadedExistingData) return;

            List<SavedData> _savedDatas = new();
            foreach (ISaveableGameData _saveableData in saveableGameData.Values)
                _savedDatas.Add(new(_saveableData.GetType(), JsonConvert.SerializeObject(_saveableData)));

            UfSave.Save(SavePath, SaveFileName, _savedDatas, Application.isEditor ? Formatting.Indented : Formatting.None);
        }

        #region Get/Set data in runtime

        public static T GetData<T>(T _defaultValue = null) where T : class, new()
        {
            string _key = typeof(T).FullName;
            return GetData(_key, _defaultValue);
        }

        private static T GetData<T>(string _key, T _defaultValue = null, bool _returnSomething = true, bool _logIfAlreadyAdded = false) where T : class, new()
        {
            _defaultValue ??= new();
            if (_defaultValue is ISaveableGameData _saveableGameData)
            {
                if (UfLogger.LogErrorIfTrue(!loadedExistingData, $"Trying to get data of type {typeof(T).Name.Quote()} before loading existing data")) return null;
                if (saveableGameData.AddValueIfNotExisting(_key, _saveableGameData, false))
                    $"GameData of type {_key} has been added".Log();
            }
            else runtimeGameData.AddValueIfNotExisting(_key, new RuntimeGameData<T>(_defaultValue), _logIfAlreadyAdded);
            return _returnSomething ? GetExistingData<T>() : null;
        }

        private static T GetExistingData<T>() where T : class, new()
        {
            string _key = typeof(T).FullName;
            if (typeof(T).ImplementInterface(typeof(ISaveableGameData)))
            {
                if (!saveableGameData.ContainsKey(_key))
                {
                    $"GameData of type {_key} not found".LogError();
                    return null;
                }
                return saveableGameData[_key] as T;
            }
            else
            {
                if (!runtimeGameData.ContainsKey(_key))
                {
                    $"GameData of type {_key} not found".LogError();
                    return null;
                }
                else return runtimeGameData[_key] is RuntimeGameData<T> _runtimeGameData ? _runtimeGameData.data : null;
            }
        }

        #endregion

        #region Classes

        public interface IRunTimeGameData
        {
            bool TryGetSaveable(out ISaveableGameData _saveableData);
        }
        public sealed class RuntimeGameData<T> : IRunTimeGameData where T : class
        {
            public T data;
            public RuntimeGameData(T _data) { data = _data; }

            public bool TryGetSaveable(out ISaveableGameData _saveableData)
            {
                if (data is ISaveableGameData _saveable)
                {
                    _saveableData = _saveable;
                    return true;
                }
                _saveableData = null;
                return false;
            }
        }

        [System.Serializable]
        public class SavedData
        {
            public Type type;
            public string data;

            public SavedData(Type _type, string _data)
            {
                type = _type;
                data = _data;
            }

            public override string ToString() => $"{"Data type :".Bold()} {type.Name.Size(10)}, {"Data :".Bold()} {data.Size(10)}";
        }


        #endregion


#if UNITY_EDITOR
        public static bool ForceReloadSaveableGameDatas() => Instance.LoadSaveableGameDatas();
#endif

    }

    public interface ISaveableGameData
    {
    }
}