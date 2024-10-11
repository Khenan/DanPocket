#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;

public static partial class SerializedPropertyUtil
{
    public class PropertyCopy
    {
        public PropertyCopy(SerializedProperty _property)
        {
            originalObject = _property.serializedObject;
            workingCopy = CreateWorkingCopy(originalObject.targetObject);
            propertyCopy = new SerializedObject(workingCopy).FindProperty(_property.propertyPath);
            PrefabStage.prefabStageOpened += OnPrefabStageChange;
            PrefabStage.prefabStageClosing += OnPrefabStageChange;
        }
        private SerializedObject originalObject;
        private Object workingCopy;
        private SerializedProperty propertyCopy;
        public SerializedObject Original => originalObject;
        public SerializedProperty OriginalProperty => originalObject?.FindProperty(propertyCopy?.propertyPath);
        public SerializedProperty Property => propertyCopy;
        public bool IsValid => originalObject != null && workingCopy && Property != null;
        public event Action onInvalidate;
        private void OnPrefabStageChange(PrefabStage _stage) => Close();
        public void Apply()
        {
            if (propertyCopy != null && originalObject != null)
                OriginalProperty.SetValue(propertyCopy.GetValue());
            Close();
        }
        public void Close()
        {
            DestroyWorkingCopy(workingCopy);
            PrefabStage.prefabStageOpened -= OnPrefabStageChange;
            PrefabStage.prefabStageClosing -= OnPrefabStageChange;
            onInvalidate?.Invoke();
        }
        ~PropertyCopy() => Close();
    }
}

#endif