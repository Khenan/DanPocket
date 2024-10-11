using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

using Slider = UnityEngine.UI.Slider;


namespace Umeshu.USystem.LoadingScreen
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingCanvasGroupFader : LoadingUtil
    {
        private CanvasGroup canvasGroup;
        public override void Init()
        {
            canvasGroup ??= GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }
        public override void FadeIn(float _progress) => canvasGroup.alpha = _progress;
        public override void FadeOut(float _progress) => canvasGroup.alpha = 1 - _progress;
        public override void Load(float _progress) => canvasGroup.alpha = 1;
    }
}