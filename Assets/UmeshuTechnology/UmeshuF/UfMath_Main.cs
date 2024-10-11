using System;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Common;
using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace Umeshu.Uf
{
    public static partial class UfMath
    {
        #region MoveTowards
        public static void MoveTowardsZero(UVar<float> _value, float _deltaTime, Action _actionOnEnd = null) => _value.Value = ReturnMoveTowards(_value.Value, 0, _deltaTime, _actionOnEnd);
        public static void MoveTowardsZero(ref float _value, float _deltaTime, Action _actionOnEnd = null) => _value = ReturnMoveTowards(_value, 0, _deltaTime, _actionOnEnd);
        public static void MoveTowards(UVar<float> _value, float _aimedValue, float _deltaTime, Action _actionOnEnd = null) => _value.Value = ReturnMoveTowards(_value.Value, _aimedValue, _deltaTime, _actionOnEnd);
        public static void MoveTowards(ref float _value, float _aimedValue, float _deltaTime, Action _actionOnEnd = null) => _value = ReturnMoveTowards(_value, _aimedValue, _deltaTime, _actionOnEnd);
        public static float ReturnMoveTowardsZero(float _value, float _deltaTime, Action _actionOnEnd = null) => ReturnMoveTowards(_value, 0, _deltaTime, _actionOnEnd);
        public static float ReturnMoveTowards(float _value, float _aimedValue, float _deltaTime, Action _actionOnEnd = null)
        {
            if (_value == _aimedValue) return _value;
            _value = Mathf.MoveTowards(_value, _aimedValue, _deltaTime);
            if (_value == _aimedValue) _actionOnEnd?.Invoke();
            return _value;
        }
        #endregion

        public static string ToPercentString(this float _value) => (_value * 100).ToString("0") + "%";

        public static float CalculateAreaOfPolygonCollider(this PolygonCollider2D _polygonCollider2D)
        {
            if (_polygonCollider2D == null) return 0;

            Vector2[] _points = _polygonCollider2D.points;
            float _area = 0;

            for (int _i = 0; _i < _points.Length; _i++)
            {
                Vector2 _point1 = _points[_i];
                Vector2 _point2 = _points[(_i + 1) % _points.Length];

                _area += (_point1.x * _point2.y) - (_point1.y * _point2.x);
            }

            _area = Mathf.Abs(_area) / 2;
            return _area;
        }

        public static List<Vector2> GetPointsOfCircle(Vector2 _origin, float _radius, float _resolution = 10f)
        {
            List<Vector2> _points = new();
            float _circleSurface = Mathf.PI * _radius * _radius;
            int _nbPoints = Mathf.CeilToInt(_circleSurface * _resolution);

            for (int _i = 0; _i < _nbPoints; _i++)
            {
                float _angle = 360f * _i / _nbPoints;
                Vector2 _point = _origin + (Quaternion.AngleAxis(_angle, Vector3.forward) * Vector2.right * _radius).ToVector2();
                _points.Add(_point);
            }

            return _points;

        }
        public static List<Vector2> GetPointsOfCone(Vector2 _origin, Vector2 _direction, float _radius, float _angle, float _resolution = 1f)
        {
            List<Vector2> _points = new() { _origin };

            float _circleSurface = Mathf.PI * _radius * _radius;
            float _usedSurface = _circleSurface * _angle / 360f;

            int _nbPoints = Mathf.CeilToInt(_usedSurface * _resolution);

            for (int _i = 0; _i < _nbPoints; _i++)
            {
                float _angleStep = _angle / (_nbPoints - 1f);
                float _angleCurrent = _angleStep * _i - _angle / 2f;

                Vector2 _point = _origin + (Quaternion.AngleAxis(_angleCurrent, Vector3.forward) * _direction.normalized * _radius).ToVector2();
                _points.Add(_point);
            }

            return _points;
        }

        public static bool IsInLayerMask(this int _layer, LayerMask _layermask) => (_layermask & (1 << _layer)) != 0;
        public static bool IsInLayerMask(this GameObject _gameObject, LayerMask _layermask) => _gameObject.layer.IsInLayerMask(_layermask);
        public static bool IsInLayerMask(this Component _component, LayerMask _layermask) => _component.gameObject.IsInLayerMask(_layermask);
        public static bool IsInLayerMask(this Collider2D _collider2D, LayerMask _layermask) => _collider2D.gameObject.IsInLayerMask(_layermask);

        public static void MakeSumOfProbabilityToOne(ref List<float> _probabilities)
        {
            float _totalValue = 0;
            for (int _i = 0; _i < _probabilities.Count; _i++)
            {
                _totalValue += _probabilities[_i];
            }
            float _ratio = 1 / _totalValue;
            for (int _i = 0; _i < _probabilities.Count; _i++)
            {
                _probabilities[_i] *= _ratio;
            }
        }

        public static float Total(this float[] _array)
        {
            float _total = 0;
            foreach (float _item in _array) { _total += _item; }
            return _total;
        }

        public static float RepeatBetween(this float _value, float _min, float _max)
        {
            if (_min == _max) return _min;
            if (_min > _max) UfObject.SwapValues(ref _min, ref _max);
            float _range = _max - _min;
            float _result = _value;
            while (_result < _min) _result += _range;
            while (_result > _max) _result -= _range;
            return _result;
        }

        public static float Remap(this float _x, float _pastA, float _pastB, float _newA, float _newB, bool _clamp = true, Func<float, float> _remapPercentageMethod = null)
        {
            float _percentage = _clamp ? Mathf.InverseLerp(_pastA, _pastB, _x) : (_x - _pastA) / (_pastB - _pastA);
            if (_remapPercentageMethod != null) _percentage = _remapPercentageMethod(_percentage);
            return Mathf.LerpUnclamped(_newA, _newB, _percentage);
        }

        public static float Lerp(this Vector2 _limits, float _lerpValue) => Mathf.Lerp(_limits.x, _limits.y, _lerpValue);

        public static float ClosestTo(this float _value, float _a, float _b) => Mathf.Abs(_value - _a) < Mathf.Abs(_value - _b) ? _a : _b;
        public static float ClosestToZero(float _a, float _b) => ClosestTo(_value: 0f, _a, _b);

        #region Distance Calculation
        private const float DIST_APPRO_ALPHA = 0.947543636291f;
        private const float DIST_APPRO_BETA = 0.392485425092f;
        public static float DistanceXZ(Vector3 _a, Vector3 _b) => Magnitude(new Vector2(_a.x - _b.x, _a.z - _b.z));
        public static float DistanceXY(Vector2 _a, Vector2 _b) => Magnitude(_b - _a);
        public static float Magnitude(Vector2 _v) => _v.x == 0 && _v.y == 0 ? 0.0001f : DIST_APPRO_ALPHA * Mathf.Max(Mathf.Abs(_v.x), Mathf.Abs(_v.y)) + DIST_APPRO_BETA * Mathf.Min(Mathf.Abs(_v.x), Mathf.Abs(_v.y));
        public static float Distance(float _a, float _b) => Mathf.Abs(_a - _b);
        public static float Distance(Component _a, Component _b) => Vector3.Distance(_a.transform.position, _b.transform.position);
        public static float DistanceXY(Component _a, Component _b) => DistanceXY(_a.transform.position, _b.transform.position);
        public static float DistanceXZ(Component _a, Component _b) => DistanceXZ(_a.transform.position, _b.transform.position);
        #endregion

        #region Ease
        public static float EasedInOut(float _x, float _pow = 2) => _x < 0.5 ? Mathf.Pow(2 * _x, _pow) / 2 : 1 - Mathf.Pow(-2 * _x + 2, _pow) / 2;
        public static Vector2 SmoothStep(Vector2 _a, Vector2 _b, float _t) => new(Mathf.SmoothStep(_a.x, _b.x, _t), Mathf.SmoothStep(_a.y, _b.y, _t));
        public static float GetEasedOutValueWithOffset(float _offset, float _x) => _offset + EaseOut(_x) * (1 - _offset);
        public static float GetEasedInValueWithOffset(float _offset, float _x) => _offset + EaseIn(_x) * (1 - _offset);
        public static float EaseOut(float _x, float _pow = 2) => 1 - Mathf.Pow(1 - Mathf.Clamp01(_x), _pow);
        public static float EaseIn(float _x, float _pow = 2) => Mathf.Pow(Mathf.Clamp01(_x), _pow);

        #endregion

        #region Get Average
        public static float GetAverage(IEnumerable<float> _listOfFloat)
        {
            float _totalValue = 0;
            foreach (float _value in _listOfFloat) { _totalValue += _value; }
            return _totalValue / (float)_listOfFloat.Count();
        }
        public static float GetAverageWithValueAdded(IEnumerable<float> _listOfFloat, float _nbOfOne)
        {
            float _totalValue = 0;
            foreach (float _value in _listOfFloat) { _totalValue += _value; }
            _totalValue += _nbOfOne;
            return _totalValue / (float)(_listOfFloat.Count() + _nbOfOne);
        }
        #endregion

        #region Range conversion -1 0 1
        public static float MinusOneOne_To_ZeroOne(float _x) => Mathf.Clamp(_x, -1f, 1f) * .5f + .5f;
        public static float ZeroOne_To_MinusOneOne(float _x) => Mathf.Clamp01(_x) * 2f - 1f;
        #endregion

        #region Sin Cosin
        public static float BackAndForthLerp(float _a, float _b, float _t) => Mathf.Lerp(_a, _b, GetBackAndForth(_t));
        public static float GetBackAndForth(float _t) => GetSin01(2 * Mathf.PI * _t);
        public static float GetSin01(float _time) => MinusOneOne_To_ZeroOne(Mathf.Sin(-Mathf.PI / 2 + _time));
        public static float GetCosin01(float _time) => MinusOneOne_To_ZeroOne(Mathf.Cos(-Mathf.PI / 2 + _time));
        #endregion

        #region GetValueFromTimeAndParameters
        public static float GetValueFromTimeAndParameters(float _time, float _delay, float _fadeTime, float _timeVisible) => GetValueFromTimeAndParameters(_time, _delay, _fadeTime, _timeVisible, out _, out _);
        public static float GetValueFromTimeAndParameters(float _time, float _delay, float _fadeTime, float _timeVisible, out bool _inSecondHalf, out bool _done)
        {
            float _alpha = GetValueFromTimeAndParametersWithSteps(_time, _delay, _fadeTime, out _done, out int _currentStep, out float _currentStepPercentage, _timeVisible);
            _inSecondHalf = _currentStep == 1 || _currentStep == 0 && _currentStepPercentage > .5f;
            return _alpha;
        }
        public static float GetValueFromTimeAndParametersWithSteps(float _time, float _delay, float _fadeTime, out bool _done, out int _currentStep, out float _currentStepPercentage, params float[] _steps)
        {
            _currentStep = -1;
            _currentStepPercentage = 0;
            float _realtime = _time - _delay;
            float _alpha = Mathf.Clamp01(_realtime / _fadeTime);
            float _timeVisible = _steps.Total();

            float _endOfFadeIn = _fadeTime;
            float _startOfFadeOut = _fadeTime + _timeVisible;
            if (_realtime > _startOfFadeOut)
            {
                _alpha = 1 - Mathf.Clamp01((_realtime - _fadeTime - _timeVisible) / _fadeTime);
                _currentStep = _steps.Length;
            }

            bool _inTheMiddleTime = _realtime < _startOfFadeOut && _realtime > _endOfFadeIn;
            if (_inTheMiddleTime)
            {
                float _usedTime = _realtime;
                for (int _stepIndex = 0; _stepIndex < _steps.Length; _stepIndex++)
                {
                    float _step = _steps[_stepIndex];
                    if (_usedTime < _step)
                    {
                        _currentStep = _stepIndex;
                        _currentStepPercentage = _usedTime / _step;
                        break;
                    }
                    else
                    {
                        _usedTime -= _step;
                    }
                }
            }

            _done = _realtime > _fadeTime * 2 + _timeVisible;
            return _alpha;
        }
        #endregion

        #region Sound
        public static float VolumeToDecibel(float _volume) => _volume != 0 ? 20.0f * Mathf.Log10(_volume) : -144.0f;
        public static float DecibelToVolume(float _dB) => Mathf.Pow(10.0f, _dB / 20.0f);
        #endregion

        #region Closest Multiple/Power of X
        /// <summary>
        /// Get the ceil multiple of two
        /// </summary>
        public static float NextPowerOfTwo(float _x) => NextPowerOf(_x, 2);

        /// <summary>
        /// Get the ceil multiple of X
        /// </summary>
        public static float NextPowerOf(float _x, float _pow) => Mathf.Pow(_pow, Mathf.CeilToInt(Mathf.Log(_x, _pow)));
        /// <summary>
        /// Get the floor multiple of two
        /// </summary>
        public static float PreviousPowerOfTwo(float _x) => PreviousPowerOf(_x, 2);

        /// <summary>
        /// Get the floor multiple of X
        /// </summary>
        public static float PreviousPowerOf(float _x, float _pow) => Mathf.Pow(_pow, Mathf.FloorToInt(Mathf.Log(_x, _pow)));

        public static float RandomWithStep(float _min, float _max, float _step)
        {
            float _result = UnityEngine.Random.Range(_min, _max);
            float _closestMultiple = ClosestMultipleOf(_result, _step);
            if (_closestMultiple < _min) _closestMultiple += _step;
            if (_closestMultiple > _max) _closestMultiple -= _step;
            return _closestMultiple;
        }

        public static int RandomWithStep(int _min, int _max, int _step)
        {
            float _floatValue = RandomWithStep((float)_min, _max, _step);
            return Mathf.RoundToInt(_floatValue);
        }
        public static float ClosestMultipleOf(this float _x, float _multiple) => _multiple.ErrorIfZero(ForbiddenValueLog.DivisionPerZero) ? _x : Mathf.Round(_x / _multiple) * _multiple;
        public static int ClosestMultipleOf(this int _x, int _multiple) => Mathf.RoundToInt(ClosestMultipleOf((float)_x, _multiple));
        #endregion

        #region Error Log
        public static bool ErrorIfZero(this int _x, ForbiddenValueLog _errorIfZeroLogType) => ErrorIfForbiddenValue(_x, _errorIfZeroLogType, 0);
        public static bool ErrorIfZero(this float _x, ForbiddenValueLog _errorIfZeroLogType) => ErrorIfForbiddenValue(_x, _errorIfZeroLogType, 0);
        public static bool ErrorIfForbiddenValue(this int _x, ForbiddenValueLog _errorIfZeroLogType, int _forbiddenValue) => ErrorIfForbiddenValue((float)_x, _errorIfZeroLogType, _forbiddenValue);
        public static bool ErrorIfForbiddenValue(this float _x, ForbiddenValueLog _errorIfZeroLogType, float _forbiddenValue)
        {
            if (_x == _forbiddenValue)
            {
                Debug.LogError(_errorIfZeroLogType switch
                {
                    ForbiddenValueLog.DivisionPerZero => "Error : Division per zero",
                    ForbiddenValueLog.LengthIsZero => "Error : Array/List size should not be " + _forbiddenValue,
                    _ => "Error : Value should not be " + _forbiddenValue,
                });
                return true;
            }
            return false;
        }

        public enum ForbiddenValueLog
        {
            Default,
            DivisionPerZero,
            LengthIsZero
        }
        #endregion

        public static Vector2 GetClosestPointOnSegment(this Vector2 _point, Vector2 _segmentStart, Vector2 _segmentEnd)
        {
            Vector2 _segmentDirection = _segmentEnd - _segmentStart;
            float _segmentLength = _segmentDirection.magnitude;
            _segmentDirection.Normalize();

            Vector2 _pointToStart = _point - _segmentStart;
            float _dotProduct = Vector2.Dot(_pointToStart, _segmentDirection);

            // nearest point is inside the segment 
            if (_dotProduct >= 0f && _dotProduct <= _segmentLength)
            {
                Vector2 _closestPoint = _segmentStart + _segmentDirection * _dotProduct;
                return _closestPoint;
            }
            else
            {
                // nearest point is outside the segment 
                Vector2 _closestPointAtStart = Vector2.Distance(_point, _segmentStart) < Vector2.Distance(_point, _segmentEnd) ? _segmentStart : _segmentEnd;
                return _closestPointAtStart;
            }
        }

        public static Vector2 LineIntersectionOnRect(Vector2 _rect, Vector2 _rectPos, Vector2 _aimedPos)
        {
            float _w = _rect.x / 2;
            float _h = _rect.y / 2;

            float _dx = _aimedPos.x - _rectPos.x;
            float _dy = _aimedPos.y - _rectPos.y;

            //if A=B return B itself
            if (_dx == 0 && _dy == 0) return _rectPos;

            float _tan_phi = _h / _w;
            float _tan_theta = Mathf.Abs(_dy / _dx);

            //tell me in which quadrant the A point is
            float _qx = Mathf.Sign(_dx);
            float _qy = Mathf.Sign(_dy);

            Vector2 _intersectionPoint;
            if (_tan_theta > _tan_phi)
            {
                float _x = _rectPos.x + (_h / _tan_theta) * _qx;
                float _y = _rectPos.y + _h * _qy;
                _intersectionPoint = new Vector2(_x, _y);
            }
            else
            {
                float _x = _rectPos.x + _w * _qx;
                float _y = _rectPos.y + _w * _tan_theta * _qy;
                _intersectionPoint = new Vector2(_x, _y);
            }
            return _intersectionPoint;
        }
        
        public static int ConvertToBitMask_Int(params bool[] _bools) => ConvertToBitMask(_maxBool: 32, _bools);
        public static byte ConvertToBitMask_Byte(params bool[] _bools) => (byte)ConvertToBitMask(_maxBool: 8, _bools);
        private static int ConvertToBitMask(uint _maxBool, params bool[] _bools)
        {
            if (_bools.Length > _maxBool) throw new Exception($"Too many bools for the bit mask : Array length is {_bools.Length} and max is {_maxBool}");
            int _bitmask = 0;
            for (int _i = 0; _i < _bools.Length; _i++)
                if (_bools[_i])
                    _bitmask |= 1 << _i;
            return _bitmask;
        }

        public static bool[] ConvertFromBitMask_Int(int _bitmask, int _size) => ConvertFromBitMask(_bitmask, _size); // Max 32 bools
        public static bool[] ConvertFromBitMask_Byte(byte _bitmask, int _size) => ConvertFromBitMask(_bitmask, _size); // Max 8 bools
        private static bool[] ConvertFromBitMask(int _bitmask, int _size)
        {
            bool[] _bools = new bool[_size];
            for (int _i = 0; _i < _size; _i++)
                _bools[_i] = ((_bitmask) & (1 << _i)) != 0;
            return _bools;
        }

        public static Vector2 GetClockwisePerpendicular(this Vector2 _vector) => _vector.ToVector3().GetClockwisePerpendicular().ToVector2();
        public static Vector2 GetCounterClockwisePerpendicular(this Vector2 _vector) => _vector.ToVector3().GetCounterClockwisePerpendicular().ToVector2();
        public static Vector3 GetClockwisePerpendicular(this Vector3 _vector) => Quaternion.AngleAxis(-90, Vector3.forward) * _vector;
        public static Vector3 GetCounterClockwisePerpendicular(this Vector3 _vector) => Quaternion.AngleAxis(90, Vector3.forward) * _vector;

        public static float WithRandomSign(this float _value, float _negativeProbability = 0.5f) => (UnityEngine.Random.value < _negativeProbability ? -1 : 1) * Mathf.Abs(_value);
        public static int WithRandomSign(this int _value, float _negativeProbability = 0.5f) => Mathf.RoundToInt(WithRandomSign((float)_value, _negativeProbability));

        public static bool IsBetween(this float _value, float _min, float _max) => _value >= _min && _value <= _max;

        public static bool IsNearZero(this float _value, float _epsilon = 0.0001f) => Mathf.Abs(_value) < _epsilon;
        public static bool IsNear(this float _value, float _target, float _epsilon = 0.0001f) => Mathf.Abs(_value - _target) < _epsilon;

        public static int IntLength(this int _i) => _i == 0 ? 1 : (int)Math.Floor(Math.Log10(Mathf.Abs(_i))) + 1;
    }
}
