using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;
using Umeshu.USystem.Addressable;
using Umeshu.USystem.LoadingScreen;
using Umeshu.USystem.Scene;
using UnityEngine;

namespace Umeshu.USystem
{
    public abstract partial class UmeshuGameManager : GameSystem<UmeshuGameManager> // GAME MODE MANAGER
    {
        private sealed class GameModeManager_AdditiveLoad : GameModeManager
        {
            public GameModeManager_AdditiveLoad()
            {
                throw new NotImplementedException("This exception is there to notify that you switched to the AdditiveLoad version of the GameModeManager. Have the reset methods been implemented across the project? If it has been implemented, you can remove this exception.");
            }
            private readonly List<GameModeKey> _loadedGameModes = new();
            protected override ILoader[] Loaders => new ILoader[] { sceneLoader, AddressableManager.Instance };
            private readonly SceneLoader sceneLoader = new();

            public override IEnumerator GoToGameMode(GameModeKey _targetGameMode)
            {
                if (_targetGameMode == GameMode) yield break;
                yield return LocalLoadGameMode(_targetGameMode);
            }
#nullable enable
            private IEnumerator LocalLoadGameMode(GameModeKey _targetGameMode)
            {
                yield return FadeIn(_targetGameMode, GameModesInfos.TransitionFadeInDuration);
                if (GameMode != null)
                {
                    Unload();
                }

                IEnumerable<string> newlyLoadedScenes;
                if (!_loadedGameModes.Contains(_targetGameMode))
                {
                    newlyLoadedScenes = InitialLoad(_targetGameMode);
                }
                else
                {
                    newlyLoadedScenes = Enumerable.Empty<string>();
                }
                yield return WaitForEndOfLoad();
                foreach (string scene in newlyLoadedScenes)
                {
                    Instance.AddSceneToHierarchy(scene);
                }
                yield return FadeOut(_targetGameMode, GameModesInfos.TransitionFadeOutDuration);
                EnableTarget(_targetGameMode);
            }
#nullable disable
            private void Unload()
            {
                List<string> currentGameModeScenes = GetGameModeLoadedScenes(GameMode);
                foreach (string scene in currentGameModeScenes)
                {
                    Instance.DisableScene(scene);
                }
            }

            private IEnumerable<string> InitialLoad(GameModeKey _targetGameMode)
            {
                _loadedGameModes.Add(_targetGameMode);
                List<string> alreadyLoadedScenes = GetGameModeLoadedScenes(_targetGameMode);
                IEnumerable<string> newScenes = StartOperationOverGameMode(sceneLoader, _targetGameMode, alreadyLoadedScenes, null);
                AddressableManager.Instance.StartCoroutine(AddressableManager.Instance.LoadNonLoadedItems(this[_targetGameMode], Instance.GetPermanentPackages(), Instance.nbItemPerLoad));
                return newScenes;
            }

            private void EnableTarget(GameModeKey _targetGameMode)
            {
                List<string> gameModeScenes = GetGameModeLoadedScenes(_targetGameMode);
                foreach (string scene in gameModeScenes)
                {
                    Instance.EnableScene(scene);
                }
            }
        }
    }
}