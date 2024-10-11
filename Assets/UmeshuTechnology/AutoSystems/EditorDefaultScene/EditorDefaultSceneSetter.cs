#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using Debug = UnityEngine.Debug;

[InitializeOnLoad]
public class EditorDefaultSceneSetter
{
    static EditorDefaultSceneSetter()
    {
        string _pathOfFirstScene = EditorBuildSettings.scenes[0].path;
        SceneAsset _sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(_pathOfFirstScene);
        if (EditorSceneManager.playModeStartScene != _sceneAsset)
        {
            EditorSceneManager.playModeStartScene = _sceneAsset;
            Debug.Log(_pathOfFirstScene + " was set as default play mode scene");
        }
    }
}

#endif