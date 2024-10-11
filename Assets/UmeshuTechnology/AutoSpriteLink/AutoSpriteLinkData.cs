using System;
using Umeshu.Utility;
using UnityEngine;

public class AutoSpriteLinkData<TEnum> : ScriptableObject where TEnum : Enum
{
    public EnumBasedSelector<TEnum, Sprite> spriteLinks = new();
}