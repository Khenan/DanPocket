using System;
using Umeshu.Uf;
using Umeshu.USystem.GameData;
using Umeshu.USystem.TouchInput;
using UnityEngine;

namespace Umeshu.Utility
{
    [RequireComponent(typeof(Camera))]
    public class CameraScroll : HeritableGameElement
    {
        public enum CameraScrollType { Horizontal, Vertical }
        [SerializeField] private CameraScrollType cameraScrollType;
        [SerializeField] private OptionalVar<float> overrideCameraSizeForCalculations = new(8, false);

        private Camera scrolledCamera;
        private float cameraVelocity = 0;
        public float CameraVelocity => cameraVelocity;
        public Camera Camera => scrolledCamera;

        private float cameraLimitMin = 0;
        private float cameraLimitMax = 100;
        private bool allowMovement = false;
        private CameraScrollRuntimeData cameraScrollRuntimeData;
        private bool hasMovedAndNotReleased = false;
        private Vector3? aimedPosition = null;
        private bool autoMovingIsPreventingInputs = false;
        private bool autoReActivateInputsAfterAutoMoving = false;

        private bool CanInput => allowMovement && !autoMovingIsPreventingInputs;
        private bool IsAutoMoving => aimedPosition != null;

        private float HorizontalCamSize => overrideCameraSizeForCalculations.Enabled ? overrideCameraSizeForCalculations.Value : scrolledCamera.GetHorizontalCamSize();
        private float VerticalCamSize => overrideCameraSizeForCalculations.Enabled ? overrideCameraSizeForCalculations.Value : scrolledCamera.GetVerticalCamSize();

        public void SetPosition(float _position)
        {
            UpdateCameraFromComponent();
            scrolledCamera.transform.position = cameraScrollType == CameraScrollType.Horizontal
                ? scrolledCamera.transform.position.With(_x: _position)
                : scrolledCamera.transform.position.With(_y: _position);
            HandleCameraLimits();
        }

        public void SetPosition(Vector3 _position)
        {
            UpdateCameraFromComponent();
            scrolledCamera.transform.position = _position;
            HandleCameraLimits();
        }
        public void SetScrollType(CameraScrollType _scrollType)
        {
            cameraScrollType = _scrollType;
        }

        public void SetLimits(float _cameraPosMin, float _cameraPosMax)
        {
            cameraLimitMin = _cameraPosMin;
            cameraLimitMax = _cameraPosMax;
            HandleCameraLimits();
        }

        private void ResetCameraMovementVariables() => cameraVelocity = 0;
        public CameraScrollType GetCameraScrollType() => cameraScrollType;

        public void UpdateCameraFromComponent()
        {
            if (scrolledCamera == null)
                scrolledCamera = GetComponent<Camera>();
        }

        private bool init;
        protected override void GameElementFirstInitialize()
        {
            init = true;
            UpdateCameraFromComponent();
            cameraScrollRuntimeData = GameDataManager.GetData<CameraScrollRuntimeData>(new());
            cameraScrollRuntimeData.currentCameraScroll = this;
            cameraScrollRuntimeData.showUI = true;
            aimedPosition = null;
        }

        protected override void GameElementEnableAndReset()
        {
            HandleCameraLimits();
            ResetCameraMovementVariables();
        }

        protected override void GameElementPlay() { }

        protected override void GameElementUpdate()
        {
            if(!init)
            {
                Debug.LogError("CameraScroll not initialized", this);
                Debug.LogError("Parent game element: " + hierarchy.Parent.Value, hierarchy.Parent.Value as MonoBehaviour);
                return;
            }
            if (!allowMovement)
            {
                cameraScrollRuntimeData.showUI = false;
                ResetCameraMovementVariables();
                return;
            }

            HandleMoveTo();

            bool _isPressing = UTouchSystem.InputHoldWithDuration(out float _duration, false);
            bool _isPressingForAWhile = _isPressing && _duration > 1;
            bool _cameraIsSlow = Mathf.Abs(CameraVelocity) < 1f;
            if (!_cameraIsSlow) hasMovedAndNotReleased = true;
            if (!_isPressing) hasMovedAndNotReleased = false;

            cameraScrollRuntimeData.showUI = _cameraIsSlow && !_isPressingForAWhile && !hasMovedAndNotReleased && aimedPosition == null && CanInput;

            if (!IsAutoMoving && autoReActivateInputsAfterAutoMoving) autoMovingIsPreventingInputs = false;

            if (CanInput)
            {
                if (UTouchSystem.InputClick(out _, false)) ResetCameraMovementVariables();
                HandleCameraDragMovement();
            }

            HandleCameraLimits();
        }

        private void HandleMoveTo()
        {
            if (!IsAutoMoving) return;
            scrolledCamera.transform.position = Vector3.Lerp(scrolledCamera.transform.position, aimedPosition.Value, 5f * Time.deltaTime);
            if (Vector3.Distance(scrolledCamera.transform.position, aimedPosition.Value) < .1f) aimedPosition = null;
            ResetCameraMovementVariables();
        }

        private void HandleCameraDragMovement()
        {
            float? _cameraMovementThisFrame = null;
            if (UTouchSystem.InputNotPressed()) cameraVelocity = Mathf.Lerp(cameraVelocity, 0, 2f * Time.deltaTime);
            else if (UTouchSystem.InputHold(out _, out _, out Vector2 _delta, out Vector2 _smoothedVelocity, out _, false))
            {
                cameraVelocity = -(cameraScrollType == CameraScrollType.Horizontal
                    ? _smoothedVelocity.x / Screen.width * HorizontalCamSize
                    : _smoothedVelocity.y / Screen.height * VerticalCamSize);

                _cameraMovementThisFrame = -(cameraScrollType == CameraScrollType.Horizontal
                    ? _delta.x / Screen.width * HorizontalCamSize
                    : _delta.y / Screen.height * VerticalCamSize);

                aimedPosition = null;
            }

            float _cameraMovement = _cameraMovementThisFrame ?? cameraVelocity * Time.deltaTime;
            Vector3 _movement = (cameraScrollType == CameraScrollType.Horizontal ? Vector3.right : Vector3.up) * _cameraMovement;
            scrolledCamera.transform.position += _movement;
        }

        private void HandleCameraLimits()
        {
            if (scrolledCamera == null) return;

            float _refPosition = cameraScrollType == CameraScrollType.Horizontal ? scrolledCamera.transform.position.x : scrolledCamera.transform.position.y;
            float _refCameraSize = cameraScrollType == CameraScrollType.Horizontal ? HorizontalCamSize : VerticalCamSize;
            float? _positionRequested = null;
            bool _isLargerThanLimit = cameraLimitMax - cameraLimitMin < _refCameraSize;
            if (_isLargerThanLimit) return;

            if (_refPosition - _refCameraSize / 2f < cameraLimitMin)
            {
                _positionRequested = cameraLimitMin + _refCameraSize / 2f;
                cameraVelocity = Mathf.Max(0, cameraVelocity);
                aimedPosition = null;
            }

            if (_refPosition + _refCameraSize / 2f > cameraLimitMax)
            {
                _positionRequested = cameraLimitMax - _refCameraSize / 2f;
                cameraVelocity = Mathf.Min(0, cameraVelocity);
                aimedPosition = null;
            }

            if (_positionRequested.HasValue)
                scrolledCamera.transform.position = cameraScrollType == CameraScrollType.Horizontal
                    ? scrolledCamera.transform.position.With(_x: _positionRequested.Value)
                    : scrolledCamera.transform.position.With(_y: _positionRequested.Value);
        }

        public void AllowMovement(bool _value) => allowMovement = _value;
        public bool IsMovementAllowed() => allowMovement;

        public void GoTo(Vector3 _worldPosition)
        {
            aimedPosition = _worldPosition;
        }

        public void GoToWithInputConditions(Vector3 _worldPosition, bool _preventInputs, bool _autoReActivate)
        {
            autoMovingIsPreventingInputs = _preventInputs;
            autoReActivateInputsAfterAutoMoving = _autoReActivate;
            GoTo(_worldPosition);
        }

        public void FreeAutoMovingInputPrevention()
        {
            autoMovingIsPreventingInputs = false;
            autoReActivateInputsAfterAutoMoving = true;
        }
    }
}
