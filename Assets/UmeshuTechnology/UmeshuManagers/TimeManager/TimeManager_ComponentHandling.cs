using UnityEngine;

namespace Umeshu.USystem.Time
{
    public abstract class TimeManager_ComponentHandling<T> : HeritableGameElement where T : Component
    {
        [SerializeField] private TimeThread thread;
        protected override TimeThread GetThread() => thread;

        #region Game Element Methods

        protected override void GameElementFirstInitialize()
        {
            if (TryGetComponent(out component))
                TimeManager.SuscribeToSpeedChange(Thread, SimulationAdapterMethod);
        }
        protected override void GameElementEnableAndReset() { }
        protected override void GameElementPlay() { }
        protected override void GameElementUpdate() { }

        #endregion

        protected T component;


        protected override void OnDestroy()
        {
            base.OnDestroy();
            TimeManager.UnsuscribeToSpeedChange(Thread, SimulationAdapterMethod);
        }

        protected abstract void SimulationAdapterMethod(float _value);

    }
}
