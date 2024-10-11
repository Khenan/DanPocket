using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class LightAndShadowCaster2DExtension
{
    private static readonly BindingFlags accessFlagsPrivate = BindingFlags.NonPublic | BindingFlags.Instance;


    private static readonly FieldInfo lightMeshField = typeof(Light2D).GetField("m_Mesh", accessFlagsPrivate);

    public static List<Vector3> GetLightVertices(this Light2D _light2D)
    {
        List<Vector3> _vertices = new();
        Mesh _mesh = (Mesh)lightMeshField.GetValue(_light2D);
        if (_mesh != null)
            foreach (Vector3 _vertex in _mesh.vertices)
                _vertices.Add(_light2D.transform.TransformPoint(_vertex));
        return _vertices;
    }


#if UNITY_EDITOR
    private static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", accessFlagsPrivate);
    private static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", accessFlagsPrivate);
    private static readonly MethodInfo onEnableMethod = typeof(ShadowCaster2D).GetMethod("OnEnable", accessFlagsPrivate);

    public static void SetShapePath(this ShadowCaster2D _shadowCaster, Vector3[] _shapePath)
    {
        shapePathField.SetValue(_shadowCaster, _shapePath);
        meshField.SetValue(_shadowCaster, null);
        onEnableMethod.Invoke(_shadowCaster, new object[0]);
    }
#endif
}