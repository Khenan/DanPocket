using UnityEngine;

namespace Umeshu.USystem.Text
{
    using System.Collections.Generic;
    using TMPro;
    using Umeshu.Uf;
    using Umeshu.USystem.TextAnimation;
    using Umeshu.USystem.TSV;
    using Umeshu.Utility;

    //[CreateAssetMenu(fileName = "LocalizationData_FontStyles_SharedData", menuName = "ScriptableObjects/UmeshuTechnology/TsvBasedData/LocalizationData/LocalizationData_FontStyles_SharedData")]
    public class LocalizationData_FontStyles : TsvDatabase
    {
        [HideInInspector][SerializeField] private SerializedDictionary<string, TextStyle> styles;

        private Dictionary<Material, Dictionary<string, Material>> materialsFromOutlineSize = new();

        public void UpdateMaterialsInStyle()
        {
            foreach (TextStyle _textStyle in styles.Values)
            {
                TMP_FontAsset _font = _textStyle.font;
                float _fontSize = _textStyle.fontSize;
                Material _material = _font.material;
                if (Application.isPlaying)
                {
                    if (!materialsFromOutlineSize.ContainsKey(_material))
                        materialsFromOutlineSize.Add(_material, new Dictionary<string, Material>());

                    Dictionary<string, Material> _materials = materialsFromOutlineSize[_material];
                    float _outlineSizeParameter = 0;
                    if (_textStyle.outlineSize > 0)
                    {
                        if (_textStyle.preciseOutline)
                        {
                            if (UfObject.TryGetOutlineParameterToHavePixelSizeOf(_font, _fontSize, _textStyle.outlineSize, out float _valueToPutInParameter)) _outlineSizeParameter = _valueToPutInParameter;
                            else $"Error of outline on _textStyle {_textStyle.textStyleKey}".Log();
                        }
                        else _outlineSizeParameter = Mathf.Clamp01(_textStyle.outlineSize);
                    }

                    string _outlineIndex = _outlineSizeParameter == 0 ? "0" : _outlineSizeParameter.ToString() + " - " + _textStyle.outlineColorHex;
                    if (!_materials.ContainsKey(_outlineIndex))
                    {
                        if (_outlineSizeParameter > 0)
                        {
                            Material _newMaterial = new(_material);
                            _newMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, _outlineSizeParameter);
                            _newMaterial.SetColor(ShaderUtilities.ID_OutlineColor, _textStyle.outlineColor);
                            _newMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, _outlineSizeParameter);
                            _materials.Add(_outlineIndex, _newMaterial);
                        }
                        else _materials.Add(_outlineIndex, _material);
                    }

                    _textStyle.material = _materials[_outlineIndex];
                }
                else _textStyle.material = _material;
            }
        }

        private const float SECURITY_OUTLINE_SIZE = 0.001f;
        protected override void HandleData(Dictionary<string, Dictionary<string, string>> _tsvData)
        {
            base.HandleData(_tsvData);
            styles.Clear();
            foreach (string _key in _tsvData.Keys)
            {
                TextStyle _textStyle = new(_key);
                foreach (string _category in _tsvData[_key].Keys)
                {
                    string _value = _tsvData[_key][_category].RemoveWhiteSpace();
                    switch (_category)
                    {
                        case "Font":
#if UNITY_EDITOR
                            _textStyle.font = UfEditor.GetAssetOfType<TMP_FontAsset>(_value + " SDF.asset");
                            if (_textStyle.font == null) $"Font {_value} not found".LogError();
#else
                            //throw new System.NotImplementedException("Font asset loading not implemented in build");
#endif
                            break;
                        case "FontSize":
                            _textStyle.fontSize = float.Parse(_value);
                            break;
                        case "PreciseOutline":
                            _textStyle.preciseOutline = bool.Parse(_value);
                            break;
                        case "OutlineSize":
                            _textStyle.outlineSize = float.Parse(_value);
                            break;
                        case "OutlineColor":
                            _textStyle.outlineColorHex = _value;
                            _textStyle.outlineColor = UfColor.HexToColor(_value);
                            break;
                        case "TextStyleFont":
                            _textStyle.textStyleFont = (TextStyleFont)System.Enum.Parse(typeof(TextStyleFont), _value);
                            break;
                        case "Caps":
                            _textStyle.caps = bool.Parse(_value);
                            break;
                        case "TextColor":
                            _textStyle.textColor = UfColor.HexToColor(_value);
                            break;
                        case "CharBefore":
                            _textStyle.charBefore = _value;
                            break;
                        case "CharAfter":
                            _textStyle.charAfter = _value;
                            break;
                        case "CustomAnimation1":
                        case "CustomAnimation2":
                        case "CustomAnimation3":
                        case "CustomAnimation4":
                            if (!string.IsNullOrEmpty(_value) && !string.IsNullOrWhiteSpace(_value))
                                _textStyle.customAnimations.Add(_value);
                            break;
                    }
                }

                if (_textStyle.outlineSize < SECURITY_OUTLINE_SIZE)
                {
                    _textStyle.outlineSize = SECURITY_OUTLINE_SIZE;
                    _textStyle.outlineColor = _textStyle.textColor;
                }

                UpdateMaterialsInStyle();

                styles.Add(_key, _textStyle);
            }
        }

        public TextStyle GetTextStyle(string _styleKey)
        {
            if (styles.TryGetValue(_styleKey, out TextStyle _style)) return _style;
            $"{_styleKey} not found in styles, styles are {styles.Keys.ToCollectionString()}".Log();
            return null;
        }

        public string ApplyStyleToTextFromStyleKey(string _text, string _styleKey)
        {
            TextStyle _style = GetTextStyle(_styleKey);
            return _style != null ? _style.SetTextToStyle(_text) : _text;
        }

        [System.Serializable]
        public class TextStyle
        {
            public string textStyleKey;
            public TMP_FontAsset font;
            public Material material;
            public float fontSize;
            public bool preciseOutline;
            public float outlineSize;
            public string outlineColorHex;
            public Color outlineColor;
            public TextStyleFont textStyleFont;
            public bool caps;
            public Color textColor;
            public string charBefore;
            public string charAfter;
            public List<string> customAnimations = new();

            public TextStyle(string _textStyleKey) => textStyleKey = _textStyleKey;

            public string SetTextToStyle(string _text)
            {
                string _prefix = "";
                string _suffix = "";

                bool _customAnimationsExist = customAnimations.Count > 0;

                if (fontSize > 0)
                {
                    _prefix += "<size=" + fontSize + ">";
                    _suffix += "</size>";

                    if (_customAnimationsExist)
                    {
                        _prefix += "<link=\"";
                        for (int _effectIndex = 0; _effectIndex < customAnimations.Count; _effectIndex++)
                        {
                            if (_effectIndex > 0) _prefix += TMPText_Animator.EFFECT_SEPARATOR;
                            string _effect = TMPText_Animation.ReplaceParametersNames(customAnimations[_effectIndex]);
                            _prefix += _effect;
                        }
                        _prefix += ">";

                        _suffix += "</link>";
                    }
                }

                switch (textStyleFont)
                {
                    case TextStyleFont.Bold:
                        _prefix += "<b>";
                        _suffix += "</b>";
                        break;
                    case TextStyleFont.Italic:
                        _prefix += "<i>";
                        _suffix += "</i>";
                        break;
                    case TextStyleFont.BoldItalic:
                        _prefix += "<b><i>";
                        _suffix += "</i></b>";
                        break;
                }

                if (caps)
                {
                    _prefix += "<uppercase>";
                    _suffix += "</uppercase>";
                }

                return _prefix + charBefore + _text + charAfter + _suffix;
            }
        }

        public enum TextStyleFont
        {
            Default,
            Bold,
            Italic,
            BoldItalic
        }
    }
}