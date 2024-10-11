#if UNITY_EDITOR
using System;
using Umeshu.Uf;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Umeshu.USystem.ComponentAutoAdd
{
    public abstract class ComponentAutoAdd<T> where T : ComponentAutoAdd<T>, new()
    {
        private static T componentLink;
        public static T ComponentLink => componentLink ??= new T();

        protected abstract Type[] GetComponentsToAdd(Component _obj);
        protected virtual ComponentPlacementType ComponentPlacement => ComponentPlacementType.Default;

        protected static void SuscribeToComponentAddedAction()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            ObjectFactory.componentWasAdded += HandleComponentAdded;

            EditorApplication.quitting -= OnEditorQuiting;
            EditorApplication.quitting += OnEditorQuiting;
        }

        private static void HandleComponentAdded(Component _obj)
        {
            Type[] _componentsToAdd = ComponentLink.GetComponentsToAdd(_obj);
            foreach (Type _componentToAdd in _componentsToAdd)
            {
                if (_componentToAdd == null) return;
                if (!_obj.gameObject.TryGetComponent(_componentToAdd, out _))
                {
                    $"Automatically adding custom component on {_obj.gameObject.name.Bold().Color(Color.magenta)} of type {_componentToAdd.Name.Bold().Color(Color.magenta)}".Log(Color.white);
                    Component _component = _obj.gameObject.AddComponent(_componentToAdd);
                    if (ComponentLink.ComponentPlacement != ComponentPlacementType.Default)
                        for (int _iterationIndex = 0; _iterationIndex < 20; _iterationIndex++)
                            if (!(ComponentLink.ComponentPlacement == ComponentPlacementType.Top ? ComponentUtility.MoveComponentUp(_component) : ComponentUtility.MoveComponentDown(_component))) break;
                }
            }
        }

        private static void OnEditorQuiting()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            EditorApplication.quitting -= OnEditorQuiting;
        }

        protected enum ComponentPlacementType
        {
            Default,
            Top,
            Bottom
        }
    }
}
#endif