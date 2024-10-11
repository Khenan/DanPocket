using System.Collections.Generic;
using UnityEngine;

namespace Umeshu.Uf
{
    /// <summary>
    /// Provides extension methods for Transform type.
    /// </summary>
    public static class UfTransform
    {
        /// <summary>
        /// Stretches the transform between two points.
        /// </summary>
        /// <param name="_component">The component with the transform to stretch.</param>
        /// <param name="_startPoint">The start point of the stretch.</param>
        /// <param name="_endPoint">The end point of the stretch.</param>
        /// <param name="_width">The width of the stretch.</param>
        /// <param name="_dir">The direction of the stretch. Default is up.</param>
        public static void StretchTransformBetweenToPoint(this Component _component, Vector3 _startPoint, Vector3 _endPoint, float _width, TransformStretchRefVector _dir = TransformStretchRefVector.up) => StretchTransformBetweenToPoint(_component.transform, _startPoint, _endPoint, _width, _dir);

        /// <summary>
        /// Stretches the transform between two points.
        /// </summary>
        /// <param name="_transform">The transform to stretch.</param>
        /// <param name="_startPoint">The start point of the stretch.</param>
        /// <param name="_endPoint">The end point of the stretch.</param>
        /// <param name="_width">The width of the stretch.</param>
        /// <param name="_dir">The direction of the stretch. Default is up.</param>
        public static void StretchTransformBetweenToPoint(this Transform _transform, Vector3 _startPoint, Vector3 _endPoint, float _width, TransformStretchRefVector _dir = TransformStretchRefVector.up)
        {
            _transform.transform.position = Vector3.Lerp(_startPoint, _endPoint, .5f);
            Vector3 _direction = _endPoint - _startPoint;

            switch (_dir)
            {
                case TransformStretchRefVector.up:
                    _transform.up = _direction;
                    _transform.localScale = new Vector3(_width, _direction.magnitude, _width);
                    break;
                case TransformStretchRefVector.right:
                    _transform.right = _direction;
                    _transform.localScale = new Vector3(_direction.magnitude, _width, _width);
                    break;
                case TransformStretchRefVector.forward:
                    _transform.forward = _direction;
                    _transform.localScale = new Vector3(_width, _width, _direction.magnitude);
                    break;
            }
        }

        /// <summary>
        /// Enum for the direction of the stretch.
        /// </summary>
        public enum TransformStretchRefVector
        {
            up,
            right,
            forward
        }

        public static string GetHierarchyPath<T>(this T _component) where T : Component => GetHierarchyPath(_component.gameObject);

        public static string GetHierarchyPath(this GameObject _go)
        {
            Transform _t = _go.transform;
            string _finalString = _t.name;
            Transform _currTransform = _t;
            int _maxDepth = 10;
            while (_currTransform.parent != null && _maxDepth > 0)
            {
                _finalString = _currTransform.parent + "/" + _finalString;
                _currTransform = _currTransform.parent;
                _maxDepth--;
            }
            return "(Scene:" + _go.scene.name + ")/" + _finalString.Replace(" (UnityEngine.Transform)", "").Replace(" (UnityEngine.RectTransform)", "");
        }
        public static void ResetLocalTransformation(this Transform _transform)
        {
            _transform.localPosition = Vector3.zero;
            _transform.localRotation = Quaternion.identity;
            _transform.localScale = Vector3.one;
        }
        public static void ResetTransformation(this Transform _transform)
        {
            _transform.position = Vector3.zero;
            _transform.rotation = Quaternion.identity;
            _transform.localScale = Vector3.one;
        }

        public static void MoveChildrens(this Transform _from, Transform _to)
        {
            Transform[] _childrens = new Transform[_from.childCount];
            for (int _i = 0; _i < _from.childCount; _i++)
                _childrens[_i] = _from.GetChild(_i);
            for (int _i = 0; _i < _childrens.Length; _i++)
                _childrens[_i].SetParent(_to);
        }
    }
}
