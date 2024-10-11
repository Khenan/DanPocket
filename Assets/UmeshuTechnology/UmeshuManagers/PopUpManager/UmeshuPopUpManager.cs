using System;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.USystem.GameData;
using Umeshu.Utility;
using UnityEngine;

namespace Umeshu.USystem.PopUp
{
    public class PopUpKey : EnumBasedKey { }

    public class PopUpRuntimeData
    {
        public int nbOfPopup = 0;
        public bool uiShouldBeHidden = false;
    }

    public abstract class UmeshuPopUpManager<TClass, TEnum> : EnumBasedGameSystem<TClass, TEnum, PopUpKey> where TClass : GameSystem<TClass> where TEnum : Enum
    {
        private readonly List<PopUp> currentPopUps = new();

        [SerializeField] private Transform popUpParent;
        [SerializeField] private EnumBasedSelector<TEnum, UPoolableAsset<PopUp>> popUps;

        protected PopUpRuntimeData popUpRuntimeData;

        protected override void SystemFirstInitialize() => popUpRuntimeData = GameDataManager.GetData<PopUpRuntimeData>();
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate() { }
        private void UpdateRunTimeData()
        {
            popUpRuntimeData.uiShouldBeHidden = currentPopUps.Exists(_popUp => _popUp.IsHiderPopUp());
            popUpRuntimeData.nbOfPopup = currentPopUps.Count;
        }

        public delegate void PopUpAction(TEnum _popUpType);
        public static event PopUpAction onPopUpPop;
        public static event PopUpAction onPopUpDepop;

        public bool CanAddPopUp(TEnum _popUpType) => popUps.GetValue(_popUpType).AssetExists();

        public TContent AddPopUp<TContent>(TEnum _popUpType, string _titleKey) where TContent : PopUpContent
        {
            PopUp _popUp = this.GetChildFromPool(true, popUps.GetValue(_popUpType));
            if (_popUp == null)
            {
                Debug.LogError($"No PopUp of type {_popUpType} found in the pool.");
                return null;
            }

            _popUp.transform.SetParent(popUpParent);
            _popUp.transform.localScale = Vector3.one;
            _popUp.onCloseRequest += RemoveCurrentPopUp;

            RectTransform _rectTransform = _popUp.GetComponent<RectTransform>();
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;

            currentPopUps.Add(_popUp);

            UpdateRunTimeData();

            _popUp.InitializePopUp(this[_popUpType], popUpRuntimeData.nbOfPopup, _titleKey, IsTheCurrentPopUp);

            onPopUpPop?.Invoke(_popUpType);

            return _popUp.GetPopUpContentAs<TContent>();
        }

        private bool IsTheCurrentPopUp(int _popUpIndex) => _popUpIndex == popUpRuntimeData.nbOfPopup;

        private void RemoveCurrentPopUp()
        {
            PopUp _popUp = currentPopUps[^1];
            _popUp.onCloseRequest -= RemoveCurrentPopUp;
            currentPopUps.Remove(_popUp);

            UpdateRunTimeData();

            onPopUpDepop?.Invoke(this[_popUp.PopUpType]);
        }

        public override void OnEnterGameMode(GameModeKey _gameMode)
        {
            base.OnEnterGameMode(_gameMode);

            foreach (PopUp _popUp in currentPopUps)
                _popUp.Close();
        }
    }
}