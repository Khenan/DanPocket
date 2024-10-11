using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MatUpdater))]
public class UpdateShaderSpriteParameters : MonoBehaviour
{
    [Header("Sprite Parameters")]
    [SerializeField, Tooltip("Is followed by _Sprite, _Tiling and _Offset in the shader")] private string shaderID = "";
    [SerializeField] private bool changeTextureAsWell = false;

    private SpriteRenderer spriteRenderer;
    private MatUpdater matUpdater;

    private Sprite lastSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        matUpdater = GetComponent<MatUpdater>();
    }

    private void Update()
    {
        if (lastSprite != spriteRenderer.sprite)
        {
            lastSprite = spriteRenderer.sprite;
            matUpdater.SetShaderSprite(spriteRenderer.sprite, shaderID, changeTextureAsWell);
        }
    }
}
