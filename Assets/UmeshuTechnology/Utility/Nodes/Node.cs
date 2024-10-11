using System.Collections;
using System.Collections.Generic;
using Umeshu.Uf;

namespace Umeshu.Utility
{
    public class Node<T> : INode<T>
    {
        public Node(T _value = default, INode<T> _parent = default, IEnumerable<INode<T>> _childrens = default)
        {
            this.value = _value;
            Parent = _parent;
            if (_childrens != null)
            {
                foreach (INode<T> _node in _childrens)
                {
                    Add(_node);
                }
            }
        }
        public Node(T _value, IEnumerable<INode<T>> _childrens, INode<T> _parent = default) : this(_value, _parent, _childrens) { }

        private T value;
        private INode<T> parent;
        private List<INode<T>> childrens = new();

        public T Value { get => value; set => this.value = value; }
        public INode<T> Parent { get => parent; set => INode<T>.Reparent(_child: this, _parent: value); }
        IReadonlyNode<T> IReadonlyNode<T>.Parent => parent;
        public INode<T> this[int _index] { get => childrens[_index]; set => childrens[_index] = value; }
        IReadonlyNode<T> IReadOnlyList<IReadonlyNode<T>>.this[int _index] => childrens[_index];
        public IEnumerable<INode<T>> Childrens => childrens;
        IEnumerable<IReadonlyNode<T>> IReadonlyNode<T>.Childrens => childrens;

        public int Count => childrens.Count;
        public virtual bool IsReadOnly => false;

        public string Name => value.ToNullableString();

        public virtual void Add(INode<T> _item) => INode<T>.Reparent(_child: _item, _parent: this);
        public void Clear() => childrens.ForEach(_child => Remove(_child));

        public bool Contains(INode<T> _item) => childrens.Contains(_item);

        public void CopyTo(INode<T>[] _array, int _arrayIndex) => childrens.CopyTo(_array);

        public IEnumerator<INode<T>> GetEnumerator() => childrens.GetEnumerator();

        public int IndexOf(INode<T> _item) => childrens.IndexOf(_item);

        public void Insert(int _index, INode<T> _item)
        {
            if (_item == null || !IsValidIndex(_index)) return;

            if (_item.Cast(out Node<T> _child)) _child.parent = this;
            else
            {
                Add(_item);
                childrens.Remove(_item);
            }
            childrens.Insert(_index, _item);
        }

        public bool Remove(INode<T> _item)
        {
            if (childrens.Remove(_item))
            {
                INode<T>.Reparent(_item, null);
                return true;
            }
            return false;
        }

        public void RemoveAt(int _index) => Remove(IsValidIndex(_index) ? this[_index] : null);

        IEnumerator IEnumerable.GetEnumerator() => childrens.GetEnumerator();


        private bool IsValidIndex(int _index) => _index >= 0 && _index < Count;

        void INode<T>.SetParentWithoutNotify(INode<T> _parent) => this.parent = _parent;
        void INode<T>.AddChildWithoutNotify(INode<T> _child) => childrens.Add(_child);
        void INode<T>.RemoveChildWithoutNotify(INode<T> _child) => childrens.Remove(_child);

        IEnumerator<IReadonlyNode<T>> IEnumerable<IReadonlyNode<T>>.GetEnumerator()
        {
            return Childrens.GetEnumerator();
        }

        public static implicit operator T(Node<T> _node) => _node.value;
        public static implicit operator Node<T>(T _value) => new(_value);

    }
}