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
    [RequireComponent(typeof(Slider))]
    public class LoadingSlider : LoadingUtil
    {
        private Slider slider;

        public override void Init()
        {
            slider ??= GetComponent<Slider>();
            slider.value = 0;
        }
        public override void FadeIn(float _progress) => slider.value = 0;
        public override void FadeOut(float _progress) => slider.value = 1;
        public override void Load(float _progress) => slider.value = _progress;
    }
}