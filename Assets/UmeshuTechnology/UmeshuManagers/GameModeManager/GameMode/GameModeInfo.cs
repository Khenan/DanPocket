using System;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Umeshu.USystem
{
    public class GameModeInfo : ScriptableObject
    {
        [Header("LOAD")]
        [SerializeField] private GameModeLoadInfos gameModeLoadInfo = new();
        public virtual string[] GetScenes() => gameModeLoadInfo.Scenes;
        public virtual AssetLabelReference[] GetPackages() => gameModeLoadInfo.packages;
        public virtual UPoolableAsset<PoolableGameElement>[] GetPoolableGameElements() => gameModeLoadInfo.poolableGameElements;

    }

    [Serializable]
    public class GameModeLoadInfos
    {
        public SerializedScene[] scenes;
        public AssetLabelReference[] packages;
        public UPoolableAsset<PoolableGameElement>[] poolableGameElements;

        public string[] Scenes => GetScenes();
        private string[] GetScenes() => scenes.ExtractArray(_serializedScene => _serializedScene.sceneName);
    }
}