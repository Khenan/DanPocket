#if UNITY_EDITOR

using Umeshu.Uf;
using UnityEditor;
using UnityEngine;

namespace Umeshu.USystem.TSV
{

    [CustomEditor(typeof(TsvDatabase), true)]
    public class TsvDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            TsvDatabase _scriptable = (TsvDatabase)target;


            if (GUILayout.Button("Clear"))
                if (EditorUtility.DisplayDialog("Clear", "Are you sure you want to clear this database?", "Yes", "No"))
                    Clear(_scriptable);

            if (GUILayout.Button(nameof(UpdateAllTSVDatabase)))
                if (EditorUtility.DisplayDialog(nameof(UpdateAllTSVDatabase), "Are you sure you want to update all TSV databases?", "Yes", "No"))
                    UpdateAllTSVDatabase();
        }

        private static void Clear(TsvDatabase _scriptable)
        {
            UfEditor.DeleteAllSubObjects(_scriptable);
            _scriptable.serializedDataByFile.Clear();
            _scriptable.data.Clear();
            EditorUtility.SetDirty(_scriptable);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Tools/Assets/TSV/" + nameof(UpdateAllTSVDatabase))]
        public static void UpdateAllTSVDatabase()
        {
            foreach (TsvDatabase _scriptable in UfEditor.GetAllAssetsOfType<TsvDatabase>())
                Clear(_scriptable);
            ReimportAllTSV();
        }

        [MenuItem("Tools/Assets/TSV/" + nameof(ReimportAllTSV))]
        private static void ReimportAllTSV() => UfEditor.ReimportAllAssetOfType<TextAsset>(".tsv");
    }
}

#endif