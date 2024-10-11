using System;
using System.Collections.Generic;
using Umeshu.Uf;
using Umeshu.USystem.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Umeshu.USystem.PopUp
{
    using Time = UnityEngine.Time;

    [RequireComponent(typeof(CanvasGroup))]
    internal sealed class PopUp : PoolableGameElement
    {
        [Header("PopUp Links")]
        [SerializeField] private UText_TMPText titleLocalizedText;
        [SerializeField] private RectTransform popUpRectTransform;
        [SerializeField] private Image cancelHitBoxImage;
        [SerializeField] private float cancelHitBoxImageAlphaMax = 0.75f;
        [Space(5)]
        [SerializeField] private List<RectTransformAnimator> rectTransfomAnimators;

        [Header("Parameters")]
        [SerializeField] private bool makeOtherUIDiseappear = true;
        [SerializeField] private bool canCloseByClickingOutside = true;

        [Space(5)]

        [SerializeField] private float timeToFadeIn = .25f;
        [SerializeField] private float timeToClose = .25f;
        [SerializeField] private float timeBeforeCanClose = .2f;



        private CanvasGroup canvasGroup;
        private PopUpContent popUpContent;

        private float timeRemainingToClose;
        private float timeRemainingBeforeCanClose;
        private bool isClosing = false;
        private int popUpIndex;
        public event Action onCloseRequest;

        private float fadeInPercentage = 0;


        public PopUpKey PopUpType { get; private set; }

        private Func<int, bool> isTheCurrentPopUpMethod;

        public void InitializePopUp(PopUpKey _popUpType, int _popUpIndex, string _titleKey, Func<int, bool> _isTheCurrentPopUpMethod)
        {
            popUpIndex = _popUpIndex;
            titleLocalizedText.SetKey(_titleKey);
            PopUpType = _popUpType;
            isTheCurrentPopUpMethod = _isTheCurrentPopUpMethod;
        }

        public T GetPopUpContentAs<T>() where T : PopUpContent => popUpContent as T;

        protected override void GameElementFirstInitialize()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            popUpContent = GetComponentInChildren<PopUpContent>();
            popUpContent.onCloseRequest += Close;

            timeRemainingToClose = timeToClose;
            timeRemainingBeforeCanClose = timeBeforeCanClose;
        }

        protected override void GameElementEnableAndReset()
        {
            isClosing = false;
            canvasGroup.alpha = 1;
            timeRemainingToClose = timeToClose;
            timeRemainingBeforeCanClose = timeBeforeCanClose;
            fadeInPercentage = 0;
            foreach (RectTransformAnimator _rectTransformAnimator in rectTransfomAnimators) if (_rectTransformAnimator != null) _rectTransformAnimator.AppearAll();
        }

        protected override void GameElementPlay() { }

        protected override void GameElementUpdate()
        {
            foreach (RectTransformAnimator _rectTransformAnimator in rectTransfomAnimators) if (_rectTransformAnimator != null) _rectTransformAnimator.UpdateRectTransforms();

            UfMath.MoveTowardsZero(ref timeRemainingBeforeCanClose, Time.deltaTime);
            if (isClosing)
            {
                canvasGroup.interactable = false;
                UfMath.MoveTowardsZero(ref timeRemainingToClose, Time.deltaTime, Stop);
            }
            else
            {
                canvasGroup.interactable = isTheCurrentPopUpMethod.Invoke(popUpIndex);
                fadeInPercentage = Mathf.MoveTowards(fadeInPercentage, 1, Time.deltaTime / timeToFadeIn);
            }

            float _fadeOutPercentage = Mathf.Clamp01(timeRemainingToClose / timeToClose);
            float _fadePercentage = _fadeOutPercentage * fadeInPercentage;

            Color _backColor = cancelHitBoxImage.color;
            _backColor.a = _fadePercentage * cancelHitBoxImageAlphaMax;
            cancelHitBoxImage.color = _backColor;
        }

        public override void Stop()
        {
            onCloseRequest?.Invoke();
            base.Stop();
        }

        public bool IsHiderPopUp() => makeOtherUIDiseappear;

        public void PlayerClickedOutside()
        {
            if (canCloseByClickingOutside) Close();
        }

        public void Close()
        {
            if (isClosing || timeRemainingBeforeCanClose > 0) return;
            isClosing = true;
            canvasGroup.interactable = false;
            foreach (RectTransformAnimator _rectTransformAnimator in rectTransfomAnimators) _rectTransformAnimator.DisappearAll();
        }
    }
}