using System.Collections.Generic;

namespace Umeshu.Utility
{
    public interface IReadonlyNode<T> : IReadOnlyList<IReadonlyNode<T>>
    {
        public IReadonlyNode<T> Parent { get; }
        public T Value { get; }
        public string Name { get; }
        public IEnumerable<IReadonlyNode<T>> Childrens { get; }

        public static IReadonlyNode<T> GetRoot(IReadonlyNode<T> _node)
        {
            while (_node?.Parent != null)
            {
                _node = _node.Parent;
            }
            return _node;
        }
    }
}
