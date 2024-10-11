using System.Collections.Generic;
using Umeshu.Uf;
using UnityEngine;
using UnityEngine.UI;

public class MatUpdater : MonoBehaviour
{
    private MaterialPropertyBlock matProp;
    private Renderer render;


    private bool CheckMatPropAndRender()
    {
        matProp ??= new MaterialPropertyBlock();
        render ??= GetComponent<Renderer>() ?? GetComponent<ParticleSystem>()?.GetComponent<Renderer>(); // Check si renderer, sinon check si c'est un fx et prend son render
        if (render != null) render?.GetPropertyBlock(matProp);
        return render != null;
    }

    private void SetMatProp()
    {
        if (render != null) render?.SetPropertyBlock(matProp);
    }

    public void SetColor(string _id, Color _value)
    {
        if (!CheckMatPropAndRender()) return;
        matProp.SetColor(_id, _value);
        SetMatProp();
    }

    public void SetBool(string _id, bool _value) => SetFloat(_id, _value ? 1 : 0);

    public void SetFloat(string _id, float _value)
    {
        if (!CheckMatPropAndRender()) return;
        matProp.SetFloat(_id, _value);
        SetMatProp();
    }

    public void SetTexture(string _id, Texture2D _value)
    {
        if (!CheckMatPropAndRender()) return;
        matProp.SetTexture(_id, _value);
        SetMatProp();
    }

    public void SetVector(string _id, Vector4 _value)
    {
        if (!CheckMatPropAndRender()) return;
        matProp.SetVector(_id, _value);
        SetMatProp();
    }

    private static readonly Dictionary<Transform, MatUpdater> matUpdaters = new();
    public static MatUpdater GetOrAddMatUpdater(Transform _transform)
    {
        if (matUpdaters.ContainsKey(_transform)) return matUpdaters[_transform];
        MatUpdater _matUpdater = _transform.GetOrAddComponent<MatUpdater>();
        matUpdaters.Add(_transform, _matUpdater);
        return _matUpdater;
    }

    public Color GetColor(string _id) => !CheckMatPropAndRender() ? default : matProp.GetColor(_id);
    public float GetFloat(string _id) => !CheckMatPropAndRender() ? default : matProp.GetFloat(_id);
    public Texture GetTexture(string _id) => !CheckMatPropAndRender() ? default : matProp.GetTexture(_id);
    public Vector4 GetVector(string _id) => !CheckMatPropAndRender() ? default : matProp.GetVector(_id);


    public void SetShaderSprite(Sprite _sprite, string _refId, bool _changeTexture = false)
    {
        _sprite.GetTilingAndOffsetFromSprite(out Vector2 _tiling, out Vector2 _offset);
        if (_changeTexture) SetTexture(_refId + "_Sprite", _sprite.texture);
        SetVector(_refId + "_Tiling", _tiling);
        SetVector(_refId + "_Offset", _offset);
    }
}
