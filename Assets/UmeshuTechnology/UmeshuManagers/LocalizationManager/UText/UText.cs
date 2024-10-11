using System;
using Umeshu.Uf;
using Umeshu.Utility;
using UnityEngine;

namespace Umeshu.USystem.Text
{
    public interface IUTextKeyGetter { string Key { get; } }
    public interface IUText
    {
#if UNITY_EDITOR
        void SetTextFromEditor(string _value, LocalizationData_FontStyles.TextStyle _textStyle);
#endif
        public void SetKey(string _newKey);
        public void SetEmptyKey();
    }
    public abstract class UText<T> : MonoBehaviour, IUText, IUTextKeyGetter where T : Component
    {
        protected T textComponent;
        public string Key => key;

        [SerializeField, HideInInspector] protected PickableString<LocalizationData_InstanceStyleLinks> instance;
        [SerializeField, HideInInspector] protected PickableString<LocalizationData_Translations> key;
        [SerializeField, HideInInspector] private OptionalVar<string> overrideText = new(_initialValue: "", _initialEnabled: false);
        private string[] baliseValues;

        private const string EMPTY_KEY = "Empty";
        private const string CUSTOM_BALISE_START = "{";
        private const string CUSTOM_BALISE_END = "}";
        private const string KEY_BALISE_START = "[";
        private const string KEY_BALISE_END = "]";
        private const string ERROR_BALISE = "#";
        private const string ERROR_TEXT = "ERR:";
        private Func<string, string> textMethodRemormating;

        private void OnEnable()
        {
            GetTextComponent();
            UpdateText();
            LocalizationManager.onLanguageChange += UpdateText;
        }

        private void OnDisable() => LocalizationManager.onLanguageChange -= UpdateText;

        public T GetTextComponent() => textComponent ??= GetComponent<T>();

        public void SetEmptyKey() => SetKey(EMPTY_KEY);

        public void SetKey(string _newKey)
        {
            key.value = _newKey;
            UpdateText();
        }

        public void OverrideText(string _text)
        {
            overrideText.Value = _text;
            overrideText.SetEnabled(_text != null);
            UpdateText();
        }

        public void RemoveOverrideText() => OverrideText(null);

#if UNITY_EDITOR
        public void SetTextFromEditor(string _value, LocalizationData_FontStyles.TextStyle _textStyle)
        {
            GetTextComponent();
            SetText(_value, _textStyle);
        }
#endif

        protected abstract void SetText(string _value, LocalizationData_FontStyles.TextStyle _textStyle);

        public virtual void UpdateText()
        {
            if (textComponent == null) return;
            SetText(GetText(), LocalizationManager.GetTextStyleFromInstanceKey(instance));
        }

        public void SetTextMethodRemormating(Func<string, string> _method) => textMethodRemormating = _method;
        public void SetBaliseValues(params object[] _baliseValues) => baliseValues = _baliseValues.ExtractArray(_baliseObject => _baliseObject.ToNullableString());

        private string GetText()
        {
            if (key == EMPTY_KEY) return "";

            string _text = overrideText.Enabled && overrideText.Value != null ? LocalizationManager.GetText_WithStyle(overrideText, instance) : LocalizationManager.GetValue_WithStyle(key, instance);

            int _baliseIndex = 0;
            string ReplaceBalise(string _input)
            {
                bool _errorCondition = baliseValues == null || _baliseIndex >= baliseValues.Length;
#if UNITY_EDITOR
                if (UfLogger.LogWarningIfTrue(_errorCondition, "No balise replacement on " + transform.GetHierarchyPath(), _context: this))
#else
                if(_errorCondition)
#endif
                    return (ERROR_BALISE + ERROR_TEXT + _input.Quote() + ERROR_BALISE).Bold().Color(Color.red);


                string _value = baliseValues[_baliseIndex];
                _baliseIndex++;
                return _value;
            }

            _text = UfText.ReplaceBalisesInText(_text, KEY_BALISE_START, KEY_BALISE_END, LocalizationManager.GetValue);
            _text = UfText.ReplaceBalisesInText(_text, CUSTOM_BALISE_START, CUSTOM_BALISE_END, ReplaceBalise);

            if (textMethodRemormating != null)
                _text = textMethodRemormating(_text);

            return _text;
        }
    }

}

