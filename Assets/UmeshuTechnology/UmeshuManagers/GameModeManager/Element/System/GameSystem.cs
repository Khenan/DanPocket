using System.Linq;
using Umeshu.Uf;

namespace Umeshu.USystem
{
    public abstract class GameSystem<T> : BaseSystem<T>, IGameSystem where T : GameSystem<T>
    {
        #region Singleton Management
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!UnityEngine.Application.isPlaying) return UfEditor.GetAllPrefabsOfType<T>().FirstOrDefault();
#endif
                return instance;
            }
        }
        private static T instance;
        #endregion

        protected sealed override bool IsInstanceExisting() => Instance != null;
        protected sealed override void SetInstance() => instance = this as T;
        protected sealed override bool IsInstanceImmortal() => true;
    }
}