using UnityEngine;

/// <summary>
/// Class that handles the instantiation of a prefab.
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class InstanceManager<T> where T : MonoBehaviour
{
    [SerializeField]
    private T prefab;
    private T instance;
    /// <summary>
    /// Get the current instance (can be null)
    /// </summary>
    public T Instance => instance;
    public bool HasInstance => instance;

    /// <summary>
    /// Get the current instance (Instantiates one when there is none) 
    /// </summary>
    public T GetInstance(Vector3? _position = null, Vector3? _scale = null, Quaternion? _rotation = null, Transform _parent = null) =>
        HasInstance ? Initialize(instance, _position, _scale, _rotation, _parent) : (instance = Initialize(GameObject.Instantiate(prefab), _position, _scale, _rotation, _parent));

    private T Initialize(T _instance, Vector3? _position, Vector3? _scale, Quaternion? _rotation, Transform _parent)
    {
        Transform _instanceTransform = _instance.transform;
        if (_position.HasValue) _instanceTransform.position = _position.Value;
        if (_scale.HasValue) _instanceTransform.localScale = _scale.Value;
        if (_rotation.HasValue) _instanceTransform.rotation = _rotation.Value;
        if (_parent) _instanceTransform.SetParent(_parent);
        return _instance;
    }
}
