using System.Collections.Generic;
using UnityEngine;

namespace Umeshu.USystem
{
    [System.Serializable] //pick this if you want to instantiate sub systems 
    public class ParentNode : ElementNode
    {
        [SerializeReference]
        private List<GameElement> childrensToInstantiate = new();
        protected override IEnumerable<IGameElement> ObjectsToInstantiate => childrensToInstantiate;
    }
}
