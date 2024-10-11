using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Umeshu.USystem.UIGuide
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class UIGuide : PoolableGameElement
    {
        public RectTransform RectTransform { get; private set; }
        public CanvasGroup CanvasGroup { get; private set; }

        public Action<UIGuide> onClickGoTo;

        protected override void GameElementFirstInitialize()
        {
            RectTransform = GetComponent<RectTransform>();
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void GameElementEnableAndReset() { onClickGoTo = null; }
        protected override void GameElementPlay() { }
        protected override void GameElementUpdate() { }

        public void GotClicked() => onClickGoTo?.Invoke(this);

    }
}