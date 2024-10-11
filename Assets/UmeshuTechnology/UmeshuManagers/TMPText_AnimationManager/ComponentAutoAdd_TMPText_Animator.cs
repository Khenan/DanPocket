#if UNITY_EDITOR
using System;
using TMPro;
using Umeshu.Uf;
using Umeshu.USystem.ComponentAutoAdd;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Umeshu.USystem.TextAnimation
{
    public static class UText_Depedencies
    {
        private const string MENU_ITEM_PATH = "Tools/Assets/Component Depedencies/TMPText_Animator/";

        [MenuItem(MENU_ITEM_PATH + nameof(EnsureTMPAnimatorDepedencies))]
        private static void EnsureTMPAnimatorDepedencies() => UfEditor.EnsureComponentDepecencies((typeof(TMP_Text), typeof(TMPText_Animator)));

        [MenuItem(MENU_ITEM_PATH + nameof(LogTMPAnimatorsInPrefabsWithVariants))]
        private static void LogTMPAnimatorsInPrefabsWithVariants() => UfEditor.LogComponentsInPrefab(false, typeof(TMPText_Animator));

        [MenuItem(MENU_ITEM_PATH + nameof(LogTMPAnimatorsInPrefabsWithoutVariants))]
        private static void LogTMPAnimatorsInPrefabsWithoutVariants() => UfEditor.LogComponentsInPrefab(true, typeof(TMPText_Animator));
    }

    [InitializeOnLoad]
    public class ComponentAutoAdd_TMPText_Animator : ComponentAutoAdd<ComponentAutoAdd_TMPText_Animator>
    {
        static ComponentAutoAdd_TMPText_Animator() => SuscribeToComponentAddedAction();

        protected override ComponentPlacementType ComponentPlacement => ComponentPlacementType.Top;

        protected override Type[] GetComponentsToAdd(Component _obj) => new Type[]{ _obj switch
        {
            TMP_Text => typeof(TMPText_Animator),
            _ => null,
        }};
    }
}
#endif