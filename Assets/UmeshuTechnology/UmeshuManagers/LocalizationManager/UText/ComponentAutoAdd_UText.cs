#if UNITY_EDITOR
using System;
using TMPro;
using Umeshu.Uf;
using Umeshu.USystem.ComponentAutoAdd;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Umeshu.USystem.Text
{
    public static class UText_Depedencies
    {
        private const string MENU_ITEM_PATH = "Tools/Assets/Component Depedencies/LocalizedText/";

        [MenuItem(MENU_ITEM_PATH + nameof(EnsureLocalizedTextDepedencies))]
        private static void EnsureLocalizedTextDepedencies() => UfEditor.EnsureComponentDepecencies(
            (typeof(TMP_Text), typeof(UText_TMPText)),
            (typeof(UnityEngine.UI.Text), typeof(UText_Text)),
            (typeof(TextMesh), typeof(UText_TextMesh))
            );

        [MenuItem(MENU_ITEM_PATH + nameof(LogLocalizedTextsInPrefabsWithVariants))]
        private static void LogLocalizedTextsInPrefabsWithVariants() => UfEditor.LogComponentsInPrefab(false,
            typeof(UText_TMPText),
            typeof(UText_Text),
            typeof(UText_TextMesh));

        [MenuItem(MENU_ITEM_PATH + nameof(LogLocalizedTextsInPrefabsWithoutVariants))]
        private static void LogLocalizedTextsInPrefabsWithoutVariants() => UfEditor.LogComponentsInPrefab(true,
            typeof(UText_TMPText),
            typeof(UText_Text),
            typeof(UText_TextMesh));
    }

    [InitializeOnLoad]
    public class ComponentAutoAdd_LocalizedText : ComponentAutoAdd<ComponentAutoAdd_LocalizedText>
    {
        static ComponentAutoAdd_LocalizedText() => SuscribeToComponentAddedAction();

        protected override ComponentPlacementType ComponentPlacement => ComponentPlacementType.Top;

        protected override Type[] GetComponentsToAdd(Component _obj) => new Type[]{ _obj switch
        {
            TMP_Text => typeof(UText_TMPText),
            UnityEngine.UI.Text => typeof(UText_Text),
            TextMesh => typeof(UText_TextMesh),
            _ => null,
        }};
    }
}
#endif