using System.Collections.Generic;
using Umeshu.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Umeshu.Common
{
    public class UAsset_Database : ScriptableObject
    {
        public List<DatabaseEntry> entries = new();
        public List<DatabaseEntry> GetEntries(AssetLabelReference _reference)
        {
            List<DatabaseEntry> _entries = new();
            foreach (DatabaseEntry _entry in entries)
                if (_entry.group == _reference)
                    _entries.Add(_entry);
            return _entries;
        }
    }
}