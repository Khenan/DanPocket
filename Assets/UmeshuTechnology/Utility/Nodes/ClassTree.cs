using System;
using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;

namespace Umeshu.Utility
{
    public class ClassTree
    {
        private ClassTree(Node<Type> _typeTreeRoot) => this.typeTreeRoot = _typeTreeRoot;
        private Node<Type> typeTreeRoot;
        public IReadonlyNode<Type> Nodes => typeTreeRoot;


        public static ClassTree Create(Type _baseType)
        {
            List<Type> _possibleTypes = new(_baseType.GetSubTypes());
            Node<Type> _root = new(_baseType);
            FillNodeRecursive(_root, _possibleTypes);
            return new ClassTree(_root);
        }

        private static void FillNodeRecursive(Node<Type> _parent, List<Type> _possibleTypes)
        {
            List<Type> _l = _possibleTypes.Where(_t => _t?.BaseType == _parent?.Value || IsFirstChildOfInterface(_t, _parent?.Value)).ToList();
            foreach (Type _type in _l)
            {
                Node<Type> _newNode = new(_type);
                _parent.Add(_newNode);
                _possibleTypes.Remove(_type);
                FillNodeRecursive(_newNode, _possibleTypes);
            }
        }
        private static bool IsFirstChildOfInterface(Type _checkedType, Type _parentType)
        {
            if (!_parentType.IsInterface)
            {
                return false;
            }
            if (!_checkedType.ImplementInterface(_parentType))
            {
                return false;
            }
            if (_checkedType.BaseType == null)
            {
                return true;
            }
            while (_checkedType.BaseType != null)
            {
                _checkedType = _checkedType.BaseType;
                if (_checkedType.ImplementInterface(_parentType))
                {
                    return false;
                }
            }
            return true;
        }
    }
}