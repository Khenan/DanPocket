#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Umeshu.Utility
{
    public static class SpriteAtlasMaker
    {


        [MenuItem("Assets/2D/" + nameof(CreateSpriteAtlas))]
        private static void CreateSpriteAtlas()
        {
            if (EditorUtility.DisplayDialog("Create Sprite Atlas", "Would you like to create a sprite atlas with all the sprite in this folder and its sub-folders?", "Yes", "No"))
            {
                UfMenuItem.InputMenuItemAction_String("Sprite Atlas Name", "Atlas_NAME", CreateSpriteAtlasNamed);
            }
        }

        private static void CreateSpriteAtlasNamed(string _atlasName)
        {
            string _pathToCurrentFolder = UfEditor.GetCurrentProjectWindowPath();
            string _endOfPath = _atlasName + ".spriteatlas";

            float _maxTextureSize = 2048;

            SpriteAtlas _spriteAtlas = new();
            AssetDatabase.CreateAsset(_spriteAtlas, _pathToCurrentFolder + "/" + _endOfPath);

            _spriteAtlas.SetPackingSettings(new SpriteAtlasPackingSettings()
            {
                enableTightPacking = true,
                padding = 2,
                enableRotation = true
            });


            // get all the sprites in the folder and add them to the sprite atlas
            List<Texture2D> _texture2Ds = UfEditor.GetAllAssetsOfType<Texture2D>(_pathToCurrentFolder);

            foreach (Texture2D _texture2D in _texture2Ds)
            {
                string _texturePath = AssetDatabase.GetAssetPath(_texture2D);
                Sprite[] _spritesInTexture = AssetDatabase.LoadAllAssetsAtPath(_texturePath).OfType<Sprite>().ToArray();
                foreach (Sprite _sprite in _spritesInTexture)
                {
                    Vector2 _spriteSize = new(_sprite.rect.width, _sprite.rect.height);
                    float _lastTextureSize = _maxTextureSize;
                    _maxTextureSize = Mathf.Max(_maxTextureSize, Mathf.Max(_spriteSize.x, _spriteSize.y));
                    if (_lastTextureSize != _maxTextureSize)
                    {
                        Debug.LogWarning("Texture size increased to: " + _maxTextureSize + " from sprite " + _sprite.name);
                    }
                    SpriteAtlasExtensions.Add(_spriteAtlas, new Object[] { _sprite });
                }
            }

            TextureImporterPlatformSettings _platformSettings = _spriteAtlas.GetPlatformSettings("DefaultTexturePlatform");
            _platformSettings.compressionQuality = 100;
            _platformSettings.crunchedCompression = true;
            _platformSettings.textureCompression = TextureImporterCompression.CompressedHQ;
            _platformSettings.maxTextureSize = Mathf.NextPowerOfTwo((int)_maxTextureSize);

            _spriteAtlas.SetPlatformSettings(_platformSettings);

            EditorUtility.SetDirty(_spriteAtlas);
            AssetDatabase.SaveAssets();

            SpriteAtlas _variant = null;
            if (EditorUtility.DisplayDialog("Create Variant", "Would you like to create a variant of the sprite atlas?", "Yes", "No"))
            {
                _spriteAtlas.SetIncludeInBuild(false);

                string _variantPath = _pathToCurrentFolder + "/" + _atlasName + "_Variant.spriteatlas";
                AssetDatabase.CopyAsset(_pathToCurrentFolder + "/" + _endOfPath, _variantPath);
                _variant = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(_variantPath);
                _variant.SetIncludeInBuild(true);
                _variant.SetIsVariant(true);
                _variant.SetMasterAtlas(_spriteAtlas);


                EditorUtility.SetDirty(_variant);
                EditorUtility.SetDirty(_spriteAtlas);
                AssetDatabase.SaveAssets();
            }


            UfEditor.ReimportRandomAssembly();

            if (EditorUtility.DisplayDialog("Pack Sprite Atlas", "Would you like to pack the sprite atlas?", "Yes", "No"))
            {
                List<SpriteAtlas> _atlases = new() { _spriteAtlas };
                if (_variant != null) _atlases.Add(_variant);
                SpriteAtlasUtility.PackAtlases(_atlases.ToArray(), EditorUserBuildSettings.activeBuildTarget);
            }


            Debug.Log("Sprite Atlas created at: " + _pathToCurrentFolder + "/" + _endOfPath);
        }

    }

}

#endif