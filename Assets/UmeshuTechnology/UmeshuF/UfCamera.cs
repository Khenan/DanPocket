using UnityEngine;

namespace Umeshu.Uf
{
    /// <summary>
    /// Provides utility methods for working with cameras.
    /// </summary>
    public static class UfCamera
    {
        /// <summary>
        /// Gets the vertical size of the camera's view.
        /// </summary>
        public static float GetVerticalCamSize(this Camera _camera, float _focusPointZ = 0)
        {
            return _camera.orthographic
                ? Mathf.Abs(_camera.orthographicSize) * 2
                : GetVerticalCamSize(_camera.fieldOfView, Mathf.Abs(_focusPointZ - _camera.transform.position.z));
        }

        /// <summary>
        /// Gets the vertical size of the camera's view based on the field of view and distance.
        /// </summary>
        public static float GetVerticalCamSize(float _fieldOfView, float _distance)
        {
            float _a = Mathf.Abs(_distance);
            float _theta = _fieldOfView * Mathf.Deg2Rad;
            float _o = 2 * _a * Mathf.Tan(_theta * .5f);
            _o = Mathf.Abs(_o);
            return _o;
        }

        /// <summary>
        /// Gets the horizontal size of the camera's view.
        /// </summary>
        public static float GetHorizontalCamSize(this Camera _camera, float _focusPointZ = 0) => GetHorizontalCamSize(GetVerticalCamSize(_camera, _focusPointZ));

        /// <summary>
        /// Gets the horizontal size of the camera's view based on the vertical size.
        /// </summary>
        public static float GetHorizontalCamSize(float _verticalSize) => Mathf.Abs(_verticalSize) * Screen.width / Screen.height;

        /// <summary>
        /// Gets the size of the camera's view.
        /// </summary>
        public static Vector2 GetCameraSize(this Camera _camera) => new(GetHorizontalCamSize(_camera), GetVerticalCamSize(_camera));

        /// <summary>
        /// Gets the world bounds of the camera's view.
        /// </summary>
        public static void GetCameraWorldBounds(this Camera _camera, out Vector2 _cameraHorizontalBounds, out Vector2 _cameraVerticalBounds, float _focusPointZ = 0)
        {
            float _verticalSize = GetVerticalCamSize(_camera, _focusPointZ);
            float _horizontalSize = GetHorizontalCamSize(_verticalSize);

            float _halfSizeX = _horizontalSize / 2f;
            float _halfSizeY = _verticalSize / 2f;
            _cameraHorizontalBounds = new(_camera.transform.position.x - _halfSizeX, _camera.transform.position.x + _halfSizeX);
            _cameraVerticalBounds = new(_camera.transform.position.y - _halfSizeY, _camera.transform.position.y + _halfSizeY);
        }

        /// <summary>
        /// Checks if a position is within the camera's view range.
        /// </summary>
        public static bool IsInCamRange(this Camera _camera, Vector2 _position, float _boundSizeAdded)
        {
            GetCameraWorldBounds(_camera, out Vector2 _cameraHorizontalBounds, out Vector2 _cameraVerticalBounds);

            Vector2 _horizontalBounds = _position.x * Vector2.one + new Vector2(-_boundSizeAdded, _boundSizeAdded);
            Vector2 _verticalBounds = _position.y * Vector2.one + new Vector2(-_boundSizeAdded, _boundSizeAdded);

            bool _instersection_horizontal = _cameraHorizontalBounds.y > _horizontalBounds.x && _cameraHorizontalBounds.x < _horizontalBounds.y;
            bool _instersection_vertical = _cameraVerticalBounds.y > _verticalBounds.x && _cameraVerticalBounds.x < _verticalBounds.y;
            return _instersection_horizontal && _instersection_vertical;
        }


        public static void PlaceCameraAsPerspectiveFromOrthographicSize(this Camera _camera, float _focusZPosition)
        {
            if (_camera.orthographic) return;
            _camera.transform.SetPosWith(_z: (-1 / Mathf.Tan(Mathf.Deg2Rad * _camera.fieldOfView * .5f) * _camera.orthographicSize) + _focusZPosition);
        }
    }
}
