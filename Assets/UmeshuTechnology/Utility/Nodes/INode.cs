using System.Collections.Generic;

namespace Umeshu.Utility
{
    public interface INode<T> : IReadonlyNode<T>, IList<INode<T>>
    {
        public new INode<T> Parent { get; set; }
        public new T Value { get; set; }
        public new IEnumerable<INode<T>> Childrens { get; }

        protected void AddChildWithoutNotify(INode<T> _child);
        protected void RemoveChildWithoutNotify(INode<T> _child);
        protected void SetParentWithoutNotify(INode<T> _parent);
        public static void Reparent(INode<T> _child, INode<T> _parent = default)
        {
            if (_child == null)
            {
                return;
            }

            _child?.Parent?.RemoveChildWithoutNotify(_child);
            _child?.SetParentWithoutNotify(_parent);
            _parent?.AddChildWithoutNotify(_child);
        }
        public static INode<T> GetRoot(INode<T> _node) => (INode<T>)IReadonlyNode<T>.GetRoot(_node);
    }
}
