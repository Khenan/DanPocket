#if UNITY_EDITOR

using System;
using Umeshu.Uf;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using SceneManager = UnityEditor.SceneManagement.EditorSceneManager;
public static partial class SerializedPropertyUtil
{
    private const string EDITING_SCENE_NAME = "EditingEnvironnement";
    private static Scene scene;

    public static T CreateWorkingComponent<T>(GameObject _gameObject = null) where T : Component => ((_gameObject ?? new GameObject()).AddComponent<T>()).FlagAsWorking();
    public static T CreateWorkingScriptableObject<T>() where T : ScriptableObject => ScriptableObject.CreateInstance<T>().FlagAsWorking();
    public static void DoComponentInspectorLayout<T>(ref T _object) where T : Component => DoInspectorLayout(ref _object, () => CreateWorkingComponent<T>());
    public static void DoScriptableInspectorLayout<T>(ref T _object) where T : ScriptableObject => DoInspectorLayout(ref _object, () => CreateWorkingScriptableObject<T>());
    public static void DoInspectorLayout<T>(ref T _object, Func<T> _instantiator) where T : Object
    {
        if (_object == null)
        {
            if (_instantiator == null) return;
            _object = _instantiator.Invoke();
        }
        DoInspectorLayout(_object);
    }
    public static void DoInspectorLayout(this Object _object)
    {
        SerializedObject _serializedObject = new(_object);
        _serializedObject.Update();
        SerializedProperty _property = _serializedObject.GetIterator();
        _property.Next(true);
        _property.NextVisible(false); // skips the "Script" display
        while (_property.NextVisible(enterChildren: false)) EditorGUILayout.PropertyField(_property); //draw all the rest of the properties
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_object, $"Edit {_object.name}");
            _serializedObject.ApplyModifiedProperties();
        }
    }

    private static void DestroyWorkingCopy(Object _object)
    {
        if (!_object) return;
        if (_object is Component _component) _object = _component.gameObject;
        if (Application.isPlaying)
        {
            Object.Destroy(_object);
        }
        else
        {
            Object.DestroyImmediate(_object);
        }
    }
    /// <summary>
    /// Sets the object to "Hide and don"t save"
    /// </summary>
    public static T FlagAsWorking<T>(this T _object) where T : Object
    {
        _object.hideFlags = HideFlags.HideAndDontSave;
        return _object;
    }
    private static T CreateWorkingCopy<T>(this T _object) where T : Object
    {
        T _workingCopy = Object.Instantiate(_object);
        _workingCopy.FlagAsWorking();
        if (_workingCopy is Component _component)
        {
            _component.gameObject.hideFlags = HideFlags.HideAndDontSave;
        }
        return _workingCopy;
    }
    private static void MoveToEditingScene(Component _component, Scene _scene) => UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(_component.gameObject, _scene);
    private static void TryActOnScene(Action<Scene> _action)
    {
        Scene? _scene = GetEditingEnvironnement();
        if (_scene?.isLoaded ?? false)
        {
            _action?.Invoke(_scene.Value);
        }
    }

    private static Scene? GetEditingEnvironnement()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            scene = SceneManager.GetSceneByName(EDITING_SCENE_NAME);
            if (!scene.isLoaded)
            {
                scene = SceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                scene.name = EDITING_SCENE_NAME;
            }
            return scene;
        }
        return null;
    }

}

#endif