using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class AutoSpriteLink_Image<TEnum, TLinkData> : AutoSpriteLink<TEnum, TLinkData, Image> where TEnum : Enum where TLinkData : AutoSpriteLinkData<TEnum>
{
    protected sealed override void SetSpriteOnComponent(Image _component, Sprite _sprite) => _component.sprite = _sprite;
}
