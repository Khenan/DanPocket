using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


public abstract class AutoSpriteLink<TEnum, TLinkData, TComponent> : HeritableGameElement, IAutoSpriteLink where TEnum : Enum where TLinkData : AutoSpriteLinkData<TEnum> where TComponent : Component
{
    [Header("Sprite Link")]
    public TEnum spritelinkType;
    public TLinkData spritelinkData;
    private TComponent spriteComponent;

    protected override void GameElementFirstInitialize() { }
    protected override void GameElementEnableAndReset() => UpdateSprite();
    protected override void GameElementPlay() { }
    protected override void GameElementUpdate() { }

    public virtual void SetSprite(TEnum _spriteLinkType)
    {
        spritelinkType = _spriteLinkType;
        UpdateSprite();
    }

    public virtual TComponent UpdateSprite()
    {
        if (spritelinkData == null)
        {
            $"{typeof(TLinkData).Name} not set".LogError();
            return null;
        }

        if (spriteComponent != null || TryGetComponent(out spriteComponent))
        {
            Sprite _sprite = spritelinkData.spriteLinks[spritelinkType];
            if (_sprite == null) $"Sprite not found for {spritelinkType} on {transform.GetHierarchyPath()}".LogOnce();
            SetSpriteOnComponent(spriteComponent, _sprite);
        }
        else $"{typeof(TComponent).Name} component not found".LogError();
        return spriteComponent;
    }

    protected abstract void SetSpriteOnComponent(TComponent _component, Sprite _sprite);

    Component IAutoSpriteLink.UpdateSprite() => UpdateSprite();
}

public interface IAutoSpriteLink
{
    Component UpdateSprite();
}
