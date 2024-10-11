using System;
using System.Linq;
using Umeshu.Uf;
using Umeshu.USystem.GameData;
using Umeshu.USystem.TSV;
using Umeshu.Utility;
using UnityEngine;

namespace Umeshu.USystem.Text
{
    public sealed class LocalizationManager : GameSystem<LocalizationManager>
    {
        private static string CurrLanguage
        {
            get => GameData_Language?.language ?? null;
            set
            {
                if (GameData_Language == null) return;
                if (UfEnum.TryGetEnumFromString<Language>(value, out _) && GameData_Language.language != value)
                {
                    GameData_Language.language = value;
                    onLanguageChange?.Invoke();
                }
            }
        }
        public enum Language { fr, en, de, it, es, ptbr, ja, sch, ko, tr, ar, nl, hi, pl, ru }
        private static GameData_Language gameData_Language;
        private static GameData_Language GameData_Language => gameData_Language ??= GameDataManager.GetData<GameData_Language>();

        [SerializeField] private LocalizationData localizationData;

        public static Action onLanguageChange;

        private static Language GetDefaultLanguage() => UnityEngine.Application.systemLanguage switch
        {
            SystemLanguage.Chinese => Language.sch,
            SystemLanguage.English => Language.en,
            SystemLanguage.French => Language.fr,
            SystemLanguage.German => Language.de,
            SystemLanguage.Italian => Language.it,
            SystemLanguage.Japanese => Language.ja,
            SystemLanguage.Portuguese => Language.ptbr,
            SystemLanguage.Spanish => Language.es,
            SystemLanguage.ChineseSimplified => Language.sch,
            SystemLanguage.ChineseTraditional => Language.sch,
            SystemLanguage.Korean => Language.ko,
            SystemLanguage.Turkish => Language.tr,
            SystemLanguage.Arabic => Language.ar,
            SystemLanguage.Dutch => Language.nl,
            SystemLanguage.Hindi => Language.hi,
            SystemLanguage.Polish => Language.pl,
            SystemLanguage.Russian => Language.ru,
            _ => Language.en,
        };

        public static Language GetLanguage() => CurrLanguage.TryGetEnumFromString(out Language _language) ? _language : GetDefaultLanguage();
        public static void SetLanguage(Language _language) => CurrLanguage = _language.ToString();

        protected override void SystemFirstInitialize()
        {
            Language _defaultLanguage = GetDefaultLanguage();

            string _savedLanguage = CurrLanguage;
            if (!string.IsNullOrEmpty(_savedLanguage) && _savedLanguage.TryGetEnumFromString(out Language _savedLanguageEnum)) _defaultLanguage = _savedLanguageEnum;

            CurrLanguage = _defaultLanguage.ToString();

            localizationData.UpdateMaterialsInStyle();
        }

        public static string GetValue(string _key) => GetValueAtLanguage(_key, CurrLanguage);
        public static string GetValue_WithStyle(string _key, string _instanceKey) => GetValueAtLanguage_WithStyle(_key, CurrLanguage, _instanceKey);
        public static string GetValueAtLanguage(string _key, Language _language) => GetValueAtLanguage(_key, _language.ToString());

        public static string GetValueAtLanguage(string _key, string _language) => LocalizationDataIsAccessible() ? Instance.localizationData.GetValueAtLanguage(_key, _language) : "Error from localization manager";
        public static string GetValueAtLanguage_WithStyle(string _key, string _language, string _instanceKey) => LocalizationDataIsAccessible() ? Instance.localizationData.GetValueAtLanguage_WithStyle(_key, _language, _instanceKey) : "Error from localization manager";
        public static LocalizationData_FontStyles.TextStyle GetTextStyleFromInstanceKey(string _instanceKey) => LocalizationDataIsAccessible() ? Instance.localizationData.GetTextStyleFromInstanceKey(_instanceKey) : null;
        public static string GetText_WithStyle(string _text, string _instanceKey) => LocalizationDataIsAccessible() ? Instance.localizationData.ApplyStyleToTextFromInstanceKey(_text, _instanceKey) : "Error from localization manager";

        private static bool LocalizationDataIsAccessible() => !UfLogger.LogErrorIfTrue(Instance == null, "Instance is null") && !UfLogger.LogErrorIfTrue(Instance.localizationData == null, $"{nameof(localizationData)} is null");

        #region Useless
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate() { }
        #endregion
    }

}