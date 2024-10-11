using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Umeshu.Uf
{
    public static class UfText
    {
        public static string Quote(this string _original) => "\"" + _original + "\"";
        public static string Bold(this string _original) => "<b>" + _original + "</b>";
        public static string Color(this string _original, Color _col) => string.Format("<color=#{0}>{1}</color>", _col.ToHexString(), _original);
        public static string ColoredInt(this int _original) => _original.ToString().Color(_original.IntToColor());
        public static string LogColoredInt(this int _original) => _original.ToString().Color(_original.IntToLogColor());
        public static string Italic(this string _original) => "<i>" + _original + "</i>";
        public static string Size(this string _original, int _size) => string.Format("<size={0}>{1}</size>", _size, _original);
        public static string Quote(this object _original) => _original.ToSafeString().Quote();
        public static string Bold(this object _original) => _original.ToSafeString().Bold();
        public static string Color(this object _original, Color _col) => _original.ToSafeString().Color(_col);
        public static string RandomColor(this object _original) => _original.ToSafeString().Color(UfColor.RandomLogColor());
        public static string Italic(this object _original) => _original.ToSafeString().Italic();
        public static string Size(this object _original, int _size) => _original.ToSafeString().Size(_size);

        public static string RemoveWhiteSpace(this string _original) => string.Concat(_original.Where(_c => !char.IsWhiteSpace(_c) && _c != '\n'));
        public static string RemoveRecurentCharacters(this string _original) => string.Concat(_original.Distinct());

        public static string PlaceCharEveryXCharCount(string _originalText, char _character, int _x)
        {
            int _nbCharSinceLastSpace = 0;
            string _newText = "";
            for (int _i = _originalText.Length - 1; _i >= 0; _i--)
            {
                char _saveChar = _originalText[_i];
                _newText = _saveChar + _newText;
                _nbCharSinceLastSpace++;
                if (_nbCharSinceLastSpace == _x)
                {
                    _newText = _character + _newText;
                    _nbCharSinceLastSpace = 0;
                }
            }
            return _newText;
        }
        public static Dictionary<string, Dictionary<string, string>> TreatTSV(TextAsset _textAsset, string _lineFilter = "", string _linePreventer = "", bool _removeWhiteSpaceOnKeys = false)
        {
            return _textAsset == null ? default : TreatTSV(_textAsset.text, _lineFilter, _linePreventer, _removeWhiteSpaceOnKeys);
        }

        public static Dictionary<string, Dictionary<string, string>> TreatTSV(string _tsvData, string _lineFilter = "", string _linePreventer = "", bool _removeWhiteSpaceOnKeys = false)
        {
            Dictionary<string, Dictionary<string, string>> _dic = new();
            string[] _lines = _tsvData.Split('\n');

            for (int _i = 0; _i < _lines.Length; _i++)
            {
                if (_i == 0) continue;

                string _line = _lines[_i];
                string[] _categories = _lines[0].Split("\t");
                string[] _datas = _line.Split("\t");
                string _key = _datas[0];
                if (_removeWhiteSpaceOnKeys) _key = UfText.RemoveWhiteSpace(_key);

                if (!_key.Contains(_lineFilter)) continue;
                if (_key.Contains(_linePreventer)) continue;

                if (string.IsNullOrEmpty(_key)) continue;
                _dic.Add(_key, new Dictionary<string, string>());

                for (int _j = 1; _j < _datas.Length; _j++)
                {
                    string _category = _categories[_j];
                    string _data = _datas[_j];

                    if (_removeWhiteSpaceOnKeys) _category = UfText.RemoveWhiteSpace(_category);
                    if (string.IsNullOrEmpty(_category)) continue;
                    _dic[_key].Add(_category, _data);
                }
            }
            return _dic;
        }

        public static Dictionary<string, List<List<string>>> TreatOneKeyMultipleLineTSV(string _tsvData)
        {
            Dictionary<string, List<List<string>>> _dic = new();

            string[] _lines = _tsvData.Split('\n');
            string _lastKey = "";
            foreach (string _line in _lines)
            {
                string[] _datas = _line.Split("\t");
                string _key = _datas[0];
                bool _keyIsEmpty = _key == "";
                if (!_keyIsEmpty)
                {
                    _dic.Add(_key, new());
                    _lastKey = _key;
                }
                if (_lastKey == null)
                {
                    continue;
                }

                List<List<string>> _list = _dic[_lastKey];
                List<string> _actualData = new();
                for (int _i = 0; _i < _datas.Length; _i++)
                {
                    if (!_keyIsEmpty && _i == 0)
                    {
                        continue;
                    }

                    string _data = _datas[_i];
                    _actualData.Add(_data);
                }
                _list.Add(_actualData);
            }
            return _dic;
        }

        public static string ThousandSeparatedDisplay(this int _number, char _character = ' ') => _number.ToString("n0").Replace(',', _character);

        public static string ExtractBaliseText(this string _text, char _baliseStart, char _baliseEnd)
        {
            string _returned = "";
            bool _record = false;
            for (int _charIndex = 0; _charIndex < _text.Length; _charIndex++)
            {
                char _char = _text[_charIndex];
                if (_char == _baliseEnd) break;
                if (_record) _returned += _char;
                if (_char == _baliseStart) _record = true;
            }
            return _returned;
        }

        public static string ReplaceBalisesInText(string _text, char _baliseStart, char _baliseEnd, Func<string, string> _baliseReplacementMethod) => ReplaceBalisesInText(_text, _baliseStart.ToString(), _baliseEnd.ToString(), _baliseReplacementMethod);
        public static string ReplaceBalisesInText(string _text, string _baliseStart, string _baliseEnd, Func<string, string> _baliseReplacementMethod)
        {
            if (UfLogger.LogErrorIfTrue(_baliseReplacementMethod == null, "No balise replacement method")) return _text;
            while (_text.Contains(_baliseStart))
            {
                int _start = _text.IndexOf(_baliseStart);
                int _end = _text.IndexOf(_baliseEnd, _start + 1);
                string _baliseKey = _text.Substring(_start + 1, _end - _start - 1);
                _text = _text.Replace(_baliseStart + _baliseKey + _baliseEnd, _baliseReplacementMethod.Invoke(_baliseKey));
            }
            return _text;
        }

        public static string Reversed(this string _text)
        {
            if (_text == null) return null;
            char[] _array = _text.ToCharArray();
            Array.Reverse(_array);
            return new(_array);
        }
    }
}
