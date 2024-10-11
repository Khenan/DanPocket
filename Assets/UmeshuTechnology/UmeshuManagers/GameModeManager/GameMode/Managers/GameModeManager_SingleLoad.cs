using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem.Addressable;
using Umeshu.USystem.LoadingScreen;
using Umeshu.USystem.Scene;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Umeshu.USystem
{
    public abstract partial class UmeshuGameManager : GameSystem<UmeshuGameManager> // GAME MODE MANAGER
    {
        private class GameModeManager_SingleLoad : GameModeManager
        {
            private readonly SceneLoader sceneLoader = new();
            private readonly SceneUnloader sceneUnloader = new();
            protected override ILoader[] Loaders => new ILoader[] { sceneLoader, sceneUnloader, AddressableManager.Instance };
            
         

            public override IEnumerator GoToGameMode(GameModeKey _targetGameMode)
            {
                if (_targetGameMode == GameMode) yield break;
                yield return LocalLoadGameMode(_targetGameMode);
            }

            private IEnumerator LocalLoadGameMode(GameModeKey _targetGameMode)
            {
                yield return FadeIn(_targetGameMode, GameModesInfos.TransitionFadeInDuration);

                #region Body

                Color _logColor = new(1, 153f / 255f, 51f / 255f); // orange
                $"START TRANSI TO {_targetGameMode.ToString().ToUpper()}".Log(_logColor, LOG_TRANSITION_CATEGORY);

                List<string> _ignoreScenes = GetGameModeLoadedScenes(_targetGameMode);

                "START LOAD SCENE".Log(_logColor, LOG_TRANSITION_CATEGORY);

                if (GameMode != null) StartOperationOverGameMode(sceneUnloader, GameMode, _ignoreScenes, Instance.RemoveSceneFromHierarchy);
                IEnumerable<string> _loadedScenes = StartOperationOverGameMode(sceneLoader, _targetGameMode, _ignoreScenes, null);

                "START LOAD ADDRESSABLE".Log(_logColor, LOG_TRANSITION_CATEGORY);

                AddressableManager.Instance.StartCoroutine(AddressableManager.Instance.LoadNonLoadedItems(this[_targetGameMode], Instance.GetPermanentPackages(), Instance.nbItemPerLoad));

                "WAIT FOR END".Log(_logColor, LOG_TRANSITION_CATEGORY);

                for (int _i = 0; _i < SceneManager.loadedSceneCount; _i++)
                {
                    string _sceneName = SceneManager.GetSceneAt(_i).name;
                }

                yield return WaitForEndOfLoad();

                "DONE".Log(_logColor, LOG_TRANSITION_CATEGORY);

                #endregion

                #region Complete

                foreach (string _loadedScene in _loadedScenes)
                    Instance.AddSceneToHierarchy(_loadedScene);
                

                yield return FadeOut(_targetGameMode, GameModesInfos.TransitionFadeOutDuration);

                foreach (string _loadedScene in _loadedScenes)
                    Instance.StartScene(_loadedScene);
                

                #endregion
            }
        }
    }
}