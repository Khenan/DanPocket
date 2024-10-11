using UnityEngine;

namespace Umeshu.Utility
{

    [CreateAssetMenu(fileName = "Texture2DImportSettings", menuName = "ScriptableObjects/UmeshuTechnology/Editor/Texture2DImportSettings")]
    public class Texture2DImportSettings : ScriptableObject
    {
        public bool enableCompression = false;
        public bool moveTemporaryTexturesToSpecificFolder = false;
        public string[] foldersToIgnore = new string[] { "TEMPORARY", "Ferr", "ExternalPackages", "FMOD", "Prevent_AutoResize" };
    }
}
