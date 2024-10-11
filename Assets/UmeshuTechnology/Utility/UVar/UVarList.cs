using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using UnityEngine;
[System.Serializable]
public class UVarList<T> : IUVar, IList<T>
{
    public UVarList(List<T> _resetValues = null)
    {
        this.value = _resetValues ?? new();
        ResetVar();
    }
    [SerializeField]
    private List<T> reset;
    [SerializeField]
    private List<T> value;
    private UEvent onCollectionChanged;
    private UEvent<T> onAdd, onRemove;

    public event Action onCollectionChange
    {
        add { onCollectionChange += value; }
        remove { onCollectionChanged -= value; }
    }
    public event Action<T> onAddElement
    {
        add { onAddElement += value; }
        remove { onAddElement -= value; }
    }
    public event Action<T> onRemoveElement
    {
        add { onRemove += value; }
        remove { onRemove -= value; }
    }

    public List<T> Reset => reset;
    public List<T> Value
    {
        get => value;
        set
        {
            foreach (T _oldValues in value)
            {
                onRemove?.Invoke(_oldValues);
                value.Remove(_oldValues);
            }
            foreach (T _newValue in value)
            {
                onAdd?.Invoke(_newValue);
                value.Add(_newValue);
            }
            onCollectionChanged?.Invoke();
        }
    }
    public T this[int _index]
    {
        get => value[_index];
        set => this.value[_index] = value;
    }
    public int Count => value.Count;
    public bool IsReadOnly => false;

    public void Add(T _item)
    {
        onAdd?.Invoke(_item);
        value.Add(_item);
        onCollectionChanged?.Invoke();
    }

    public void Clear()
    {
        foreach (T _clearedItem in value) onRemove?.Invoke(_clearedItem);
        value.Clear();
        onCollectionChanged?.Invoke();
    }

    public bool Contains(T _item) => value.Contains(_item);

    public void CopyTo(T[] _array, int _arrayIndex) => value.CopyTo(_array, _arrayIndex);

    public IEnumerator<T> GetEnumerator() => value.GetEnumerator();

    public int IndexOf(T _item) => value.IndexOf(_item);

    public void Insert(int _index, T _item)
    {
        if (_index > 0 && _index < value.Count)
        {
            onAdd?.Invoke(_item);
        }
        value.Insert(_index, _item);
        onCollectionChanged?.Invoke();
    }

    public bool Remove(T _item)
    {
        if (value.Contains(_item))
        {
            onRemove?.Invoke(_item);
            value.Remove(_item);
            onCollectionChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void RemoveAt(int _index)
    {
        if (_index < 0 || _index >= value.Count) return;
        onRemove?.Invoke(this[_index]);
        value.RemoveAt(_index);
        onCollectionChanged?.Invoke();
    }

    public void ResetVar()
    {
        value = new(reset);
    }

    IEnumerator IEnumerable.GetEnumerator() => value.GetEnumerator();
}
