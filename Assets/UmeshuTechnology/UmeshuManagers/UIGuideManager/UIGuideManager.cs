using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.USystem.GameData;
using Umeshu.Utility;
using UnityEditor;
using UnityEngine;
using static Umeshu.Utility.CameraScroll;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Umeshu.USystem.UIGuide
{
    public sealed class UIGuideManager : SceneSystem<UIGuideManager>
    {

        [Header("Scene Links")]
        [SerializeField] private UPoolableAsset<UIGuide> guideRef;
        [SerializeField] private UIGuideVisualData guideVisualData;

        private readonly List<UIGuideLink> uiGuideLinks = new();
        private RectTransform rectTransform;
        private CameraScroll cameraScroll;

        protected override void SystemFirstInitialize() => rectTransform = GetComponent<RectTransform>();
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate() { }
        protected override void UnitySystemUpdate()
        {
            base.UnitySystemUpdate();
            if (cameraScroll == null) return;
            foreach (UIGuideLink _uiGuideLink in uiGuideLinks)
                UpdateGuideToAimedObject(rectTransform, _uiGuideLink, cameraScroll, guideVisualData);
        }

        #region Add/Remove UI Guide

        public static UIGuide AddUIGuide(Transform _aimedObject) => Instance?.AddUIGuideLocally(_aimedObject);
        public static void RemoveUIGuide(UIGuide _guide) => Instance?.RemoveUIGuideLocally(_guide);

        private UIGuide AddUIGuideLocally(Transform _aimedObject)
        {
            UIGuide _guide = this.GetChildFromPool(true, guideRef);
            _guide.transform.SetParent(rectTransform);
            _guide.onClickGoTo += UIGuideGotClicked;
            uiGuideLinks.Add(new UIGuideLink { uiGuide = _guide, aimedObject = _aimedObject });
            cameraScroll = GameDataManager.GetData<CameraScrollRuntimeData>().currentCameraScroll;
            return _guide;
        }

        private void RemoveUIGuideLocally(UIGuide _guide)
        {
            UIGuideLink _uiGuideLink = uiGuideLinks.Find(_link => _link.uiGuide == _guide);
            if (_uiGuideLink == null) return;
            _uiGuideLink.uiGuide.onClickGoTo -= UIGuideGotClicked;
            _uiGuideLink.uiGuide.Stop();
            uiGuideLinks.Remove(_uiGuideLink);
        }

        protected override void OnDestroy()
        {
            foreach (UIGuideLink _uiGuideLink in uiGuideLinks.ToArray())
                RemoveUIGuide(_uiGuideLink.uiGuide);
            base.OnDestroy();
        }

        #endregion

        public static void UpdateGuideToAimedObject(RectTransform _containerRect, UIGuideLink _guideLink, CameraScroll _cameraScroll, UIGuideVisualData _guideVisualData)
        {
            UIGuide _guide = _guideLink.uiGuide;
            Vector2 _worldPosition = _guideLink.aimedObject.position;
            Vector2 _aimedPixelPosition = _cameraScroll.Camera.WorldToScreenPoint(_worldPosition);
            UIGuideRuntimeData _data = GetGuideDataFromRectAndAim(_containerRect, _guide.RectTransform, _aimedPixelPosition, _cameraScroll.GetCameraScrollType());
            UpdateGuideToData(_guide, _data, _guideVisualData);
        }

        private void UIGuideGotClicked(UIGuide _guide)
        {
            UIGuideLink _guideLink = uiGuideLinks.Find(_link => _link.uiGuide == _guide);
            if (_guideLink == null)
            {
                "UIGuideManager: UIGuideGotClicked: _guideLink is null".LogError();
                return;
            }
            cameraScroll.GoTo(_guideLink.aimedObject.position);
        }

        #region Utility Methods

        public static void UpdateGuideToData(UIGuide _guide, UIGuideRuntimeData _data, UIGuideVisualData _guideVisualData)
        {
            _guide.transform.position = _data.finalPosition;
            UIGuideDataState _guideDataState;
            if (_data.distanceRatio < 1)
            {
                float _minDistancePercentage = Mathf.InverseLerp(_guideVisualData.minDistanceRatio, 1f, _data.distanceRatio);
                _guideDataState = UIGuideDataState.Lerp(_guideVisualData.minDistanceState, _guideVisualData.mediumDistanceState, _minDistancePercentage);
            }
            else
            {
                float _maxDistancePercentage = Mathf.InverseLerp(1f, _guideVisualData.maxDistanceRatio, _data.distanceRatio);
                _guideDataState = UIGuideDataState.Lerp(_guideVisualData.mediumDistanceState, _guideVisualData.maxDistanceState, _maxDistancePercentage);
            }
            bool _guideIsActive = _data.distanceRatio >= 1;
            _guide.CanvasGroup.interactable = _guideIsActive;
            _guide.CanvasGroup.blocksRaycasts = _guideIsActive;
            _guide.CanvasGroup.alpha = _guideDataState.alpha;
            _guide.RectTransform.localScale = Vector3.one * _guideDataState.size;
            _guide.RectTransform.up = _data.upVector;
        }

        public static UIGuideRuntimeData GetGuideDataFromRectAndAim(RectTransform _containerRect, RectTransform _guideRect, Vector2 _aimedPixelPosition, CameraScrollType _cameraScrollType)
        {
            Vector2 _containerRectSize = _containerRect.rect.size * _containerRect.lossyScale;
            Vector2 _guideRectSize = _guideRect.rect.size * _guideRect.lossyScale;

            Vector2 _efficientRect = _containerRectSize - _guideRectSize;
            Vector2 _intersectionPoint = UfMath.LineIntersectionOnRect(_efficientRect, _containerRect.position, _aimedPixelPosition);

            Vector2 _containerPos = _containerRect.position;
            Vector2 _aimedPosFromRect = _aimedPixelPosition - _containerPos;
            Vector2 _intersecPosFromRect = _intersectionPoint - _containerPos;
            Vector2 _finalPosition = _aimedPosFromRect.sqrMagnitude < _intersecPosFromRect.sqrMagnitude ? _aimedPixelPosition : _intersectionPoint;

            float _visibleSize;
            float _aimedDistance;
            if (_cameraScrollType == CameraScrollType.Horizontal)
            {
                _visibleSize = _efficientRect.x / 2f;
                _aimedDistance = Mathf.Abs(_aimedPixelPosition.x - _containerRect.position.x);
            }
            else
            {
                _visibleSize = _efficientRect.y / 2f;
                _aimedDistance = Mathf.Abs(_aimedPixelPosition.y - _containerRect.position.y);
            }
            float _distanceRatio = Mathf.Abs(_aimedDistance / _visibleSize);
            Vector2 _upVector = _aimedPixelPosition - _containerRect.position.ToVector2();

            return new UIGuideRuntimeData
            {
                finalPosition = _finalPosition,
                upVector = _upVector,
                distanceRatio = _distanceRatio
            };

            #endregion
        }

        #region Utility Classes

        public class UIGuideLink
        {
            public UIGuide uiGuide;
            public Transform aimedObject;
        }

        [Serializable]
        public class UIGuideVisualData
        {
            [Range(1.01f, 10f)] public float maxDistanceRatio = 5;
            [Range(0.01f, .9f)] public float minDistanceRatio = .9f;

            public UIGuideDataState minDistanceState;
            public UIGuideDataState mediumDistanceState;
            public UIGuideDataState maxDistanceState;
        }

        [Serializable]
        public struct UIGuideDataState
        {
            public float alpha;
            public float size;

            public static UIGuideDataState Lerp(UIGuideDataState _a, UIGuideDataState _b, float _percentage)
            {
                return new UIGuideDataState
                {
                    alpha = Mathf.Lerp(_a.alpha, _b.alpha, _percentage),
                    size = Mathf.Lerp(_a.size, _b.size, _percentage)
                };
            }
        }

        public struct UIGuideRuntimeData
        {
            public Vector2 finalPosition;
            public Vector2 upVector;
            public float distanceRatio;
        }

        #endregion
    }
}