using UnityEngine;

namespace Umeshu.USystem.Text
{
    using Umeshu.Uf;

    public class LocalizationData : ScriptableObject
    {
        public LocalizationData_Translations localizationDataTranslations;
        public LocalizationData_FontStyles localizationDataFontStyles;
        public LocalizationData_InstanceStyleLinks localizationDataInstanceStyleLinks;

        internal string GetValueAtLanguage(string _key, string _language)
        {
            string _errorReturnValue = $"Error from localization data scriptable";
            return
                UfLogger.LogErrorIfTrue(localizationDataTranslations == null, $"{nameof(localizationDataTranslations)} is null in {nameof(LocalizationData)} scriptable object", _context: this) ? _errorReturnValue :
               localizationDataTranslations.GetValueAtLanguage(_key, _language).Replace("\\n", "\n"); ;
        }

        internal void UpdateMaterialsInStyle() => localizationDataFontStyles.UpdateMaterialsInStyle();
        internal LocalizationData_FontStyles.TextStyle GetTextStyleFromStyleKey(string _styleKey) => localizationDataFontStyles.GetTextStyle(_styleKey);
        internal LocalizationData_FontStyles.TextStyle GetTextStyleFromInstanceKey(string _instanceKey) => localizationDataFontStyles.GetTextStyle(localizationDataInstanceStyleLinks.GetStyleLink(_instanceKey));

        internal string GetStyleLink(string _styleKey) => localizationDataInstanceStyleLinks.GetStyleLink(_styleKey);

        internal string ApplyStyleToTextFromStyleKey(string _text, string _styleKey) => localizationDataFontStyles.ApplyStyleToTextFromStyleKey(_text, _styleKey);
        internal string ApplyStyleToTextFromInstanceKey(string _text, string _instanceKey) => localizationDataFontStyles.ApplyStyleToTextFromStyleKey(_text, localizationDataInstanceStyleLinks.GetStyleLink(_instanceKey));

        internal string GetValueAtLanguage_WithStyle(string _key, string _language, string _instanceKey)
        {
            string _text = GetValueAtLanguage(_key, _language);
            string _styleKey = GetStyleLink(_instanceKey);
            string _stylisedText = ApplyStyleToTextFromStyleKey(_text, _styleKey);
            return _stylisedText;
        }
    }
}

#if UNITY_EDITOR
namespace Umeshu.USystem.Text
{
    using Umeshu.Uf;
    using Umeshu.USystem.TSV;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(LocalizationData))]
    public class LocalizationDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            LocalizationData _localizationData = (LocalizationData)target;

            if (_localizationData.localizationDataTranslations == null || _localizationData.localizationDataFontStyles == null || _localizationData.localizationDataInstanceStyleLinks == null)
            {
                GUI.color = Color.red.With(_g: .3f, _b: .3f);
                EditorGUILayout.HelpBox("One or more scriptables are missing", MessageType.Warning);
                if (GUILayout.Button("Create missing scriptables"))
                {
                    // Get the path to the folder contains the scriptable object
                    string _folderPath = AssetDatabase.GetAssetPath(target).Replace(target.name + ".asset", "");
                    if (_localizationData.localizationDataTranslations == null) _localizationData.localizationDataTranslations = UfEditor.CreateScritableObjectInProject<LocalizationData_Translations>(_folderPath);
                    if (_localizationData.localizationDataFontStyles == null) _localizationData.localizationDataFontStyles = UfEditor.CreateScritableObjectInProject<LocalizationData_FontStyles>(_folderPath);
                    if (_localizationData.localizationDataInstanceStyleLinks == null) _localizationData.localizationDataInstanceStyleLinks = UfEditor.CreateScritableObjectInProject<LocalizationData_InstanceStyleLinks>(_folderPath);
                }
                return;
            }

            // check if the fileNameFilter is set on each of the scriptables
            if (_localizationData.localizationDataTranslations.fileNameFilter != "Translations" || _localizationData.localizationDataFontStyles.fileNameFilter != "FontStyles" || _localizationData.localizationDataInstanceStyleLinks.fileNameFilter != "InstanceStyleLinks")
            {
                GUI.color = Color.red.With(_g: .3f, _b: .3f);
                EditorGUILayout.HelpBox("fileNameFilter is not set on each of the scriptables", MessageType.Warning);
                if (GUILayout.Button("Set fileNameFilter on each of the scriptables"))
                {
                    _localizationData.localizationDataTranslations.fileNameFilter = "Translations";
                    _localizationData.localizationDataFontStyles.fileNameFilter = "FontStyles";
                    _localizationData.localizationDataInstanceStyleLinks.fileNameFilter = "InstanceStyleLinks";
                    EditorUtility.SetDirty(_localizationData.localizationDataTranslations);
                    EditorUtility.SetDirty(_localizationData.localizationDataFontStyles);
                    EditorUtility.SetDirty(_localizationData.localizationDataInstanceStyleLinks);
                }
                return;
            }

            if (GUILayout.Button(nameof(TsvDatabaseEditor.UpdateAllTSVDatabase)))
                if (EditorUtility.DisplayDialog(nameof(TsvDatabaseEditor.UpdateAllTSVDatabase), "Are you sure you want to update all TSV databases?", "Yes", "No"))
                    TsvDatabaseEditor.UpdateAllTSVDatabase();

            GUILayout.BeginHorizontal();
            if (_localizationData.localizationDataTranslations != null && GUILayout.Button("Open Translations")) Selection.activeObject = _localizationData.localizationDataTranslations;
            if (_localizationData.localizationDataFontStyles != null && GUILayout.Button("Open Font Styles")) Selection.activeObject = _localizationData.localizationDataFontStyles;
            if (_localizationData.localizationDataInstanceStyleLinks != null && GUILayout.Button("Open Instance Style Links")) Selection.activeObject = _localizationData.localizationDataInstanceStyleLinks;
            GUILayout.EndHorizontal();
        }
    }
}
#endif