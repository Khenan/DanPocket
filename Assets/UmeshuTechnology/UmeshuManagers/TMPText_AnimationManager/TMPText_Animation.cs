using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.USystem.TextAnimation
{
    /// <summary>
    /// How to create a new prebuild text animation?
    /// <list type="number">
    /// <item>Create a new <see cref="TMPText_Animation"/> in "TMPText_AnimationManager/PreBuildAnimations"</item>
    /// <item>Modify <see cref="ApplyOnData"/>"</item>
    /// <item>Add a new <see cref="AnimationType"/> for the animation and use it to define <see cref="TMPText_Animation.Animation"/></item>
    /// <item>Add new "case" in <see cref="TMPText_AnimationManager.FillPrebuildAnimationSlots"/>"</item>
    /// <item>Go modify the prefab that contain <see cref="TMPText_AnimationManager"/>  and press [Fill Prebuild Animation Slots]</item>
    /// </list>
    /// </summary>
    public abstract class TMPText_Animation : ScriptableObject
    {
        #region Constants Parameter

        private const string PARAMETER_TO_REPLACE_TIME = "time";
        protected const string PARAMETER_TIME = "t";

        private const string PARAMETER_TO_REPLACE_CHARACTER_APPARITION_OFFSET = "characterApparitionOffset";
        protected const string PARAMETER_CHARACTER_APPARITION_OFFSET = "cao";

        private const string PARAMETER_TO_REPLACE_START_SCALE = "startScale";
        protected const string PARAMETER_START_SCALE = "ss";

        private const string PARAMETER_TO_REPLACE_START_ALPHA = "startAlpha";
        protected const string PARAMETER_START_ALPHA = "sa";

        private const string PARAMETER_TO_REPLACE_AMPLITUDE = "amplitude";
        protected const string PARAMETER_AMPLITUDE = "a";

        private const string PARAMETER_TO_REPLACE_FREQUENCY = "frequency";
        protected const string PARAMETER_FREQUENCY = "f";

        private const string PARAMETER_TO_REPLACE_CHARACTER_OFFSET = "characterOffset";
        protected const string PARAMETER_CHARACTER_OFFSET = "co";

        private const string PARAMETER_TO_REPLACE_PERCENTAGE_VISIBLE = "percentageVisible";
        protected const string PARAMETER_PERCENTAGE_VISIBLE = "pv";

        private const string PARAMETER_TO_REPLACE_SPEED = "speed";
        protected const string PARAMETER_SPEED = "s";

        #endregion

        /// <summary>
        /// Used to define the type of animation in a new <see cref="TMPText_Animation"/>.
        /// It is then checked in <see cref="TMPText_AnimationManager"/>.
        /// </summary>
        protected internal enum AnimationType
        {
            Custom,
            Rotation,
            Scale,
            CharacterAppearance,
            Blink,
            Wave,
            WaveX,
            RotationWiggle,
        }

        public static CultureInfo FileCultureInfo => CultureInfo.CreateSpecificCulture("fr-FR");
        private Dictionary<string, string> parameters = new();
        protected abstract AnimationType Animation { get; }
        protected internal virtual string Key => Animation.ToString();

        internal void Animate(TMPText_AnimationData _animationData, Dictionary<string, string> _parameters)
        {
            parameters = _parameters;
            ApplyOnData(_animationData);
        }

        protected abstract void ApplyOnData(TMPText_AnimationData _animationData);

        protected float TryGetParameterValue_Float(string _parameterName, float _defaultValue)
        {
            if (parameters != null && parameters.ContainsKey(_parameterName))
            {
                string _parameterValue = parameters[_parameterName];

                bool _parseSuccess = float.TryParse(_parameterValue, NumberStyles.Any, FileCultureInfo, out float _result);
                return _parseSuccess ? _result : _defaultValue;
            }

            return _defaultValue;
        }

        protected int TryGetParameterValue_Int(string _parameterName, int _defaultValue) => parameters != null && parameters.ContainsKey(_parameterName) && int.TryParse(parameters[_parameterName], NumberStyles.Any, FileCultureInfo, out int _result).LogErrorIfFalse($"Couldn't parse {parameters[_parameterName]} into an int") ? _result : _defaultValue;

        protected bool TryGetParameterValue_Bool(string _parameterName, bool _defaultValue) => parameters != null && parameters.ContainsKey(_parameterName) && bool.TryParse(parameters[_parameterName], out bool _result).LogErrorIfFalse($"Couldn't parse {parameters[_parameterName]} into a bool") ? _result : _defaultValue;

        protected string TryGetParameterValue_String(string _parameterName, string _defaultValue) => parameters != null && parameters.ContainsKey(_parameterName) ? parameters[_parameterName] : _defaultValue;

        protected Color TryGetParameterValue_Color(string _parameterName, Color _defaultValue) => parameters != null && parameters.ContainsKey(_parameterName) && ColorUtility.TryParseHtmlString(parameters[_parameterName], out Color _result).LogErrorIfFalse($"Couldn't parse {parameters[_parameterName]} into a color") ? _result : _defaultValue;

        internal static string ReplaceParametersNames(string _effect)
        {
            _effect = ReplaceCustomAnimationParameterName(_effect, PARAMETER_TO_REPLACE_TIME, PARAMETER_TIME);
            _effect = ReplaceCustomAnimationParameterName(_effect, PARAMETER_TO_REPLACE_CHARACTER_APPARITION_OFFSET, PARAMETER_CHARACTER_APPARITION_OFFSET);
            _effect = ReplaceCustomAnimationParameterName(_effect, PARAMETER_TO_REPLACE_START_SCALE, PARAMETER_START_SCALE);
            _effect = ReplaceCustomAnimationParameterName(_effect, PARAMETER_TO_REPLACE_START_ALPHA, PARAMETER_START_ALPHA);
            _effect = ReplaceCustomAnimationParameterName(_effect, PARAMETER_TO_REPLACE_AMPLITUDE, PARAMETER_AMPLITUDE);
            _effect = ReplaceCustomAnimationParameterName(_effect, PARAMETER_TO_REPLACE_FREQUENCY, PARAMETER_FREQUENCY);
            _effect = ReplaceCustomAnimationParameterName(_effect, PARAMETER_TO_REPLACE_CHARACTER_OFFSET, PARAMETER_CHARACTER_OFFSET);
            _effect = ReplaceCustomAnimationParameterName(_effect, PARAMETER_TO_REPLACE_PERCENTAGE_VISIBLE, PARAMETER_PERCENTAGE_VISIBLE);
            _effect = ReplaceCustomAnimationParameterName(_effect, PARAMETER_TO_REPLACE_SPEED, PARAMETER_SPEED);
            return _effect;
        }

        internal static string ReplaceCustomAnimationParameterName(string _effect, string _parameterToReplace, string _parameterNewName)
        {
            string _replacedEffect = _effect.Replace(_parameterToReplace + TMPText_Animator.PARAMETER_VALUE_SEPARATOR, _parameterNewName + TMPText_Animator.PARAMETER_VALUE_SEPARATOR);
#if UNITY_EDITOR
            if (_effect != _replacedEffect && !Application.isPlaying)
                $"Replaced {_parameterToReplace.Bold().Quote().Color(Color.red)} by {_parameterNewName.Bold().Quote().Color(Color.green)} in {_effect.Bold().Quote().Color(Color.red)} to {_replacedEffect.Bold().Quote().Color(Color.green)}".Log();
#endif
            return _replacedEffect;
        }
    }
}