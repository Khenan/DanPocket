#if UNITY_EDITOR

namespace Umeshu.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Umeshu.Uf;
    using UnityEditor;
    using UnityEngine;

    public class Texture2DImporter : AssetPostprocessor
    {
        private const string TEMPORARY_FOLDER = "TEMPORARY";
        private const string TEMPORARY_PREFIX = "TEMP_";
        private static readonly List<string> assetPathsInProcess = new();


        private void OnPostprocessTexture(Texture2D _importedTexture)
        {
            "Texture2DImporter.OnPostprocessTexture prevent".Log();
            return;
            if (assetPathsInProcess.Contains(assetPath)) return;

            EnsureFormatting(assetPath);

            static async void EnsureFormatting(string _assetPath)
            {
                Texture2DImportSettings _importSettings = TextureFormatting.Get2DImportSettings();
                if (_importSettings == null) return;

                foreach (string _folder in _importSettings.foldersToIgnore)
                {
                    if (_assetPath.Contains(_folder))
                        return;
                }

                if (_assetPath.Contains(TEMPORARY_PREFIX))
                {
                    if (_importSettings.moveTemporaryTexturesToSpecificFolder)
                    {
                        string _newPath = "Assets/" + TEMPORARY_FOLDER + "/" + _assetPath.Split('/')[^1];
                        AssetDatabase.MoveAsset(_assetPath, _newPath);
                    }
                    return;
                }

                if (_assetPath.Contains(TEMPORARY_FOLDER))
                {
                    AddTemporaryPrefix(_assetPath);
                    return;
                }

                assetPathsInProcess.Add(_assetPath);
                await Task.Delay(1);

                Texture2D _texture = AssetDatabase.LoadAssetAtPath<Texture2D>(_assetPath);

                if (_texture.MustFormatTexture(out TextureImporter _textureImporter, out string _textureInfo, out _))
                {
                    if (EditorUtility.DisplayDialog("Texture Import Error", $"The texture as {_assetPath.Quote()} is not a valid texture. Do you want to resize it?", "Yes", "No"))
                        TextureFormatting.ResizeTextureToPowerOfTwo(_textureInfo, _textureImporter, _texture);
                    else
                    {
                        // Ask of the user wants to delete the texture or move it to a temporary folder at Assets/TEMPORARY
                        if (EditorUtility.DisplayDialog("Texture Import Error", $"The texture as {_assetPath.Quote()} is not a valid texture. Do you want to delete it?", "Yes", "No"))
                        {
                            AssetDatabase.DeleteAsset(_assetPath);
                        }
                        else
                        {
                            if (_importSettings.moveTemporaryTexturesToSpecificFolder)
                            {
                                string _temporaryPath = "Assets/" + TEMPORARY_FOLDER;
                                if (!AssetDatabase.IsValidFolder(_temporaryPath))
                                    AssetDatabase.CreateFolder("Assets", TEMPORARY_FOLDER);

                                string _newPath = _temporaryPath + "/" + _assetPath.Split('/')[^1];
                                AssetDatabase.MoveAsset(_assetPath, _newPath);

                                _newPath = AddTemporaryPrefix(_newPath);

                                EditorUtility.DisplayDialog("Texture Import Error", $"The texture as {_assetPath.Quote()} has been moved to {_newPath.Quote()}.", "OK");
                            }
                            else
                            {
                                _assetPath = AddTemporaryPrefix(_assetPath);

                                if (!_assetPath.Contains(TEMPORARY_PREFIX))
                                    EditorUtility.DisplayDialog("Texture Import Error", $"The texture as {_assetPath.Quote()} has been renamed.", "OK");
                            }


                        }
                    }
                }
                else
                {
                    //$"Texture2DImporter.OnImportAsset: Texture is valid since {(_validTextureLog + _assetPath).Quote()}.".Log();
                }
                if (_texture.IsFromAssetPath())
                {
                    if (_importSettings.enableCompression) _texture.EnableCompression();
                    else _texture.RemoveCompression();
                }


                assetPathsInProcess.Remove(_assetPath);
            }

        }

        private static string AddTemporaryPrefix(string _path)
        {
            if (!_path.Contains(TEMPORARY_PREFIX))
            {
                string _previousName = _path.Split('/')[^1];
                string _newName = TEMPORARY_PREFIX + _previousName;
                AssetDatabase.RenameAsset(_path, _newName);
                return _path.Replace(_previousName, _newName);
            }
            return _path;
        }
    }
}

#endif