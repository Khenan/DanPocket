using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Umeshu.Uf
{
    public static class UfObject
    {
        #region Text Mesh Pro
        //private const float MULTIPLY_RATIO_TO_FIND_MAX_OUTLINE_SIZE = 91.42862f;
        //public static float GetMaxOutlineSize(this TMP_Text _tmp) => GetMaxOutlineSizeAtPointWithConst(_tmp, MULTIPLY_RATIO_TO_FIND_MAX_OUTLINE_SIZE);
        // private const float MULTIPLY_RATIO_TO_FIND_MAX_OUTLINE_SIZE_AT_POINT_FOUR_WITHOUT_RATIO = 90.00005f;
        private const float MULTIPLY_RATIO_TO_FIND_MAX_OUTLINE_SIZE_AT_POINT_FOUR_WITHOUT_RATIO = 82f;
        public static float GetMaxOutlineSizeAtPointFourWithoutRatio(this TMP_Text _tmp) => GetMaxOutlineSizeAtPointWithConst(_tmp.font, _tmp.fontSize, MULTIPLY_RATIO_TO_FIND_MAX_OUTLINE_SIZE_AT_POINT_FOUR_WITHOUT_RATIO);
        public static float GetMaxOutlineSizeAtPointFourWithoutRatio(TMP_FontAsset _fontAsset, float _fontSize) => GetMaxOutlineSizeAtPointWithConst(_fontAsset, _fontSize, MULTIPLY_RATIO_TO_FIND_MAX_OUTLINE_SIZE_AT_POINT_FOUR_WITHOUT_RATIO);
        public static bool TryGetOutlineParameterToHavePixelSizeOf(this TMP_Text _tmp, float _pixelSize, out float _valueToPutInParameter) => TryGetOutlineParameterToHavePixelSizeOf(_tmp.font, _tmp.fontSize, _pixelSize, out _valueToPutInParameter);
        public static bool TryGetOutlineParameterToHavePixelSizeOf(TMP_FontAsset _fontAsset, float _fontSize, float _pixelSize, out float _valueToPutInParameter)
        {
            float _maxOutlineAtPointFour = GetMaxOutlineSizeAtPointFourWithoutRatio(_fontAsset, _fontSize);
            float _ratio = _pixelSize / _maxOutlineAtPointFour;
            _valueToPutInParameter = Mathf.Clamp(_ratio * .4f, 0, .4f);
            bool _validRatio = _ratio <= 1;
            int _wantedPixelSize = Mathf.CeilToInt(_fontAsset.atlasPadding * _ratio);
            bool _cantHaveThisPixelSize = _wantedPixelSize > 64;
            string _fontAssetNotBigEnoughString = $"The pixel size is too big for the font asset named {_fontAsset.name}.";
            return _validRatio.LogErrorIfFalse(
                _cantHaveThisPixelSize ? _fontAssetNotBigEnoughString :
                _fontAssetNotBigEnoughString + $" Please increase padding to at least {Mathf.CeilToInt(_fontAsset.atlasPadding * _ratio)}");
        }

        public static bool TrySetOulinePixelOnTextMeshPro(this TMP_Text _tmp, float _pixelSize)
        {
            if (_tmp.TryGetOutlineParameterToHavePixelSizeOf(_pixelSize, out float _outlineParameter))
            {
                Material _material = _tmp.fontSharedMaterial;
                _material.SetFloat(ShaderUtilities.ID_FaceDilate, _outlineParameter);
                _material.SetFloat(ShaderUtilities.ID_OutlineWidth, _outlineParameter);
                return true;
            }
            return false;
        }

        private static float GetMaxOutlineSizeAtPointWithConst(TMP_FontAsset _fontAsset, float _fontSize, float _const)
        {
            float _pointSizeSampling = _fontAsset.faceInfo.pointSize;
            float _padding = _fontAsset.atlasPadding;
            float _ratio = _padding / _pointSizeSampling;
            return _ratio * _const * (_fontSize / 100f);
        }
        #endregion

        public static int ToInt(this bool _value) => _value ? 1 : 0;
        public static float ToFloat(this bool _value) => _value ? 1f : 0f;
        public static void DoActionAtValueChange<T>(ref T _refVariable, T _wantedValue, Action<T> _action)
        {
            if (_refVariable.Equals(_wantedValue)) return;
            _refVariable = _wantedValue;
            _action?.Invoke(_refVariable);
        }

        public static void SwapValues<T>(ref T _a, ref T _b) => (_a, _b) = (_b, _a);

        /// <summary>
        /// Filters an array of MonoBehaviours to only include those that are active in the hierarchy.
        /// </summary>
        public static T[] FilterActiveInHierarchy<T>(this T[] _baseArray) where T : MonoBehaviour
        {
            List<T> _validList = new();
            foreach (T _item in _baseArray)
            {
                if (_item.gameObject.activeInHierarchy)
                {
                    _validList.Add(_item);
                }
            }
            return _validList.ToArray();
        }

        #region Component Methods

        /// <summary>
        /// Gets all components of a certain type on an object and its children.
        /// </summary>
        public static T[] GetComponentsOnObjectAndChildrens<T>(this Component _component) where T : Component
        {
            T[] _onObject = _component.GetComponents<T>();
            T[] _onChildrens = _component.GetComponentsInChildren<T>();
            return UfCollection.MergeArrays(true, _onChildrens, _onObject);
        }

        /// <summary>
        /// Executes an action on a component and all its children of a certain type.
        /// </summary>
        public static void DoEffectOnObjectAndChildrens<T>(this Transform _transform, Action<T> _action) where T : Component
        {
            T[] _components = _transform.GetComponentsInChildren<T>();
            foreach (T _component in _components) { _action?.Invoke(_component); }
        }

        /// <summary>
        /// Gets a component of a certain type on a transform, or adds one if it doesn't exist.
        /// </summary>
        public static T GetOrAddComponent<T>(this Transform _transform) where T : Component => _transform.TryGetComponent(out T _component) ? _component : _transform.AddComponent<T>();

        /// <summary>
        /// Executes an action on a component and all its children of a certain type with a parameter.
        /// </summary>
        public static void DoEffectOnComponentAndChildsWithParameter<T, U>(T _component, U _parameter, Action<T, U> _method, Func<T, U, bool> _validChildMethod = null, bool _ifNotValidIgnoreChilds = true) where T : Component
        {
            bool _isValid = _validChildMethod == null || _validChildMethod.Invoke(_component, _parameter);
            if (_isValid)
                _method.Invoke(_component, _parameter);
            if (_isValid || !_ifNotValidIgnoreChilds)
                for (int _i = 0; _i < _component.transform.childCount; _i++)
                    if (_component.transform.GetChild(_i).TryGetComponent(out T _childComponent))
                        DoEffectOnComponentAndChildsWithParameter(_childComponent, _parameter, _method, _validChildMethod, _ifNotValidIgnoreChilds);
        }

        #endregion

        #region Conversion Methods

        /// <summary>
        /// Converts an array of one type to an array of another type.
        /// </summary>
        public static T[] Convert<U, T>(this U[] _from)
        {
            T[] _values = new T[_from?.Length ?? 0];
            if (_values.Length > 0)
                for (int _i = 0; _i < _values.Length; _i++)
                    _values[_i] = (T)System.Convert.ChangeType(_from[_i], typeof(T));
            return _values;
        }

        /// <summary>
        /// Converts an array of one type to an array of another type using a conversion method.
        /// </summary>
        public static T[] Convert<U, T>(this U[] _from, Func<U, T> _conversionMethod)
        {
            T[] _values = new T[_from?.Length ?? 0];
            if (_values.Length > 0)
                if (_conversionMethod == null) for (int _i = 0; _i < _values.Length; _i++) _values[_i] = default;
                else for (int _i = 0; _i < _values.Length; _i++) _values[_i] = _conversionMethod.Invoke(_from[_i]);
            return _values;
        }

        /// <summary>
        /// Tries to cast an object of one type to another type.
        /// </summary>
        public static bool Cast<U, T>(this U _from, out T _to) where T : class
        {
            _to = _from as T;
            return _to != null;
        }

        #endregion

        /// <summary>
        /// Resets the velocity of a Rigidbody2D.
        /// </summary>
        public static void ResetRigidbodyVelocity(this Rigidbody2D _rigidbody2D)
        {
            _rigidbody2D.velocity = Vector3.zero;
            _rigidbody2D.angularVelocity = 0;
        }

        /// <summary>
        /// Resets the velocity of a Rigidbody.
        /// </summary>
        public static void ResetRigidbodyVelocity(this Rigidbody _rigidbody)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// Creates a root GameObject for a scene and moves all other root GameObjects to be its children.
        /// </summary>
        public static GameObject CreateRootOfScene(Scene _scene)
        {
            GameObject _sceneRoot = new(_scene.name + "_GeneratedRoot");
            _sceneRoot.transform.SetAsRootOfScene(_scene);
            return _sceneRoot;
        }

        public static void SetAsRootOfScene(this Transform _transform, Scene _scene)
        {
            _transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(_transform.gameObject, _scene);
            foreach (GameObject _rootGameObject in _scene.GetRootGameObjects())
                if (_rootGameObject != _transform)
                    _rootGameObject.transform.SetParent(_transform);
        }
        public static bool IsOneOf<T>(this T _value, params T[] _values) => _values.Contains(_value);
        public static bool IsNotOneOf<T>(this T _value, params T[] _values) => !_values.Contains(_value);



        public static void UpdateShapeToSprite(this PolygonCollider2D _collider, Sprite _sprite)
        {
            if (_collider != null && _sprite != null)
            {
                _collider.pathCount = _sprite.GetPhysicsShapeCount();
                List<Vector2> _path = new();
                for (int _i = 0; _i < _collider.pathCount; _i++)
                {
                    _path.Clear();
                    _sprite.GetPhysicsShape(_i, _path);
                    _collider.SetPath(_i, _path.ToArray());
                }
            }
        }
    }
}