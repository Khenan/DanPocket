#if UNITY_EDITOR

using Umeshu.Utility;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Texture2DImportSettings))]
public class Texture2DImportSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Enable Crunch Compression"))
        {
            Texture2DImportSettings _settings = (Texture2DImportSettings)target;
            _settings.enableCompression = true;
            EditorUtility.SetDirty(_settings);
            TextureFormatting.EnableCrunchCompressionOnAllTextures();
        }

        if (GUILayout.Button("Disable Crunch Compression"))
        {
            Texture2DImportSettings _settings = (Texture2DImportSettings)target;
            _settings.enableCompression = false;
            EditorUtility.SetDirty(_settings);
            TextureFormatting.DisableCrunchCompressionOnAllTextures();
        }
    }
}


#endif