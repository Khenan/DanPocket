using UnityEngine;

namespace Umeshu.Uf
{
    public static class UfVisual
    {
        public static Gradient ReplaceColors(this Gradient _originalGradient, Color _color)
        {
            GradientColorKey[] _colors = _originalGradient.colorKeys;
            for (int _i = 0; _i < _originalGradient.colorKeys.Length; _i++)
            {
                GradientColorKey _colorKey = _originalGradient.colorKeys[_i];
                _colors[_i] = new GradientColorKey(_color, _colorKey.time);
            }
            Gradient _gradient = new();
            _gradient.SetKeys(_colors, _originalGradient.alphaKeys);
            return _gradient;
        }

        public static Texture2D GenerateSpriteSheet(Sprite[] _sprites)
        {
            Vector2 _cellSize = new();
            foreach (Sprite _sprite in _sprites)
            {
                _cellSize = new(Mathf.Max(_cellSize.x, _sprite.rect.size.x), Mathf.Max(_cellSize.y, _sprite.rect.size.y));
            }
            Vector2Int _cellSizeInt = new(Mathf.CeilToInt(_cellSize.x), Mathf.CeilToInt(_cellSize.y));
            Texture2D _tex = new(_cellSizeInt.x * _sprites.Length, _cellSizeInt.y);

            for (int _i = 0; _i < _sprites.Length; _i++)
            {
                int _xDecal = _cellSizeInt.x * _i;
                Texture2D _originTex = _sprites[_i].texture;
                for (int _x = 0; _x < _cellSizeInt.x; _x++)
                {
                    for (int _y = 0; _y < _cellSizeInt.y; _y++)
                    {
                        Color _originPixel = _originTex.GetPixel(Mathf.CeilToInt(_sprites[_i].rect.x) + _x, Mathf.CeilToInt(_sprites[_i].rect.y) + _y);
                        _tex.SetPixel(_xDecal + _x, _y, _originPixel);
                    }
                }
            }
            _tex.Apply();
            return _tex;
        }

        #region Hue Shift
        public static Color GetHueShiftedColor(Color _original, float _shift)
        {
            Color.RGBToHSV(_original, out float _h, out float _s, out float _v);
            _h += _shift;
            return Color.HSVToRGB(_h, _s, _v);
        }

        public static Gradient GetHueShiftedGradient(Gradient _original, float _shift)
        {
            Gradient _newGradient = new();
            GradientColorKey[] _newColorKeys = new GradientColorKey[_original.colorKeys.Length];
            for (int _i = 0; _i < _original.colorKeys.Length; _i++)
            {
                GradientColorKey _colorKey = _original.colorKeys[_i];
                _newColorKeys[_i] = new(GetHueShiftedColor(_colorKey.color, _shift), _colorKey.time);
            }
            _newGradient.colorKeys = _newColorKeys;
            _newGradient.alphaKeys = _original.alphaKeys;
            return _newGradient;
        }
        #endregion

        #region Sprite texture data
        public static void GetTilingAndOffsetFromSprite(this Sprite _sprite, out Vector2 _tiling, out Vector2 _offset)
        {
            _offset = new(_sprite.rect.x / (float)_sprite.texture.width, _sprite.rect.y / (float)_sprite.texture.height);
            _tiling = new(_sprite.rect.width / (float)_sprite.texture.width, _sprite.rect.height / (float)_sprite.texture.height);
        }

        public static void GetSpriteSizeY(this Sprite _sprite, out float _posY, out float _sizeY, out float _sizeInWorldY)
        {
            _posY = _sprite.rect.y / (float)_sprite.texture.height;
            _sizeY = _sprite.rect.height / (float)_sprite.texture.height;
            _sizeInWorldY = _sprite.rect.height / (float)_sprite.pixelsPerUnit;
        }
        #endregion

        #region Animation

        public static bool ContainsParameter(this Animator _animator, string _param)
        {
            foreach (AnimatorControllerParameter _animParam in _animator.parameters)
            {
                if (_animParam.name == _param) return true;
            }
            return false;
        }

        public static bool TryDoAnim(ref float _timeRemainingInAnim, float _timeBeforeAnim, string _trigger, Animator _anmtr)
        {
            if (_timeRemainingInAnim > 0) return false;
            _timeRemainingInAnim = _timeBeforeAnim;
            _anmtr.SetTrigger(_trigger);
            return true;
        }

        public static void TriggerAnim(Animator _animator, AnimationClip _animClip, string _trigger, out float _timeRemainingInAnim)
        {
            AnimatorOverrideController _animatorOverrideController = new(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _animatorOverrideController;
            _animatorOverrideController[_animator.runtimeAnimatorController.animationClips[0].name] = _animClip;
            _animator.SetTrigger(_trigger);
            _timeRemainingInAnim = _animClip.averageDuration;
        }

        #endregion

    }
}
