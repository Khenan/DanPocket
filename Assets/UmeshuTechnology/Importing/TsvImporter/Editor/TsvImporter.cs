#if UNITY_EDITOR

namespace Umeshu.USystem.TSV
{
    using System.IO;
    using UnityEditor;
    using UnityEditor.AssetImporters;
    using UnityEngine;

    [ScriptedImporter(1, "tsv", 255)]
    public class TsvImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext _ctx)
        {
            string _assetName = _ctx.assetPath.Split('/')[^1].Replace(".tsv", "");

            TextAsset _textAsset = new(File.ReadAllText(_ctx.assetPath))
            {
                name = _assetName,
                hideFlags = HideFlags.None
            };
            _ctx.AddObjectToAsset(Path.GetFileNameWithoutExtension(_ctx.assetPath), _textAsset);
            _ctx.SetMainObject(_textAsset);


            AssetDatabase.SaveAssetIfDirty(_textAsset);

            Umeshu.USystem.TSV.TsvDatabase.TsvGotImported(_textAsset, _assetName);
            Umeshu.USystem.Fonts.FontCharacterChecker.TextAssetGotImported(_textAsset);
        }
    }
}

#endif