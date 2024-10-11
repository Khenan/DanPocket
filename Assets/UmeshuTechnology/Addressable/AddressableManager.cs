using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem.Pool;
using Umeshu.USystem.Scene;
using Umeshu.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace Umeshu.USystem.Addressable
{
    public sealed class AddressableManager : GameSystem<AddressableManager>, ILoader
    {

        #region System Methods

        protected override void SystemFirstInitialize() { }
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate() { }

        #endregion


        private float progress = 0;
        public float Progress => progress;
        public bool IsDone => progress == 1;
        public float VisualWeightOnLoading => 5;

        public UEvent<UPoolableAsset<PoolableGameElement>[]> onLoadFinished = new();

        private Dictionary<AssetLabelReference, List<AsyncOperationHandle<Object>>> currentPackages = new();
        private Dictionary<AssetLabelReference, List<AsyncOperationHandle<Sprite[]>>> currentSpriteSheetPackages = new();

        public static int GetKey(AssetLabelReference _assetLabelReference, Type _type) => $"{_assetLabelReference.RuntimeKey}->{_type.Name}".GetHashCode();
        public static readonly Dictionary<int, List<Object>> assets = new();
        public static Action<AssetLabelReference> onMainPackageLoad;
        public static Action<AssetLabelReference> onMainPackageUnload;
        public static Action<AssetLabelReference> onSpriteSheetPackageUnload;

        public T GetAsset<T>(AssetLabelReference _assetLabelReference, string _assetName, bool _logErrors = true) where T : Object => GetAsset<T>(_assetLabelReference, _assetName, typeof(T), _logErrors);
        public T GetAsset<T>(AssetLabelReference _assetLabelReference, string _assetName, Type _type, bool _logErrors = true) where T : Object
        {
            if (IsInvalidKey(_assetLabelReference, _type, _logErrors, out _)) return default;
            List<T> _list = GetAssets<T>(_assetLabelReference, _type, _logErrors);
            List<T> _filteredList = new();
            foreach (T _item in _list)
                if (_item != null && _item.name.Equals(_assetName))
                    _filteredList.Add(_item);
            if (_filteredList.Count > 0) return _filteredList[0];
            if (_logErrors) LogObjectNotFound(_assetName, _assetLabelReference, _type, _list);
            return default;
        }

        public List<T> GetAssets<T>(AssetLabelReference _assetLabelReference, bool _logErrors = true) where T : Object => GetAssets<T>(_assetLabelReference, typeof(T), _logErrors);
        public List<T> GetAssets<T>(AssetLabelReference _assetLabelReference, Type _type, bool _logErrors = true) where T : Object
        {
            if (IsInvalidKey(_assetLabelReference, _type, _logErrors, out int _key)) return default;
            List<Object> _list = assets[_key];
            List<T> _filteredList = new();
            foreach (Object _item in _list)
                if (_item != null && _item is T _value && _item.GetType() == _type)
                    _filteredList.Add(_value);
            if (_filteredList.Count > 0) return _filteredList;
            if (_logErrors) LogObjectNotFound(null, _assetLabelReference, _type, _list);
            return default;
        }

        public bool IsInvalidKey(AssetLabelReference _assetLabelReference, Type _type, bool _logErrors, out int _key)
        {
            _key = GetKey(_assetLabelReference, _type);
            if (string.IsNullOrEmpty(_assetLabelReference.labelString))
            {
                if (_logErrors) $"AssetLabelReference is null".LogError();
                return true;
            }
            if (!assets.ContainsKey(_key))
            {
                if (_logErrors) $"Couldnt find collection \"{_assetLabelReference.RuntimeKey}\" with key {_key}, collection is {assets.Keys.ToCollectionString()},".LogError();
                return true;
            }
            return false;
        }

        public void LogObjectNotFound<T>(string _assetName, AssetLabelReference _assetLabelReference, Type _type, List<T> _list)
        {
            if (_assetName == null) $"Couldnt find object of type \"{_type.Name}\" in collection \"{_assetLabelReference.RuntimeKey}\"".LogError();
            else $"Couldnt find object named \"{_assetName}\", of type \"{_type.Name}\" in collection \"{_assetLabelReference.RuntimeKey}\"".LogError();
            _list.LogErrorCollection();
        }

        public List<T> GetAllLoadedAssetsOfType<T>() where T : Object => GetAllLoadedAssetsOfType<T>(typeof(T));
        public List<T> GetAllLoadedAssetsOfType<T>(Type _type) where T : Object
        {
            List<T> _list = new();
            foreach (List<Object> _assets in assets.Values)
                _list.AddRange(_assets.ExtractList(_obj => _obj as T, _obj => _obj.GetType() == _type));
            return _list;
        }

        public IEnumerator LoadNonLoadedItems(GameModeInfo _gameModeInfo, AssetLabelReference[] _permanentPackages, int _nbItemPerLoad)
        {
            AssetLabelReference[] _possiblePackagesToAdd = _permanentPackages.GetMergedWith(_gameModeInfo.GetPackages());
            object[] _currentPackageRuntimeKeys = currentPackages.Keys.ExtractArray(_package => _package.RuntimeKey);
            List<AssetLabelReference> _packagesToAdd = _possiblePackagesToAdd.ExtractList(_ => _, _packageKey => !_currentPackageRuntimeKeys.Contains(_packageKey.RuntimeKey));
            yield return LoadAddressablePackages(_packagesToAdd.ToList(), _nbItemPerLoad);
            onLoadFinished?.Invoke(_gameModeInfo.GetPoolableGameElements());
        }
        public IEnumerator HandleAddressableLoading(GameModeInfo _gameModeInfo, AssetLabelReference[] _permanentPackages, int _nbItemPerLoad)
        {
            object[] _gameModeRuntimeKeys = _gameModeInfo.GetPackages().ExtractArray(_package => _package.RuntimeKey);
            object[] _permanentPackageRuntimeKeys = _permanentPackages.ExtractArray(_package => _package.RuntimeKey);

            ReleaseUnusedHandles(_gameModeRuntimeKeys, _permanentPackageRuntimeKeys, currentPackages, onMainPackageUnload, _addedToLog: " from main packages");
            ReleaseUnusedHandles(_gameModeRuntimeKeys, _permanentPackageRuntimeKeys, currentSpriteSheetPackages, onSpriteSheetPackageUnload, _addedToLog: " from spritesheet packages");

            foreach (List<Object> _assets in assets.Values)
                _assets.RemoveAll(_asset => _asset == null);

            PoolManager.ReleasePoolWithNoReferences();

            AssetLabelReference[] _possiblePackagesToAdd = _permanentPackages.GetMergedWith(_gameModeInfo.GetPackages());
            object[] _currentPackageRuntimeKeys = currentPackages.Keys.ExtractArray(_package => _package.RuntimeKey);
            List<AssetLabelReference> _packagesToAdd = _possiblePackagesToAdd.ExtractList(_ => _, _packageKey => !_currentPackageRuntimeKeys.Contains(_packageKey.RuntimeKey));
            yield return LoadAddressablePackages(_packagesToAdd, _nbItemPerLoad);
            onLoadFinished?.Invoke(_gameModeInfo.GetPoolableGameElements());
        }

        private static void ReleaseUnusedHandles<TObj>(object[] _gameModeRuntimeKeys, object[] _permanentPackageRuntimeKeys, Dictionary<AssetLabelReference, List<AsyncOperationHandle<TObj>>> _packageDictionary, Action<AssetLabelReference> _callBack, string _addedToLog)
        {
            foreach (AssetLabelReference _packageKey in _packageDictionary.Keys.ToArray())
            {
                if (!_gameModeRuntimeKeys.Contains(_packageKey.RuntimeKey) && !_permanentPackageRuntimeKeys.Contains(_packageKey.RuntimeKey))
                {
                    $"Start unload package \"{_packageKey.RuntimeKey.Bold()}\" {_addedToLog}".Log(Color.red, UmeshuGameManager.LOG_LOAD_PACKAGE_CATEGORY);
                    List<AsyncOperationHandle<TObj>> _handles = _packageDictionary[_packageKey];
                    foreach (AsyncOperationHandle<TObj> _handle in _handles)
                        Addressables.Release(_handle);
                    _packageDictionary.Remove(_packageKey);
                    $"End unload package \"{_packageKey.RuntimeKey.Bold()}\" {_addedToLog}".Log(Color.red, UmeshuGameManager.LOG_LOAD_PACKAGE_CATEGORY);
                    _callBack?.Invoke(_packageKey);
                }
            }
        }


        private IEnumerator LoadProcedure<TList, THandle>(List<TList> _inList, Func<TList, AsyncOperationHandle<THandle>> _methodToGetHandle, int _nbItemPerLoad, List<AsyncOperationHandle<THandle>> _handles, float _endProgress)
        {
            int _nbItemLoadingTotal = 0;
            int _nbItemCurrentlyLoading = 0;
            float _startProgress = progress;
            for (int _i = 0; _i < _inList.Count; _i++)
            {
                TList _object = _inList[_i];
                AsyncOperationHandle<THandle> _handle = _methodToGetHandle(_object);
                _handles.Add(_handle);

                _nbItemCurrentlyLoading++;
                _nbItemLoadingTotal++;
                if (_nbItemCurrentlyLoading >= _nbItemPerLoad || _i == _inList.Count - 1)
                {
                    foreach (AsyncOperationHandle<THandle> _handleToLoad in _handles)
                    {
                        while (!_handleToLoad.IsDone)
                            yield return new WaitForEndOfFrame();
                        SetProgress(_progress: Mathf.Lerp(_startProgress, _endProgress, (float)_nbItemLoadingTotal / _inList.Count));
                    }
                    _nbItemCurrentlyLoading = 0;
                }
            }
            SetProgress(_progress: _endProgress);
        }

        private IEnumerator LoadAddressablePackages(List<AssetLabelReference> _packages, int _nbItemPerLoad)
        {
            SetProgress(_progress: 0, _checkIfNotInferior: false);

            //TODO : creating a class tree is very expansive. Doing a class tree on UnityEngine.Object is like... REALLY expansive. 
            ClassTree _classTree = ClassTree.Create(typeof(Object));
            List<Type> _types = new();
            GetAllTypeFromNode(_types, _classTree.Nodes);
            _types = _types.GetInverted();

            float _progressPerPackage = .95f / _packages.Count;
            for (int _packageIndex = 0; _packageIndex < _packages.Count; _packageIndex++)
            {
                float _locationProgressEnd = _progressPerPackage * _packageIndex + _progressPerPackage * .33f;
                float _objectProgressEnd = _progressPerPackage * _packageIndex + _progressPerPackage * .75f;
                float _spriteProgressEnd = _progressPerPackage * _packageIndex + _progressPerPackage * 1;

                AssetLabelReference _package = _packages[_packageIndex];
                $"Start load package \"{_package.RuntimeKey.Bold()}\"".Log(Color.magenta, UmeshuGameManager.LOG_LOAD_PACKAGE_CATEGORY);

                List<string> _existingAdress = new();
                List<IResourceLocation> _filteredLocations = new();
                List<AsyncOperationHandle<IList<IResourceLocation>>> _locationHandles = new();

                yield return LoadProcedure(_types, (_type) => Addressables.LoadResourceLocationsAsync(_package, _type), _nbItemPerLoad, _locationHandles, _locationProgressEnd);

                foreach (AsyncOperationHandle<IList<IResourceLocation>> _locationHandle in _locationHandles)
                    foreach (IResourceLocation _location in _locationHandle.Result)
                    {
                        string _key = _location.ToString();
                        if (!_existingAdress.Contains(_key))
                        {
                            _existingAdress.Add(_key);
                            _filteredLocations.Add(_location);
                        }
                    }

                List<AsyncOperationHandle<Object>> _loadHandles = new();
                List<AsyncOperationHandle<Sprite[]>> _loadSpritesheetHandles = new();

                List<IResourceLocation> _objectLocations = new();
                List<IResourceLocation> _spriteLocations = new();
                foreach (IResourceLocation _filteredLocation in _filteredLocations)
                {
                    if (_filteredLocation.ResourceType == typeof(Sprite)) _spriteLocations.Add(_filteredLocation);
                    else _objectLocations.Add(_filteredLocation);
                }

                yield return LoadProcedure(_objectLocations, Addressables.LoadAssetAsync<Object>, _nbItemPerLoad, _loadHandles, _objectProgressEnd);
                yield return LoadProcedure(_spriteLocations, Addressables.LoadAssetAsync<Sprite[]>, _nbItemPerLoad, _loadSpritesheetHandles, _spriteProgressEnd);

                ProcessHandles(_package, _loadHandles, _loadSpritesheetHandles);

                $"Finished load package \"{_package.RuntimeKey.Bold()}\"".Log(Color.magenta, UmeshuGameManager.LOG_LOAD_PACKAGE_CATEGORY);
                onMainPackageLoad?.Invoke(_package);
            }
            SetProgress(_progress: 1);
        }

        private void SetProgress(float _progress, bool _checkIfNotInferior = true)
        {
            if (_progress < progress && _checkIfNotInferior) return;
            progress = _progress;
        }


        private void ProcessHandles(
            AssetLabelReference _package,
            List<AsyncOperationHandle<Object>> _loadHandles,
            List<AsyncOperationHandle<Sprite[]>> _loadSpritesheetHandles)
        {
            foreach (AsyncOperationHandle<Sprite[]> _loadSpritesheetHandle in _loadSpritesheetHandles)
            {
                foreach (Sprite _sprite in _loadSpritesheetHandle.Result)
                {
                    int _key = AddressableManager.GetKey(_package, typeof(Sprite));
                    AddressableManager.assets.AddObjectToListDictionnary(_key, _sprite, _canBeTwiceInList: false);
                    $"Loaded sprite named \"{_sprite.name.Bold()}\", in spritesheet from collection \"{_package.RuntimeKey.Bold()}\" with key \"{_key.Bold()}\"".Log(Color.yellow, UmeshuGameManager.LOG_LOAD_CATEGORY);
                }
                currentSpriteSheetPackages.AddObjectToListDictionnary(_package, _loadSpritesheetHandle, _canBeTwiceInList: false);
            }

            foreach (AsyncOperationHandle<Object> _loadHandle in _loadHandles)
            {
                int _key = AddressableManager.GetKey(_package, _loadHandle.Result.GetType());
                $"{_loadHandle.Result.GetType().Quote().Color(Color.cyan)} named {_loadHandle.Result.name.Quote().Color(Color.green)}".LogDesc($"Added in {_package.labelString.Quote().Color(Color.yellow)}", "Addressable Loading");
                AddressableManager.assets.AddObjectToListDictionnary(_key, _loadHandle.Result, _canBeTwiceInList: false);

                $"Loaded object named \"{_loadHandle.Result.name.Bold()}\", of type \"{_loadHandle.Result.GetType().Name.Bold()}\" in collection \"{_package.RuntimeKey.Bold()}\" with key \"{_key.Bold()}\"".Log(Color.green, UmeshuGameManager.LOG_LOAD_CATEGORY);
                currentPackages.AddObjectToListDictionnary(_package, _loadHandle, _canBeTwiceInList: false);
            }
        }

        private void GetAllTypeFromNode(List<Type> _types, IReadonlyNode<Type> _node)
        {
            _types.Add(_node.Value);
            foreach (IReadonlyNode<Type> _childNode in _node)
                GetAllTypeFromNode(_types, _childNode);
        }

    }

}