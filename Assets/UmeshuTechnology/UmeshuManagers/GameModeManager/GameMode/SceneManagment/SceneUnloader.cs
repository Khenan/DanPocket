using UnityEngine;
using UnityEngine.SceneManagement;

namespace Umeshu.USystem.Scene
{
    public class SceneUnloader : SceneOperator
    {
        protected override AsyncOperation StartSceneOperation(string _sceneName)
        {
            return SceneManager.UnloadSceneAsync(_sceneName);
        }
    }
}
