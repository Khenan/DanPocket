using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Umeshu.Uf
{
    public static class UfMouse
    {

        public static bool IsMouseOverLayer2D(Vector2 _inputPosition, Camera _camera, LayerMask _layerMask, out RaycastHit2D _raycastHit2D)
        {
            Vector2 _mousePos = _camera.GetInputWorldPosition(_inputPosition);
            _raycastHit2D = Physics2D.Raycast(_mousePos, Vector2.zero, Mathf.Infinity, _layerMask);
            return _raycastHit2D.collider != null;
        }

        public static bool IsMouseOverLayer3D(Vector2 _inputPosition, Camera _camera, LayerMask _layerMask, out RaycastHit _raycastHit)
        {
            Ray _ray = _camera.ScreenPointToRay(_inputPosition);
            return Physics.Raycast(_ray, out _raycastHit, Mathf.Infinity, _layerMask);
        }

        public static bool TryGetComponentWithRaycast2D<T>(Vector2 _inputPosition, Camera _camera, LayerMask _layerMask, out T _component)
        {
            if (IsMouseOverLayer2D(_inputPosition, _camera, _layerMask, out RaycastHit2D _hit))
            {
                _component = _hit.collider.GetComponent<T>();
                return _component != null;
            }
            _component = default;
            return false;
        }

        public static bool TryGetComponentWithRaycast3D<T>(Vector2 _inputPosition, Camera _camera, LayerMask _layerMask, out T _component)
        {
            if (IsMouseOverLayer3D(_inputPosition, _camera, _layerMask, out RaycastHit _hit))
            {
                _component = _hit.collider.GetComponent<T>();
                return _component != null;
            }
            _component = default;
            return false;
        }

        public static bool IsScreenPositionOverUI(Vector2 _screenPosition)
        {
            if (EventSystem.current == null)
            {
                "EventSystem.current is null".LogError();
                return false;
            }
            PointerEventData _pointerData = new(EventSystem.current) { pointerId = -1, position = _screenPosition };
            List<RaycastResult> _results = new();
            EventSystem.current.RaycastAll(_pointerData, _results);
            return _results.Count > 0;
        }

        public static Vector2 GetInputWorldPosition(this Camera _camera, Vector2 _inputPosition) => _camera.ScreenToWorldPoint(_inputPosition);

    }
}
