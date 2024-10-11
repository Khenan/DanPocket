using System.Collections.Generic;
using TMPro;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.USystem.TextAnimation
{
    using Time = UnityEngine.Time;

    [RequireComponent(typeof(TMP_Text))]
    public class TMPText_Animator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool persistentLetters = false;

        private TMP_Text text;
        private string lastText;

        private readonly List<TMPText_AnimationData> animationDatas = new();

        public const char EFFECT_SEPARATOR = '/';
        public const char PARAMETER_SLOT_SEPARATOR = ':';
        public const char PARAMETER_SEPARATOR = ';';
        public const char PARAMETER_VALUE_SEPARATOR = '=';

        private TextChangeCallType lastTextChangeType;
        private int lastTextChangeCallFrame;
        public enum TextChangeCallType
        {
            LateUpdate,
            TextChanged
        }

        private void Awake() => text = GetComponent<TMP_Text>();
        private void OnEnable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChange);
            animationDatas.Clear();
        }
        private void OnDisable() => TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChange);

        private void OnTextChange(Object _obj)
        {
            if (_obj == text && lastText != text.text)
            {
                lastText = text.text;
                if (!persistentLetters) animationDatas.Clear();
                UpdateTMPText(TextChangeCallType.TextChanged);
            }
        }

        private void LateUpdate() => UpdateTMPText(TextChangeCallType.LateUpdate);

        private void UpdateTMPText(TextChangeCallType _textChangeType)
        {
            TMP_TextInfo _textInfo = text.textInfo;
            int _characterCount = _textInfo.characterCount;

            if (_characterCount == 0)
            {
                animationDatas.Clear();
                return;
            }

            if (_textInfo.linkInfo.Length == 0) return;

            text.ForceMeshUpdate();

            if (_textChangeType != lastTextChangeType && lastTextChangeCallFrame == Time.frameCount) return;
            lastTextChangeCallFrame = Time.frameCount;
            lastTextChangeType = _textChangeType;

            int _visibleCharacterIndex = -1;

            for (int _characterIndex = 0; _characterIndex < _characterCount; _characterIndex++)
            {
                static bool CharacterIndexIsInLink(int _characterIndex, TMP_LinkInfo _link) => _characterIndex >= _link.linkTextfirstCharacterIndex && _characterIndex < _link.linkTextfirstCharacterIndex + _link.linkTextLength;

                TMP_CharacterInfo _charInfo = _textInfo.characterInfo[_characterIndex];

                if (!_charInfo.isVisible) continue;

                _visibleCharacterIndex++;

                #region Skip character if not in link
                bool _isInLink = false;
                foreach (TMP_LinkInfo _link in _textInfo.linkInfo)
                    if (CharacterIndexIsInLink(_characterIndex, _link))
                    {
                        _isInLink = true;
                        break;
                    }
                if (!_isInLink) continue;
                #endregion

                #region Get Basic Data

                int _materialIndex = _charInfo.materialReferenceIndex;
                int _vertexIndex = _charInfo.vertexIndex;


                Vector3[] _destinationVertices = _textInfo.meshInfo[_materialIndex].vertices;
                Color32[] _destinationColors = _textInfo.meshInfo[_materialIndex].colors32;

                #endregion

                #region Setup animation data

                if (_visibleCharacterIndex >= animationDatas.Count) animationDatas.Add(new TMPText_AnimationData());
                TMPText_AnimationData _animationData = animationDatas[_visibleCharacterIndex];

                string _word = _charInfo.character.ToString();
                TMP_CharacterInfo _characterInfo = _textInfo.characterInfo[_characterIndex];
                foreach (TMP_WordInfo _wordInfo in _textInfo.wordInfo)
                    if (_wordInfo.textComponent != null && _characterInfo.index >= _wordInfo.firstCharacterIndex && _characterInfo.index <= _wordInfo.lastCharacterIndex)
                    {
                        _word = _wordInfo.GetWord();
                        break;
                    }

                bool _characterIsNotTheSameAsPrevious = _animationData.character != _charInfo.character;
                if (_characterIsNotTheSameAsPrevious) _animationData.timeAtCharacterCreation = Time.unscaledTime;

                _animationData.color = _destinationColors[_vertexIndex];
                _animationData.word = _word;
                _animationData.character = _charInfo.character;
                _animationData.characterIndex = _characterIndex;

                _animationData.offset = new(0, 0);
                _animationData.scale = 1;
                _animationData.rotation = 0;

                _animationData.time = Time.unscaledTime;

                #endregion

                foreach (TMP_LinkInfo _link in _textInfo.linkInfo)
                {
                    if (!CharacterIndexIsInLink(_characterIndex, _link)) continue;

                    string[] _effects = _link.GetLinkID().Split(EFFECT_SEPARATOR);
                    foreach (string _effect in _effects)
                    {
                        #region Get Parameters
                        string _effectName = _effect.Split(PARAMETER_SLOT_SEPARATOR)[0];
                        Dictionary<string, string> _effectParameters = null;
                        if (_effect.Contains(PARAMETER_SLOT_SEPARATOR))
                        {
                            _effectParameters = new Dictionary<string, string>();
                            string[] _parameters = _effect.Split(PARAMETER_SLOT_SEPARATOR)[1].Split(PARAMETER_SEPARATOR);
                            foreach (string _parameter in _parameters)
                            {
                                string[] _parameterData = _parameter.Split(PARAMETER_VALUE_SEPARATOR);
                                if (_parameterData.Length == 2 && !string.IsNullOrEmpty(_parameterData[1])) _effectParameters.Add(_parameterData[0], _parameterData[1]);
                            }
                        }
                        #endregion

                        if (TMPText_AnimationManager.Instance == null) "TMPText_AnimationManager.Instance is null".LogError();
                        else
                        {
                            TMPText_Animation _animation = TMPText_AnimationManager.Instance.GetAnimation(_effectName);
                            if (_animation == null) ($"TMPText_AnimationManager.Instance.GetAnimation({_effectName.Quote()}) is null in {_effect.Quote()}").LogError();
                            else _animation.Animate(_animationData, _effectParameters);
                        }
                    }
                }

                #region Apply changes

                Vector3 _center = (_destinationVertices[_vertexIndex] + _destinationVertices[_vertexIndex + 2]) / 2;

                for (int _vertexIteratorIndex = 0; _vertexIteratorIndex < 4; _vertexIteratorIndex++)
                {
                    Vector3 _vertex = _destinationVertices[_vertexIndex + _vertexIteratorIndex];
                    _vertex -= _center;
                    _vertex = Quaternion.Euler(0, 0, _animationData.rotation) * _vertex; // rotation
                    _vertex *= _animationData.scale; // scale
                    _vertex += _center;
                    _vertex += _animationData.offset.ToVector3(); // decal
                    _destinationVertices[_vertexIndex + _vertexIteratorIndex] = _vertex;
                    _destinationColors[_vertexIndex + _vertexIteratorIndex] = _animationData.color; // color
                }

                #endregion
            }

            while (animationDatas.Count > _visibleCharacterIndex + 1) animationDatas.RemoveAt(animationDatas.Count - 1);

            text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }

}

