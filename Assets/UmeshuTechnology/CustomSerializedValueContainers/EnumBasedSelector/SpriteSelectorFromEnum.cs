using System;
using UnityEngine;

namespace Umeshu.Utility
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class SpriteSelectorFromEnum<T> : VarSelectorFromEnum<T, Sprite> where T : Enum
    {
        SpriteRenderer spriteRenderer;

        protected override void DoSelection()
        {
            if (spriteRenderer != null || TryGetComponent(out spriteRenderer)) spriteRenderer.sprite = values.GetValue(Key);
        }
    }
}

