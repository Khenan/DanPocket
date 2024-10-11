using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Umeshu.USystem.Text
{

    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class UText_Text : UText<UnityEngine.UI.Text>
    {
        protected override void SetText(string _value, LocalizationData_FontStyles.TextStyle _textStyle)
        {
            textComponent.text = _value;
            textComponent.color = _textStyle.textColor;
            if (_textStyle.fontSize > 0) textComponent.fontSize = Mathf.FloorToInt(_textStyle.fontSize);
        }
    }

}
