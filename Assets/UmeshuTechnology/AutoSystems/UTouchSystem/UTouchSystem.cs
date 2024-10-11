using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;
using Umeshu.Utility;
using UnityEngine;

namespace Umeshu.USystem.TouchInput
{
    using Time = UnityEngine.Time;

    [AutoCreateInstance]
    public class UTouchSystem : SingletonUpdatedSystem<UTouchSystem>
    {

        #region External Methods

        public static bool InputDown(out Vector2 _position, bool _allowStartedOnUI)
        {
            _position = Vector2.zero;
            if (Instance == null) return false;

            foreach (UTouch _touch in GetFilteredTouches(_allowStartedOnUI))
                if (_touch.baseTouch.phase == TouchPhase.Began && _touch.valid) { _position = _touch.baseTouch.position; return true; }
            return false;
        }


        public static bool InputHoldWithPositions(out Vector2 _startPosition, out Vector2 _currentPosition, bool _allowStartedOnUI) => InputHold(out _startPosition, out _currentPosition, out _, out _, out _, _allowStartedOnUI);
        public static bool InputHoldWithDelta(out Vector2 _delta, bool _allowStartedOnUI) => InputHold(out _, out _, out _delta, out _, out _, _allowStartedOnUI);
        public static bool InputHoldWithVelocity(out Vector2 _smoothedVelocity, bool _allowStartedOnUI) => InputHold(out _, out _, out _, out _smoothedVelocity, out _, _allowStartedOnUI);
        public static bool InputHoldWithDuration(out float _duration, bool _allowStartedOnUI) => InputHold(out _, out _, out _, out _, out _duration, _allowStartedOnUI);
        public static bool InputHold(out Vector2 _startPosition, out Vector2 _currentPosition, out Vector2 _delta, out Vector2 _smoothedVelocity, out float _duration, bool _allowStartedOnUI)
        {
            _startPosition = Vector2.zero;
            _currentPosition = Vector2.zero;
            _delta = Vector2.zero;
            _smoothedVelocity = Vector2.zero;
            _duration = 0;
            if (Instance == null) return false;

            foreach (UTouch _touch in GetFilteredTouches(_allowStartedOnUI))
                if (_touch.InputHold(out _startPosition, out _currentPosition, out _delta, out _smoothedVelocity, out _duration)) return true;
            return false;
        }

        public static bool InputUp(out Vector2 _position, bool _allowStartedOnUI)
        {
            _position = Vector2.zero;
            if (Instance == null) return false;

            foreach (UTouch _touch in GetFilteredTouches(_allowStartedOnUI))
                if (_touch.baseTouch.phase == TouchPhase.Ended && _touch.valid) { _position = _touch.baseTouch.position; return true; }
            return false;
        }

        public static bool InputNotPressed() => Instance == null || !Instance.touchMap.Values.Any(_touch => _touch.valid);

        public static bool InputClick(out Vector2 _position, bool _allowStartedOnUI)
        {
            _position = Vector2.zero;
            if (Instance == null) return false;

            foreach (UTouch _touch in GetFilteredTouches(_allowStartedOnUI))
                if (_touch.InputClick(out _position)) return true;
            return false;
        }

        private static List<UTouch> GetTouchesThatDidntStartOnUI() => Instance.touchMap.Values.Where(_touch => !_touch.startedOnUI).ToList();
        private static List<UTouch> GetFilteredTouches(bool _allowStartedOnUI) => _allowStartedOnUI ? Instance.touchMap.Values.ToList() : GetTouchesThatDidntStartOnUI();


        #endregion

        #region Logic

        private const float CLICK_DURATION = 0.2f;
        private const float STATIONARY_MAX_MOVE_PIXEL_NON_SQRT = 50;
        private const int MAX_VELOCITY_STOCK = 30;

        private readonly Dictionary<int, UTouch> touchMap = new();

        protected override void UpdateMethod()
        {
            if (!touchMap.ContainsKey(-1)) touchMap.Add(-1, new UTouch() { baseTouch = new() { fingerId = -1 } });

            foreach (Touch _touch in Input.touches)
            {
                int _index = _touch.fingerId;
                if (_touch.phase == TouchPhase.Began)
                {
                    UTouch _uTouch = new() { baseTouch = _touch };
                    HandleTouchDown(_uTouch);
                    touchMap.SetValue(_index, _uTouch);
                }
                else
                {
                    UTouch _existingUTouch = touchMap[_index];
                    _existingUTouch.baseTouch = _touch;
                }
            }

            foreach (UTouch _touch in touchMap.Values.ToArray())
            {
                if (_touch.IsMouseTouch) HandleMouseValidation(_touch);

                if (!_touch.valid)
                {
                    if (!_touch.IsMouseTouch) touchMap.Remove(_touch.baseTouch.fingerId);
                    continue;
                }

                if (_touch.IsMouseTouch) HandleMouseTouch(_touch);
                else HandleFingerTouch(_touch);

                if (_touch.baseTouch.phase.IsOneOf(TouchPhase.Stationary, TouchPhase.Moved))
                {
                    _touch.lastVelocities.Add(_touch.baseTouch.deltaPosition / _touch.baseTouch.deltaTime);
                    if (_touch.lastVelocities.Count > MAX_VELOCITY_STOCK) _touch.lastVelocities.RemoveAt(0);

                    _touch.averageVelocity = Vector2.zero;
                    foreach (Vector2 _velocity in _touch.lastVelocities)
                        _touch.averageVelocity += _velocity / (float)_touch.lastVelocities.Count;
                }

                _touch.duration = Time.unscaledTime - _touch.startTime;
                if (_touch.baseTouch.phase == TouchPhase.Moved && !_touch.inputMoved) _touch.inputMoved = true;
            }
        }

        private void HandleMouseValidation(UTouch _touch) => _touch.valid = Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0);

        private void HandleMouseTouch(UTouch _touch)
        {
            _touch.baseTouch.deltaPosition = Input.mousePosition.ToVector2() - _touch.baseTouch.position;
            _touch.baseTouch.deltaTime = Time.unscaledDeltaTime;

            _touch.baseTouch.position = Input.mousePosition;
            _touch.baseTouch.rawPosition = Input.mousePosition;

            _touch.baseTouch.phase = Input.GetMouseButtonDown(0) ?
                TouchPhase.Began :
                Input.GetMouseButtonUp(0) ? TouchPhase.Ended
                : (_touch.baseTouch.position - _touch.startPosition).sqrMagnitude > STATIONARY_MAX_MOVE_PIXEL_NON_SQRT ? TouchPhase.Moved :
                TouchPhase.Stationary;

            if (_touch.baseTouch.phase == TouchPhase.Began) HandleTouchDown(_touch);
            if (_touch.baseTouch.phase == TouchPhase.Ended)
            {
                _touch.lastVelocities.Clear();
                _touch.averageVelocity = Vector2.zero;
            }
        }

        private void HandleFingerTouch(UTouch _touch)
        {
            _touch.valid = _touch.baseTouch.phase != TouchPhase.Ended;
        }

        private void HandleTouchDown(UTouch _touch)
        {
            _touch.startTime = Time.unscaledTime;
            _touch.startedOnUI = UfMouse.IsScreenPositionOverUI(_touch.baseTouch.position);
            _touch.inputMoved = false;
            _touch.startPosition = _touch.baseTouch.position;
        }

        public class UTouch
        {
            public Touch baseTouch;
            public float startTime;
            public float duration;
            public bool valid = true;
            public bool startedOnUI;
            public bool inputMoved = false;
            public Vector2 startPosition = Vector2.zero;
            public Vector2 averageVelocity = Vector2.zero;
            public List<Vector2> lastVelocities = new();

            public bool IsMouseTouch => baseTouch.fingerId == -1;

            private bool ValidForClick => duration < CLICK_DURATION && !inputMoved && (!IsMouseTouch || valid);

            public bool InputClick(out Vector2 _position)
            {
                _position = baseTouch.position;
                return baseTouch.phase == TouchPhase.Ended && ValidForClick;
            }

            public bool InputHold(out Vector2 _startPosition, out Vector2 _currentPosition, out Vector2 _delta, out Vector2 _smoothedVelocity, out float _duration)
            {
                _startPosition = startPosition;
                _currentPosition = baseTouch.position;
                _delta = baseTouch.deltaPosition;
                _smoothedVelocity = averageVelocity;
                _duration = duration;
                return baseTouch.phase.IsOneOf(TouchPhase.Moved, TouchPhase.Stationary);
            }
        }
        #endregion

    }
}
