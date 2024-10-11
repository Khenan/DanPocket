using System;
using System.Linq;
using Umeshu.Uf;

namespace Umeshu.Utility
{
    [System.Serializable]
    public class StringDatabaseBasedSelector<TDatabase, TValue> : ArrayBasedSelector<string, TValue> where TDatabase : UnityEngine.Object, IPickableStringDatabase
    {
        public override string ConvertKeyToString(string _key) => _key;

        public override string[] GetCollection()
        {
#if UNITY_EDITOR
            TDatabase _database = UfEditor.GetAssetOfType<TDatabase>();
            return _database.GetCollection();
#else
            return new string[0];
#endif
        }
    }
}
