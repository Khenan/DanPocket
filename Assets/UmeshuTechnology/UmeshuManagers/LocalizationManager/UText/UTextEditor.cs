#if UNITY_EDITOR

using System;
using Umeshu.Uf;
using Umeshu.USystem.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UText<>), true)]
public class UTextEditor : Editor
{
    private LocalizationData localizationData;
    private LocalizationManager.Language currentLanguage;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        localizationData ??= UfEditor.GetAssetOfType<LocalizationData>();
        if (localizationData == null)
        {
            EditorGUILayout.HelpBox("No LocalizationData found in the project", MessageType.Error);
            return;
        }

        IUTextKeyGetter _localizedTextKeyGetter = target as IUTextKeyGetter;

        GUILayout.BeginHorizontal();

        Color _color = GUI.color;

        GUI.color = Color.yellow;
        SerializedProperty _instanceProperty = serializedObject.FindProperty("instance");
        EditorGUILayout.LabelField("Instance:", GUILayout.MaxWidth(60));
        EditorGUILayout.PropertyField(_instanceProperty, new(""), true, GUILayout.MinWidth(100));

        GUI.color = Color.green.With(_r: .5f, _b: .5f);
        SerializedProperty _keyProperty = serializedObject.FindProperty("key");
        EditorGUILayout.LabelField("Key:", GUILayout.MaxWidth(30));
        EditorGUILayout.PropertyField(_keyProperty, new(""), true, GUILayout.MinWidth(100));

        GUI.color = Color.red.With(_g: .8f, _b: .5f);
        currentLanguage = PopUp(currentLanguage, (_index, _collection) => EditorGUILayout.Popup(_index, _collection, GUILayout.MaxWidth(60)));

        GUI.color = _color;
        if (GUILayout.Button("Set"))
        {
            PickableString<LocalizationData_InstanceStyleLinks> _pickableString = _instanceProperty.GetValue() as PickableString<LocalizationData_InstanceStyleLinks>;
            LocalizationData_FontStyles.TextStyle _textStyle = localizationData.GetTextStyleFromInstanceKey(_pickableString.value);
            (target as IUText).SetTextFromEditor(localizationData.GetValueAtLanguage_WithStyle(_localizedTextKeyGetter.Key, currentLanguage.ToString(), _pickableString.value), _textStyle);
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Set EN"))
        {
            PickableString<LocalizationData_InstanceStyleLinks> _pickableString = _instanceProperty.GetValue() as PickableString<LocalizationData_InstanceStyleLinks>;
            LocalizationData_FontStyles.TextStyle _textStyle = localizationData.GetTextStyleFromInstanceKey(_pickableString.value);
            currentLanguage = LocalizationManager.Language.en;
            (target as IUText).SetTextFromEditor(localizationData.GetValueAtLanguage_WithStyle(_localizedTextKeyGetter.Key, LocalizationManager.Language.en.ToString(), _pickableString.value), _textStyle);
            EditorUtility.SetDirty(target);
        }

        GUI.color = _color;
        GUIStyle _style = new("BoldLabel") { normal = { textColor = (Color.white * .75f).WithAlpha(1) } };
        EditorGUILayout.LabelField(localizationData.GetValueAtLanguage(_localizedTextKeyGetter.Key, currentLanguage.ToString()), _style);

        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideText"), true);
        serializedObject.ApplyModifiedProperties();

    }

    private static T PopUp<T>(T _existingValue, Func<int, string[], int> _displayMethod) where T : Enum => PopUp(_existingValue.ToString(), UfEnum.GetEnumStrings<T>(), _displayMethod).GetEnumFromString<T>();

    private static string PopUp(string _existingValue, string[] _collection, Func<int, string[], int> _displayMethod)
    {
        int _choiceIndex = _collection.IndexOf(_existingValue);
        if (_choiceIndex > -1)
        {
            _choiceIndex = _displayMethod.Invoke(_choiceIndex, _collection);
            return _collection[_choiceIndex];
        }
        return _existingValue;
    }
}

#endif