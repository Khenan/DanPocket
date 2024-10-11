using Umeshu.Common;
using Umeshu.Uf;
using UnityEngine;
namespace Umeshu.USystem.Pool
{
    public class PoolableChildTransferer : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private bool takeInHierarchyChildrensToo = true;

        public IPoolableChild[] poolableChilds;

        private void Awake()
        {
            UpdateChilds();
        }

        public void UpdateChilds()
        {
            poolableChilds = GetComponents<IPoolableChild>();

            if (takeInHierarchyChildrensToo)
            {
                IPoolableChild[] _hierarchyChildComponents = GetComponentsInChildren<IPoolableChild>();

                poolableChilds = poolableChilds.GetMergedWith(_hierarchyChildComponents);
            }
        }

        public void ResetVars()
        {
            foreach (IPoolableChild _poolableChild in poolableChilds)
            {
                _poolableChild.ResetVars();
                UVarExtension.ResetVars(_poolableChild);
            }
        }
    }
}
