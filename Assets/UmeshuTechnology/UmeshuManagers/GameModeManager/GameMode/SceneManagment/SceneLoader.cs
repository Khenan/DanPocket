using UnityEngine;
using UnityEngine.SceneManagement;

namespace Umeshu.USystem.Scene
{
    public class SceneLoader : SceneOperator
    {
        protected override AsyncOperation StartSceneOperation(string _sceneName)
        {
            return SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        }
    }
}