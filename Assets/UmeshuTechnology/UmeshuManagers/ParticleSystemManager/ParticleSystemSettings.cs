using System;
using System.Collections.Generic;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.USystem.VFX
{
    public class ParticleSystemSettings
    {
        public string ParticleSystemName { get; private set; }
        private readonly List<Action<ParticleSystem, ParticleSystemSettings>> optionMethods = new();
        public ParticleSystemSettings(string _fxName)
        {
            this.ParticleSystemName = _fxName;
            parent = null;
            usePool = true;
            isImmortal = false;
        }

        public void ApplyMethods(ParticleSystem _particleSystem)
        {
            foreach (Action<ParticleSystem, ParticleSystemSettings> _method in optionMethods)
                _method?.Invoke(_particleSystem, this);
        }

        #region Option Declarations
        public ParticleSystemOption<Vector3> pos = new(SetPosition);
        public ParticleSystemOption<Quaternion> rot = new(SetRotation);
        public ParticleSystemOption<Color> col = new(SetColor);
        public ParticleSystemOption<float> scale = new(SetScale);
        public ParticleSystemOption<Vector3> scaleVector = new(SetScaleFromVector);
        public ParticleSystemOption<float> speed = new(SetSpeed);
        public ParticleSystemOption<Gradient> colGradient = new(SetGradient);
        public ParticleSystemOption<Sprite> sprite = new(SetSprite);
        public ParticleSystemOption<float> radius = new(SetRadius);
        public ParticleSystemOption<float> burst = new(SetBurst);
        public ParticleSystemOption<Vector3> upVector = new(SetUpVector);
        public ParticleSystemOption<Color> trailColor = new(SetTrail);
        public ParticleSystemOption<Vector2> shapeScale = new(SetShapeScale);

        public Transform parent;
        public bool usePool;
        public bool isImmortal;
        #endregion

        #region Use Methods
        public ParticleSystemSettings WithPos(Vector3 _pos) => UseOption(ref pos, _pos);
        public ParticleSystemSettings WithRot(Quaternion _rot) => UseOption(ref rot, _rot);
        public ParticleSystemSettings WithCol(Color _col) => UseOption(ref col, _col);
        public ParticleSystemSettings WithScale(float _scale) => UseOption(ref scale, _scale);
        public ParticleSystemSettings WithScaleVector(Vector3 _scaleVector) => UseOption(ref scaleVector, _scaleVector);
        public ParticleSystemSettings WithSpeed(float _speed) => UseOption(ref speed, _speed);
        public ParticleSystemSettings WithGradient(Gradient _colGradient) => UseOption(ref colGradient, _colGradient);
        public ParticleSystemSettings WithSprite(Sprite _sprite) => UseOption(ref sprite, _sprite);
        public ParticleSystemSettings WithRadius(float _radius) => UseOption(ref radius, _radius);
        public ParticleSystemSettings WithBurst(float _burst) => UseOption(ref burst, _burst);
        public ParticleSystemSettings WithUpVector(Vector3 _upVector) => UseOption(ref upVector, _upVector);
        public ParticleSystemSettings WithTrailColor(Color _trailColor) => UseOption(ref trailColor, _trailColor);
        public ParticleSystemSettings WithPool(bool _usePool) => UseOption(ref usePool, _usePool);
        public ParticleSystemSettings WithParent(Transform _parent) => UseOption(ref parent, _parent);
        public ParticleSystemSettings WithShapeScale(Vector2 _shapeScale) => UseOption(ref shapeScale, _shapeScale);
        #endregion

        #region Use Option Methods
        private ParticleSystemSettings UseOption<T>(ref ParticleSystemOption<T> _particleSystemOption, T _value)
        {
            if (!_particleSystemOption.Enabled)
            {
                _particleSystemOption.Use(_value);
                optionMethods.Add(_particleSystemOption.Method);
            }
            else Debug.LogError($"Tried to call the same ParticleSystem option twice for the ParticleSystem name \"{ParticleSystemName}\", it will only take the first call");
            return this;
        }

        private ParticleSystemSettings UseOption<T>(ref T _particleSystemOption, T _value)
        {
            _particleSystemOption = _value;
            return this;
        }
        #endregion

        #region Set Method
        private static void SetPosition(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings){

            if (_particleSystem.GetComponent<VFX_ParameterTags>().editablePosition)
            {
                _particleSystem.transform.position = _particleSystemSettings.pos.Value;

            }
        }
        private static void SetRotation(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            if (_particleSystem.GetComponent<VFX_ParameterTags>().editableRotation)
            {
                _particleSystem.transform.localRotation = _particleSystemSettings.rot.Value;
            }
        }
        private static void SetScale(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings) => _particleSystem.transform.localScale = _particleSystemSettings.scale.Value * Vector3.one;
        private static void SetScaleFromVector(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings) => _particleSystem.transform.localScale = new Vector3(_particleSystemSettings.scaleVector.Value.x, _particleSystemSettings.scaleVector.Value.y, _particleSystemSettings.scaleVector.Value.z);
        private static void SetUpVector(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings) => _particleSystem.transform.up = _particleSystemSettings.upVector.Value;
        private static void SetShapeScale(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            ParticleSystem.ShapeModule _shape = _particleSystem.shape;
            _shape.scale = _particleSystemSettings.shapeScale.Value;
        }

        #region Set color
        private static void SetColor(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings) => UfObject.DoEffectOnComponentAndChildsWithParameter(_particleSystem, _particleSystemSettings, SetColor_Method, SetColor_Condition);
        private static void SetColor_Method(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            ParticleSystem.MainModule _main = _particleSystem.main;
            _main.startColor = _particleSystemSettings.col.Value;
        }
        private static bool SetColor_Condition(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            return !_particleSystem.CompareTag("NoColorSet");
        }
        #endregion

        #region Set Gradient
        private static void SetGradient(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings) => UfObject.DoEffectOnComponentAndChildsWithParameter(_particleSystem, _particleSystemSettings, SetGradient_Method, SetColor_Condition);
        private static void SetGradient_Method(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            ParticleSystem.MainModule _main = _particleSystem.main;
            _main.startColor = new ParticleSystem.MinMaxGradient(_particleSystemSettings.colGradient.Value.colorKeys[0].color, _particleSystemSettings.colGradient.Value.colorKeys[1].color);
        }
        #endregion

        #region Set Trail
        private static void SetTrail(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings) => UfObject.DoEffectOnComponentAndChildsWithParameter(_particleSystem, _particleSystemSettings, SetTrail_Method, SetColor_Condition);
        private static void SetTrail_Method(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            ParticleSystem.TrailModule _trail = _particleSystem.trails;
            _trail.colorOverLifetime = _particleSystemSettings.trailColor.Value;
        }
        #endregion

        #region Set Speed
        private static void SetSpeed(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            UfObject.DoEffectOnComponentAndChildsWithParameter(_particleSystem, _particleSystemSettings, SetSpeed_Method);
            SetAnimatorSpeed(_particleSystem, _particleSystemSettings);
        }
        private static void SetSpeed_Method(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            ParticleSystem.MainModule _main = _particleSystem.main;
            _main.startSpeedMultiplier *= _particleSystemSettings.speed.Value;
            _main.startDelayMultiplier /= _particleSystemSettings.speed.Value;
        }
        private static void SetAnimatorSpeed(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            if (_particleSystem.TryGetComponent(out Animator _anmtr)) _anmtr.speed *= _particleSystemSettings.speed.Value;
        }
        #endregion

        #region Set Sprite
        private static void SetSprite(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            MaterialPropertyBlock _matProp = new();
            Renderer _fxRender = _particleSystem.GetComponent<Renderer>();
            _particleSystemSettings.sprite.Value.GetTilingAndOffsetFromSprite(out Vector2 _tiling, out Vector2 _offset);
            _fxRender.GetPropertyBlock(_matProp);
            _matProp.SetTexture("_BaseMap", _particleSystemSettings.sprite.Value.texture);
            _matProp.SetVector("_BaseMap_ST", new Vector4(_tiling.x, _tiling.y, _offset.x, _offset.y));
            _fxRender.SetPropertyBlock(_matProp);
        }
        #endregion

        #region Set Radius
        private static void SetRadius(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            ParticleSystem.ShapeModule _shape = _particleSystem.shape;
            _shape.radius = _particleSystemSettings.radius.Value;
        }
        #endregion

        #region Set Burst
        private static void SetBurst(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            if (_particleSystem.GetComponent<VFX_ParameterTags>().scalableBurst)
            {
                ParticleSystem.EmissionModule _emission = _particleSystem.emission;
                ParticleSystem.Burst _burst = new ParticleSystem.Burst();
                _burst.count = _particleSystemSettings.burst.Value;
                _emission.SetBurst(0, _burst);
            }
            
        }
        #endregion

        #endregion
    }


    public class ParticleSystemOption<T>
    {
        public bool Enabled { get; private set; } = false;
        public T Value { get; private set; } = default;
        public Action<ParticleSystem, ParticleSystemSettings> Method { get; private set; }

        public void Use(T _newValue)
        {
            Value = _newValue;
            Enabled = true;
        }

        public ParticleSystemOption(Action<ParticleSystem, ParticleSystemSettings> _method)
        {
            Method = _method;
        }
    }
}