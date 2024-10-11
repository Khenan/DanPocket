using System.Collections.Generic;
using Umeshu.Uf;
using Umeshu.Utility;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Umeshu.USystem.TextAnimation
{
    public sealed class TMPText_AnimationManager : GameSystem<TMPText_AnimationManager>
    {
        [SerializeField] private EnumBasedSelector<TMPText_Animation.AnimationType, TMPText_Animation> preBuildAnimations;
        [SerializeField] private TMPText_Animation[] customAnimations;

        private readonly Dictionary<string, TMPText_Animation> animationsDictionary = new();

        protected override void SystemFirstInitialize()
        {
            List<TMPText_Animation> _animationsToRegister = new();

            foreach (TMPText_Animation.AnimationType _animationType in UfEnum.GetEnumArray<TMPText_Animation.AnimationType>())
            {
                TMPText_Animation _animation = preBuildAnimations[_animationType];
                if (_animation == null) continue;
                _animationsToRegister.Add(_animation);
            }

            foreach (TMPText_Animation _animation in customAnimations)
            {
                if (_animation == null) continue;
                _animationsToRegister.Add(_animation);
            }

            foreach (TMPText_Animation _animation in _animationsToRegister)
            {
                if (animationsDictionary.ContainsKey(_animation.Key))
                {
                    $"TMPText_AnimationManager: Animation with name {_animation.Key} already exists.".LogError();
                    continue;
                }
                animationsDictionary.Add(_animation.Key.RemoveWhiteSpace().ToLower(), _animation);
            }
        }
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate() { }

        internal TMPText_Animation GetAnimation(string _effectName)
        {
            string _usedEffectName = _effectName.RemoveWhiteSpace().ToLower();
            if (animationsDictionary.ContainsKey(_usedEffectName)) return animationsDictionary[_usedEffectName];
            $"TMPText_AnimationManager: Animation with name {_effectName.Quote()} not found.".LogError();
            return null;
        }

#if UNITY_EDITOR


        internal void FillPrebuildAnimationSlots()
        {
            // Get the folder that contain the folder PreBuildAnimations 

            string _path = UfEditor.GetScriptableObjectAssetPath<TMPText_Animation_Scale>();
            _path = System.IO.Path.GetDirectoryName(_path);
            _path += @"\";// System.IO.Path.Combine(_path, "TMPText_Animation_.asset");
            Debug.Log(_path);

            foreach (TMPText_Animation.AnimationType _animationType in UfEnum.GetEnumArray<TMPText_Animation.AnimationType>())
            {
                TMPText_Animation _animation = _animationType switch
                {
                    TMPText_Animation.AnimationType.Custom => null,
                    TMPText_Animation.AnimationType.Rotation => UfEditor.GetOrCreateScriptableObject<TMPText_Animation_Rotation>(_path),
                    TMPText_Animation.AnimationType.Scale => UfEditor.GetOrCreateScriptableObject<TMPText_Animation_Scale>(_path),
                    TMPText_Animation.AnimationType.CharacterAppearance => UfEditor.GetOrCreateScriptableObject<TMPText_Animation_CharacterAppearance>(_path),
                    TMPText_Animation.AnimationType.Blink => UfEditor.GetOrCreateScriptableObject<TMPText_Animation_Blink>(_path),
                    TMPText_Animation.AnimationType.Wave => UfEditor.GetOrCreateScriptableObject<TMPText_Animation_Wave>(_path),
                    TMPText_Animation.AnimationType.WaveX => UfEditor.GetOrCreateScriptableObject<TMPText_Animation_WaveX>(_path),
                    TMPText_Animation.AnimationType.RotationWiggle => UfEditor.GetOrCreateScriptableObject<TMPText_Animation_RotationWiggle>(_path),
                    _ => null
                };
                preBuildAnimations.SetValueAt(_animationType, _animation);
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}