using System.Collections.Generic;
using UnityEngine;

namespace Umeshu.Common
{
    public abstract class DynamicStringDatabase : ScriptableObject
    {
        public List<string> entries = new();
    }
}