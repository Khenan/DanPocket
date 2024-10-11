using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.USystem.Addressable;
using Umeshu.USystem.LoadingScreen;
using Umeshu.USystem.Pool;
using Umeshu.USystem.Scene;
using Umeshu.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Umeshu.USystem
{
    public abstract partial class UmeshuGameManager : GameSystem<UmeshuGameManager> // SCENE PROXIES
    {
        public Dictionary<string, GameModeSceneProxy> sceneProxies = new();

        private GameModeSceneProxy gameModeSceneProxyRef;

        private void InitGameModeSceneProxyRef()
        {
            gameModeSceneProxyRef = new GameObject("GameModeSceneProxyRef").AddComponent<GameModeSceneProxy>();
            gameModeSceneProxyRef.transform.SetParent(transform);
        }

        public void AddSceneToHierarchy(string _sceneName)
        {
            if (sceneProxies.ContainsKey(_sceneName))
            {
                Debug.LogError($"Scene proxy for {_sceneName} already exists");
                return;
            }

            GameModeSceneProxy _sceneProxy = PoolManager.GetObject(gameModeSceneProxyRef, this);
            sceneProxies.Add(_sceneName, _sceneProxy);
            _sceneProxy.SetAsSceneParent(SceneManager.GetSceneByName(_sceneName));
            _sceneProxy.InitObject(true);
        }

        public void StartScene(string _sceneName)
        {
            if (!sceneProxies.ContainsKey(_sceneName))
            {
                Debug.LogError($"Scene proxy for {_sceneName} does not exist");
                return;
            }

            sceneProxies[_sceneName].hierarchy.Play();
        }

        public void RemoveSceneFromHierarchy(string _sceneName)
        {
            if (!sceneProxies.ContainsKey(_sceneName))
            {
                Debug.LogError($"Scene proxy for {_sceneName} does not exist");
                return;
            }

            sceneProxies[_sceneName].LeaveScene();
            sceneProxies.Remove(_sceneName);
        }

        public void EnableScene(string _sceneName)
        {
            if (!sceneProxies.ContainsKey(_sceneName))
            {
                AddSceneToHierarchy(_sceneName);
            }
            
            for (int i = 0; i < sceneProxies[_sceneName].gameObject.transform.childCount; i++)
            {
                sceneProxies[_sceneName].gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }


            StartScene(_sceneName);
        }

        public void DisableScene(string _sceneName)
        {
            if (!sceneProxies.ContainsKey(_sceneName))
            {
                Debug.LogError($"Scene proxy for {_sceneName} does not exist");
                return;
            }

            sceneProxies[_sceneName].hierarchy.Stop();

            for (int i = 0; i < sceneProxies[_sceneName].gameObject.transform.childCount; i++)
            {
                sceneProxies[_sceneName].gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}