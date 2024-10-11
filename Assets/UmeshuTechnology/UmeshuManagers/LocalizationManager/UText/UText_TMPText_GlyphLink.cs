using TMPro;
using Umeshu.Uf;
using UnityEngine;


public class UText_TMPText_GlyphLink : ScriptableObject
{
    public TMP_SpriteAsset spriteAsset;

    private const char GLYPH_START = '@';
    private const char GLYPH_END = '@';

    internal string ReplaceKeysWithGlyphs(string _text, TMP_Text _tmp_text)
    {
        bool _isInvalid = spriteAsset == null || string.IsNullOrEmpty(_text) || !_text.Contains(GLYPH_START) || !_text.Contains(GLYPH_END);
        if (_isInvalid)
        {
            return _text;
        }
        else
        {
            _tmp_text.spriteAsset = spriteAsset;
            return UfText.ReplaceBalisesInText(_text, GLYPH_START, GLYPH_END, ReplaceKeyWithGlyph);
        }
    }

    private string ReplaceKeyWithGlyph(string _text)
    {
        if (spriteAsset == null) return _text;
        int _index = spriteAsset.GetSpriteIndexFromName(_text);
        return _index == -1 ? $"[Sprite {_text.Quote()} not found]" : $"<sprite index={_index}>";
    }
}
