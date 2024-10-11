using System;
using Umeshu.Utility;
using UnityEngine;

namespace Umeshu.USystem.TSV
{
    public class TsvDatabase_File : ScriptableObject
    {
        public SerializedDictionary<string, SerializedDictionary<string, string>> data = new();
    }
}