#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Umeshu.USystem.ComponentAutoAdd
{
    [InitializeOnLoad]
    public class ComponentAutoAdd_GameElement : ComponentAutoAdd<ComponentAutoAdd_GameElement>
    {
        static ComponentAutoAdd_GameElement() => SuscribeToComponentAddedAction();

        protected override Type[] GetComponentsToAdd(Component _obj)
        {
            string _wantedComponent = "GameElement" + _obj.GetType().Name;
            return new Type[] { Type.GetType(_wantedComponent) };
        }
    }
}
#endif