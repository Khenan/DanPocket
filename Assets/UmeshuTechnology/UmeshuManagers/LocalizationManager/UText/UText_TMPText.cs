using TMPro;
using UnityEngine;

namespace Umeshu.USystem.Text
{
    [RequireComponent(typeof(TMP_Text))]
    public class UText_TMPText : UText<TMP_Text>
    {
        [SerializeField] private UText_TMPText_GlyphLink uText_TMPText_GlyphLink;

        protected override void SetText(string _value, LocalizationData_FontStyles.TextStyle _textStyle)
        {
            if (uText_TMPText_GlyphLink != null) _value = uText_TMPText_GlyphLink.ReplaceKeysWithGlyphs(_value, textComponent);
            textComponent.text = _value;
            textComponent.fontSize = _textStyle.fontSize;
            textComponent.enableAutoSizing = false;
            textComponent.font = _textStyle.font;
            textComponent.color = _textStyle.textColor;
            if (Application.isPlaying) textComponent.fontMaterial = _textStyle.material;
        }
    }
}
