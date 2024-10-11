using System;
using UnityEngine;

public abstract class AutoSpriteLink_SpriteRenderer<TEnum, TLinkData> : AutoSpriteLink<TEnum, TLinkData, SpriteRenderer> where TEnum : Enum where TLinkData : AutoSpriteLinkData<TEnum>
{
    protected sealed override void SetSpriteOnComponent(SpriteRenderer _component, Sprite _sprite) => _component.sprite = _sprite;
}
