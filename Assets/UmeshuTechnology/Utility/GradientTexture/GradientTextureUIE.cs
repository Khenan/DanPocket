#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;
using Umeshu.Utility;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using PopupWindow = UnityEditor.PopupWindow;

namespace Umeshu.Common
{
    [CustomPropertyDrawer(typeof(GradientTexture), true)]
    public class GradientTextureUIE : PropertyDrawerUtil
    {
        private const int TEXTURE_DISPLAY_HEIGHT = 10;
        private const int SPACE_UNDER = 5;

        public override void OnCustomGUI(ref Rect _position, SerializedProperty _property, GUIContent _label)
        {
            GUIContent _labelOfProperty = new(_label.text, _label.tooltip);

            _position.height -= TEXTURE_DISPLAY_HEIGHT + SPACE_UNDER;

            Rect[] _rects = _position.SplitRect(_withSpacing: true, 0.7f, .8f);
            Rect _gradientRect = _rects[0];
            Rect _widthRect = _rects[1];
            Rect _buttonRect = _rects[2];

            SerializedProperty _gradientProperty = _property.FindPropertyRelative(nameof(GradientTexture.gradient));
            SerializedProperty _widthProperty = _property.FindPropertyRelative(nameof(GradientTexture.width));

            EditorGUI.PropertyField(_gradientRect, _gradientProperty, _labelOfProperty);
            EditorGUI.PropertyField(_widthRect, _widthProperty, GUIContent.none);

            if (GUI.Button(_buttonRect, "Update"))
            {
                GradientTexture _gradientTexture = _property.GetValue() as GradientTexture;
                _gradientTexture.UpdateTextureFromGradient();
            }

            _position.y += _position.height;
            _position.height = TEXTURE_DISPLAY_HEIGHT;

            Rect[] _textureRects = _position.SplitRect(_withSpacing: true, 0.4f);
            Rect _labelRect = _textureRects[0];
            Rect _textureRect = _textureRects[1];

            SerializedProperty _textureProperty = _property.FindPropertyRelative(nameof(GradientTexture.texture));
            Texture2D _texture = _textureProperty.GetValue() as Texture2D;
            if (_texture != null)
            {
                EditorGUI.LabelField(_labelRect, "Texture");
                EditorGUI.DrawPreviewTexture(_textureRect, _texture);
            }
            else
            {
                GUIStyle _errorStyle = new(GUI.skin.label);
                _errorStyle.normal.textColor = Color.red;
                _errorStyle.alignment = TextAnchor.MiddleCenter;
                _errorStyle.fontStyle = FontStyle.Bold;
                _errorStyle.wordWrap = true;
                string _labelText = "Texture is null.";
                EditorGUI.LabelField(_textureRect, _labelText, _errorStyle);
            }
        }

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return base.GetPropertyHeight(_property, _label) + TEXTURE_DISPLAY_HEIGHT + SPACE_UNDER;
        }
    }
}
#endif