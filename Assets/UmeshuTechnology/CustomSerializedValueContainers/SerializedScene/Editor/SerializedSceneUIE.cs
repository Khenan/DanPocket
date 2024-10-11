#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializedScene), true)]
public class SerializedSceneUIE : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginProperty(_position, _label, _property);


        SerializedProperty _scenePathProperty = _property.FindPropertyRelative(nameof(SerializedScene.scenePath));
        SerializedProperty _sceneNameProperty = _property.FindPropertyRelative(nameof(SerializedScene.sceneName));
        SerializedProperty _sceneProperty = _property.FindPropertyRelative(nameof(SerializedScene.scene));

        _scenePathProperty.serializedObject.Update();

        string _scenePath = _scenePathProperty.stringValue;
        SceneAsset _oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_scenePath);
        if (_oldScene == null)
        {
            _oldScene = _sceneProperty.objectReferenceValue as SceneAsset;
            if (_oldScene != null)
            {
                string _newPath = AssetDatabase.GetAssetPath(_oldScene);
                _scenePathProperty.stringValue = _newPath;
                Debug.Log("SWITCH PATH TO " + _newPath);
                UpdateSceneName(_sceneNameProperty, _newPath);
            }
        }

        EditorGUI.BeginChangeCheck();

        SceneAsset _newScene = EditorGUI.ObjectField(_position, _label, _oldScene, typeof(SceneAsset), false) as SceneAsset;

        if (EditorGUI.EndChangeCheck())
        {
            _sceneProperty.objectReferenceValue = _newScene;

            string _newPath = AssetDatabase.GetAssetPath(_newScene);
            _scenePathProperty.stringValue = _newPath;
            UpdateSceneName(_sceneNameProperty, _newPath);
        }

        _scenePathProperty.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }

    private void UpdateSceneName(SerializedProperty _sceneNameProperty, string _newPath)
    {
        string _sceneName = "Undefined";
        string[] _scenePathSplited = _newPath.Split('/');
        if (_scenePathSplited.Length > 0) _sceneName = _scenePathSplited[^1].Replace(".unity", "");
        _sceneNameProperty.stringValue = _sceneName;
    }
}

#endif