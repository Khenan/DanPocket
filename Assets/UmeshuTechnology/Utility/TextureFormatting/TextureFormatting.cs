#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Umeshu.Uf;
using UnityEditor;
using UnityEngine;

namespace Umeshu.Utility
{
    public static class TextureFormatting
    {
        #region Utility Methods
        private struct TextureLogFromSize
        {
            internal string log;
            internal int size;

            internal TextureLogFromSize(string _log, int _size)
            {
                log = _log;
                size = _size;
            }

            public override string ToString() => $"{log}";
        }

        internal static Texture2DImportSettings Get2DImportSettings()
        {
            if (EditorApplication.isCompiling) return null;

            if (!Directory.Exists("Assets/UmeshuEditorData"))
                AssetDatabase.CreateFolder("Assets", "UmeshuEditorData");
            return UfEditor.GetOrCreateScriptableObject<Texture2DImportSettings>(_path: "Assets/UmeshuEditorData");
        }
        internal static bool IsFromIgnoredFolders(this Object _object) => _object.IsFromSpecificFolders(Get2DImportSettings().foldersToIgnore);


        #endregion

        #region Called by User

        [MenuItem("Assets/Texture2D/" + nameof(FormatTexture))]
        private static void FormatTexture()
        {
            if (Selection.activeObject is Texture2D _texture)
            {
                if (!_texture.MustFormatTexture(out TextureImporter _textureImporter, out string _textureInfo, out _)) return;
                ResizeTextureToPowerOfTwo(_textureInfo, _textureImporter, _texture);
            }
            else Debug.LogError($"Can't resize texture : object is not a Texture2D");
            Debug.Log("Texture resize method is done");
        }

        [MenuItem("Tools/Assets/Texture2D/Formatting/" + nameof(LogNonFormatedTextures))]
        private static void LogNonFormatedTextures()
        {
            List<Texture2D> _textures = UfEditor.GetAllAssetsOfType<Texture2D>();
            List<TextureLogFromSize> _textureLogFromSizes = new();
            List<string> _validTextures = new();
            foreach (Texture2D _texture in _textures)
            {
                if (!_texture.MustFormatTexture(out _, out string _textureInfo, out string _validTextureLog))
                {
                    _validTextures.Add(_validTextureLog);
                    continue;
                }
                _textureLogFromSizes.Add(new(_textureInfo, Mathf.Max(_texture.width, _texture.height)));
            }
            _textureLogFromSizes.Sort((_a, _b) => -_a.size.CompareTo(_b.size));
            $"Textures that are formated - {_validTextures.ToCollectionString()}".Log();
            $"Textures that are not formated - {_textureLogFromSizes.ToCollectionString()}".Log();
        }


        [MenuItem("Tools/Assets/Texture2D/Formatting/" + nameof(ForceTexturesToPowerOfTwo))]
        private static void ForceTexturesToPowerOfTwo()
        {
            if (!EditorUtility.DisplayDialog("Force Textures to Power of Two", "This operation will resize all textures to the nearest power of two. Are you sure you want to continue?", "Yes", "No"))
                return;

            DoMethodOnAllResizableTextures(ResizeTextureToPowerOfTwo, (_count) => EditorUtility.DisplayDialog("Continue?", $"You resized {_count} textures. Do you want to continue?", "Yes", "No"), 50, false);
        }

        [MenuItem("Tools/Assets/Texture2D/Formatting/" + nameof(ReplacePivotOfTooBigTextures))]
        private static void ReplacePivotOfTooBigTextures()
        {
            if (!EditorUtility.DisplayDialog("Replace Pivot of Too Big Textures", "This operation will resize all textures to the nearest power of two and replace the pivot. Are you sure you want to continue?", "Yes", "No"))
                return;

            DoMethodOnAllResizableTextures(ReplacePivotOfTooBigTexture, (_count) => EditorUtility.DisplayDialog("Continue?", $"You resized the pivots of {_count} textures. Do you want to continue?", "Yes", "No"), 5, true);
        }

        [MenuItem("Tools/Assets/Texture2D/CrunchCompression/" + nameof(EnableCrunchCompressionOnAllTextures))]
        internal static void EnableCrunchCompressionOnAllTextures()
        {
            if (!EditorUtility.DisplayDialog("Enable Crunch Compression on All Textures", "This operation will enable crunch compression on all textures. Are you sure you want to continue?", "Yes", "No"))
                return;

            UfEditor.GetAllAssetsOfType<Texture2D>().ForEach(_texture => _texture.EnableCompression());
        }

        [MenuItem("Tools/Assets/Texture2D/CrunchCompression/" + nameof(DisableCrunchCompressionOnAllTextures))]
        internal static void DisableCrunchCompressionOnAllTextures()
        {
            if (!EditorUtility.DisplayDialog("Disable Crunch Compression on All Textures", "This operation will disable crunch compression on all textures. Are you sure you want to continue?", "Yes", "No"))
                return;

            UfEditor.GetAllAssetsOfType<Texture2D>().ForEach(_texture => _texture.RemoveCompression());
        }

        #endregion

        internal enum TextureChecks
        {
            None = 0,
            FromAssetPath = 1 << 0,
            FromSpecificFolders = 1 << 1,
            HasValidDimensions = 1 << 2,
            HasImporter = 1 << 3,
            IsSprite = 1 << 4,
            IsSingleSpriteUsedInVFX = 1 << 5,
            All =
                FromAssetPath |
                FromSpecificFolders |
                HasValidDimensions |
                HasImporter |
                IsSprite |
                IsSingleSpriteUsedInVFX
        }

        internal static bool MustFormatTexture(this Texture2D _texture, out TextureImporter _textureImporter, out string _textureInfo, out string _validTextureLog, TextureChecks _textureChecks = TextureChecks.All)
        {
            _textureInfo = _texture.name + " " + _texture.width + "x" + _texture.height + " at path " + AssetDatabase.GetAssetPath(_texture);
            _textureImporter = null;
            _validTextureLog = "";

            if ((_textureChecks & TextureChecks.FromAssetPath) != 0 && !_texture.IsFromAssetPath())
            {
                _validTextureLog = "Not from the asset folder : " + _textureInfo;
                return false;
            }
            if ((_textureChecks & TextureChecks.FromSpecificFolders) != 0 && _texture.IsFromIgnoredFolders())
            {
                _validTextureLog = "From ignored folder : " + _textureInfo;
                return false;
            }
            if ((_textureChecks & TextureChecks.HasValidDimensions) != 0 && _texture.HasValidDimensions())
            {
                _validTextureLog = "Already power of two : " + _textureInfo;
                return false;
            }
            if ((_textureChecks & TextureChecks.HasImporter) != 0 && !_texture.TryGetTextureImporter(out _textureImporter))
            {
                _validTextureLog = "No importer : " + _textureInfo;
                return false;
            }
            if ((_textureChecks & TextureChecks.IsSprite) != 0 && _textureImporter.textureType != TextureImporterType.Sprite)
            {
                _validTextureLog = "Not a sprite : " + _textureInfo;
                return false;
            }
            if ((_textureChecks & TextureChecks.IsSingleSpriteUsedInVFX) != 0 && _textureImporter.spriteImportMode == SpriteImportMode.Single && _texture.IsTextureUsedInMaterials())
            {
                _validTextureLog = "Is a single sprite texture used in VFX" + _textureInfo;
                return false;
            }

            return true;
        }

        private static void DoMethodOnAllResizableTextures(System.Func<string, TextureImporter, Texture2D, bool> _method, System.Func<int, bool> _confirmMethod, int _askEvery, bool _startWithBiggest)
        {
            List<Texture2D> _textures = UfEditor.GetAllAssetsOfType<Texture2D>();
            int _count = 0;
            if (_startWithBiggest) _textures.Sort((_a, _b) => -(_a.width * _a.height - _b.width * _b.height));
            foreach (Texture2D _texture in _textures)
            {
                if (!_texture.MustFormatTexture(out TextureImporter _textureImporter, out string _textureInfo, out _)) continue;
                if (!_method(_textureInfo, _textureImporter, _texture)) continue;

                #region Security Window
                if (_confirmMethod != null)
                {
                    _count++;
                    int _nextCountAsk = Mathf.CeilToInt(_count / (float)_askEvery) * _askEvery;

                    bool _showWindowToContinue = _count == _nextCountAsk;
                    if (_showWindowToContinue && !_confirmMethod(_count))
                        break;
                }
                #endregion
            }
            AssetDatabase.Refresh();
        }

        #region Final Methods

        private static bool ReplacePivotOfTooBigTexture(string _textureInfo, TextureImporter _textureImporter, Texture2D _texture)
        {
            // --- Check if texture size is over 8192
            _textureImporter.GetTrueTextureSize(out int _trueWidth, out int _trueHeight);

            if (_trueWidth <= 8192 && _trueHeight <= 8192) return false;

            float _ratioOfSprites = 8192f / Mathf.Max(_trueWidth, _trueHeight);


            // read the text of the .meta file
            string _metaPath = AssetDatabase.GetAssetPath(_texture) + ".meta";
            string _metaText = File.ReadAllText(_metaPath);

            Dictionary<string, List<string>> _keyToReplace = new()
            {
                { "spriteID:", new List<string>() },
                { "tessellationDetail:", new List<string>() },
                { "internalID:", new List<string>() }
            };

            foreach (string _line in _metaText.Split('\n'))
                foreach (string _key in _keyToReplace.Keys)
                    if (_line.Contains(_key))
                        _keyToReplace[_key].Add(_line);

            if (_textureImporter.spriteImportMode == SpriteImportMode.Multiple)
            {
                SpriteMetaData[] _newSpritesheet = new SpriteMetaData[_textureImporter.spritesheet.Length];

                for (int _i = 0; _i < _textureImporter.spritesheet.Length; _i++)
                {
                    SpriteMetaData _metaData = _textureImporter.spritesheet[_i];
                    _metaData.rect = new Rect(
                        _metaData.rect.x * _ratioOfSprites,
                        _metaData.rect.y * _ratioOfSprites,
                        _metaData.rect.width * _ratioOfSprites,
                        _metaData.rect.height * _ratioOfSprites
                    );
                    _metaData.border = new Vector4(
                        _metaData.border.x * _ratioOfSprites,
                        _metaData.border.y * _ratioOfSprites,
                        _metaData.border.z * _ratioOfSprites,
                        _metaData.border.w * _ratioOfSprites
                    );
                    _newSpritesheet[_i] = _metaData;
                }

                _textureImporter.spritesheet = _newSpritesheet;


                _metaText = File.ReadAllText(_metaPath);
                // replace sprite sheet in the meta file


            }

            _textureImporter.spritePixelsPerUnit *= _ratioOfSprites;

            EditorUtility.SetDirty(_textureImporter);

            if (AssetDatabase.WriteImportSettingsIfDirty(_textureImporter.assetPath))
                _textureImporter.SaveAndReimport();


            if (_textureImporter.spriteImportMode == SpriteImportMode.Multiple)
            {
                _metaText = File.ReadAllText(_metaPath);

                string _newMetaText = "";
                string[] _metaLines = _metaText.Split('\n');
                for (int _metaIndex = 0; _metaIndex < _metaLines.Length; _metaIndex++)
                {
                    string _line = _metaLines[_metaIndex];
                    bool _replaced = false;
                    foreach (string _key in _keyToReplace.Keys)
                        if (_line.Contains(_key))
                        {
                            _newMetaText += _keyToReplace[_key][0] + "\n";
                            _keyToReplace[_key].RemoveAt(0);
                            _replaced = true;
                            break;
                        }
                    if (!_replaced)
                    {
                        _newMetaText += _line;
                        if (_metaIndex < _metaLines.Length - 1) _newMetaText += "\n";
                    }
                }
                File.WriteAllText(_metaPath, _newMetaText);
            }


            Debug.Log($"{_textureInfo} pivot was updated", _texture);

            return true;
        }

        internal static bool ResizeTextureToPowerOfTwo(string _textureInfo, TextureImporter _textureImporter, Texture2D _texture)
        {
            // --- Save original settings
            int _maxTextureSize = _textureImporter.maxTextureSize;
            bool _isTextureReadable = _textureImporter.isReadable;


            // --- Check if texture size is over 8192
            _textureImporter.GetTrueTextureSize(out int _trueWidth, out int _trueHeight);
            if (_trueWidth > 8192 || _trueHeight > 8192)
            {
                Debug.LogError(_textureInfo + " is too big to be resized automatically");

                return false;
            }

            // --- Set settings for changements
            _textureImporter.isReadable = true;
            _textureImporter.maxTextureSize = 8192;
            if (AssetDatabase.WriteImportSettingsIfDirty(_textureImporter.assetPath)) _textureImporter.SaveAndReimport();

            // --- Create new texture
            int _previousWidth = _texture.width;
            int _previousHeight = _texture.height;
            int _newWidth = Mathf.NextPowerOfTwo(_texture.width);
            int _newHeight = Mathf.NextPowerOfTwo(_texture.height);
            Texture2D _newTexture = new(_newWidth, _newHeight);

            // --- Create clean array of pixels to make the transparent pixels extend the texture on the right and the top
            Color32[] _pixels = new Color32[_newWidth * _newHeight];
            for (int _i = 0; _i < _pixels.Length; _i++) _pixels[_i] = Color.clear;
            for (int _x = 0; _x < _texture.width; _x++)
                for (int _y = 0; _y < _texture.height; _y++)
                    _pixels[_x + _y * _newWidth] = _texture.GetPixel(_x, _y);

            // --- Apply the pixels to the new texture
            _newTexture.SetPixels32(_pixels);
            _newTexture.Apply();
            byte[] _bytes = _newTexture.EncodeToPNG();

            // --- Save the new texture
            File.WriteAllBytes(Application.dataPath + "/../" + AssetDatabase.GetAssetPath(_texture), _bytes);

            // --- Recalculate max size under certains conditions
            int _previousMax = Mathf.Max(_previousWidth, _previousHeight);
            int _newMax = Mathf.Max(_newWidth, _newHeight);
            float _ratio = _previousMax / (float)_newMax;
            if (_ratio < 0.75f) _maxTextureSize = Mathf.NextPowerOfTwo(_maxTextureSize + 1);

            // --- Update pivot in pixel
            if (_textureImporter.spriteImportMode == SpriteImportMode.Single)
            {
                TextureImporterSettings _textureSettings = new();
                _textureImporter.ReadTextureSettings(_textureSettings);
                Vector2 _pivot = _textureSettings.spriteAlignment switch
                {
                    (int)SpriteAlignment.TopLeft => new Vector2(0, 1),
                    (int)SpriteAlignment.TopCenter => new Vector2(0.5f, 1),
                    (int)SpriteAlignment.TopRight => new Vector2(1, 1),
                    (int)SpriteAlignment.LeftCenter => new Vector2(0, 0.5f),
                    (int)SpriteAlignment.RightCenter => new Vector2(1, 0.5f),
                    (int)SpriteAlignment.BottomLeft => new Vector2(0, 0),
                    (int)SpriteAlignment.BottomCenter => new Vector2(0.5f, 0),
                    (int)SpriteAlignment.BottomRight => new Vector2(1, 0),
                    _ => _textureImporter.spritePivot,
                };
                Vector2 _newPivot = new(_pivot.x * (_previousWidth / (float)_newWidth), _pivot.y * (_previousHeight / (float)_newHeight));

                _textureSettings.spriteAlignment = (int)SpriteAlignment.Custom;
                _textureImporter.SetTextureSettings(_textureSettings);
                _textureImporter.spritePivot = _newPivot;
            }

            // --- Reset settings
            _textureImporter.isReadable = _isTextureReadable;
            _textureImporter.maxTextureSize = _maxTextureSize;
            _textureImporter.crunchedCompression = true;
            _textureImporter.compressionQuality = 100;
            if (AssetDatabase.WriteImportSettingsIfDirty(_textureImporter.assetPath)) _textureImporter.SaveAndReimport();


            // --- Log path of the texture
            Debug.Log($"{_textureInfo} was at size {_previousWidth}x{_previousHeight} was has been resized to {_newWidth}x{_newHeight} and caped at {_maxTextureSize}", _newTexture);

            return true;
        }

        #endregion


    }
}
#endif