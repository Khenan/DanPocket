using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Umeshu.USystem
{
    [System.Serializable]
    public class SystemNode : ParentNode
    {
        [SerializeField] private List<ScriptableElement> scriptableElements;
        protected override IEnumerable<IGameElement> ObjectsToInstantiate => base.ObjectsToInstantiate.Union(scriptableElements);
    }
}
