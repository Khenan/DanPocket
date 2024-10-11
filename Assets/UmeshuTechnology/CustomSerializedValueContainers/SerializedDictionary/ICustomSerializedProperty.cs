namespace Umeshu.Utility
{
    public interface ICustomSerializedProperty
    {
#if UNITY_EDITOR
        void Serialize();
#endif
    }
}