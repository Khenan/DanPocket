using Umeshu.Common;
using UnityEngine;
using UnityEngine.UI;

public class USpriteSetter : HeritableGameElement
{
    [Header("Sprite Setter")]
    [SerializeField] private UAsset<Sprite> sprite;

    protected override void GameElementFirstInitialize()
    {
        if (sprite == null) return;
        if (sprite.Value == null) return;
        if (TryGetComponent(out SpriteRenderer _spriteRenderer)) _spriteRenderer.sprite = sprite.Value;
        if (TryGetComponent(out Image _image)) _image.sprite = sprite.Value;
    }

    protected override void GameElementEnableAndReset() { }
    protected override void GameElementPlay() { }
    protected override void GameElementUpdate() { }


}
