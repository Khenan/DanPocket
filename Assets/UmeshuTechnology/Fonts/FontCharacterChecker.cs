#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using TextAsset = UnityEngine.TextAsset;

namespace Umeshu.USystem.Fonts
{
    internal static class FontCharacterChecker
    {
        internal static void TextAssetGotImported(TextAsset _textAsset) => CheckForMissingCharactersToAddToFontCharacterFile(_textAsset);

        internal static void CheckForMissingCharactersToAddToFontCharacterFile(params TextAsset[] _textAssets)
        {
            TextAsset _textAssetForCharacters = UfEditor.GetAssetOfType<TextAsset>("FontCharacters.txt");
            if (_textAssetForCharacters == null)
            {
                Debug.LogError("FontCharacters file not found. Please create one.");
                return;
            }

            string _characters = _textAssetForCharacters.text;
            string _charactersToAdd = "";
            foreach (TextAsset _textAsset in _textAssets)
            {
                if (_textAsset == _textAssetForCharacters) continue;
                string _text = _textAsset.text;
                for (int _i = 0; _i < _text.Length; _i++)
                    if (!_characters.Contains(_text[_i].ToString()) && !_charactersToAdd.Contains(_text[_i].ToString()))
                        _charactersToAdd += _text[_i];
            }

            _charactersToAdd = _charactersToAdd.RemoveWhiteSpace();
            if (_charactersToAdd.Length > 0)
            {
                _characters += _charactersToAdd;
                _characters = _characters.RemoveWhiteSpace();
                _characters = _characters.RemoveRecurentCharacters();

                async void SaveData()
                {
                    await Task.Delay(1);
                    File.WriteAllText(AssetDatabase.GetAssetPath(_textAssetForCharacters), _characters);
                    EditorUtility.SetDirty(_textAssetForCharacters);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();

                    // Display window to tell user to update fonts
                    EditorUtility.DisplayDialog("Font Characters Updated", "Characters added to FontCharacters file, please update fonts", "OK");
                    $"{"Characters added to FontCharacters file, please update fonts : ".Color(Color.red)} {_charactersToAdd.ToCollectionString()}".Log();
                }
                SaveData();

            }
        }
    }
}
#endif