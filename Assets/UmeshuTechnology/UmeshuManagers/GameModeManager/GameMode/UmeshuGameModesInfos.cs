using System;
using System.Collections.Generic;
using Umeshu.Uf;
using Umeshu.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Umeshu.USystem
{
    public abstract class UmeshuGameModesInfos<TGameMode> : IGameModesInfos where TGameMode : Enum
    {
        [SerializeField] protected EnumBasedSelector<TGameMode, GameModeInfo> infos;
        [SerializeField] protected AssetLabelReference[] permanentPackages;
        [SerializeField] protected TGameMode startGameMode;
        [Header("Loading Screen Datas")]
        [SerializeField] protected SerializedScene defaultLoadingScene;
        [SerializeField] protected float loadingFadeInDuration = 1;
        [SerializeField] protected float loadingFadeOutDuration = 1;
        private readonly Dictionary<TGameMode, GameModeKey> gameModeKeys = new();
        private readonly Dictionary<GameModeKey, TGameMode> gameModeIDs = new();

        public GameModeKey StartGameMode => gameModeKeys[startGameMode];
        public EnumBasedSelector<TGameMode, GameModeInfo> Infos => infos;
        public GameModeInfo this[GameModeKey _gameModeKey] => infos[GetGameModeID(_gameModeKey)];
        public GameModeInfo this[TGameMode _gameModeID] => infos[_gameModeID];
        public string TransitionSceneName => defaultLoadingScene != null ? defaultLoadingScene.sceneName : "";
        public float TransitionFadeInDuration => loadingFadeInDuration;
        public float TransitionFadeOutDuration => loadingFadeOutDuration;
        public TGameMode GetGameModeID(GameModeKey _gameModeKey) => gameModeIDs[_gameModeKey];
        public GameModeKey GetGameModeKey(TGameMode _gameModeID) => gameModeKeys[_gameModeID];
        public AssetLabelReference[] GetPermanentPackages() => permanentPackages;

        public void CreateGameModeKeys()
        {
            gameModeKeys.Clear();
            gameModeIDs.Clear();
            foreach (TGameMode _enumValue in UfEnum.GetEnumArray<TGameMode>())
            {
                GameModeKey _gameModeKey = new(_enumValue.ToString());
                gameModeKeys.SetValue(_enumValue, _gameModeKey);
                gameModeIDs.SetValue(_gameModeKey, _enumValue);
            }
        }
    }
}