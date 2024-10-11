using Umeshu.Uf;
using UnityEngine;

[System.Serializable]
public class PickableString<T> where T : Object, IPickableStringDatabase
{
    public string value;

    public static implicit operator string(PickableString<T> _pickableString) => _pickableString.value;
}

public interface IPickableStringDatabase { string[] GetCollection(); }

#if UNITY_EDITOR
// Dummy class to satisfy the generic constraints of PickableString<T>
class DummyPickableStringDatabase : Object, IPickableStringDatabase { public string[] GetCollection() => null; }
#endif