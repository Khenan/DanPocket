#if UNITY_EDITOR

using UnityEditor;

namespace Umeshu.Utility
{


    [CustomEditor(typeof(IndexedValuesAnimator), true)]
    [CanEditMultipleObjects]
    public class ListValuesSetter_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            for (int _i = 0; _i < targets.Length; _i++)
            {
                IndexedValuesAnimator _target = (IndexedValuesAnimator)targets[_i];
                _target.UpdateValues(_forceUpdate: true);
            }
        }
    }
}
#endif
