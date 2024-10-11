using UnityEngine;

namespace Umeshu.USystem.Time
{
    public class TimeManager_ParticleSystemSimulationSpeedHandling : TimeManager_ComponentHandling<ParticleSystem>
    {
        private float startSimulationSpeed = 1;

        protected override void GameElementFirstInitialize()
        {
            base.GameElementFirstInitialize();
            startSimulationSpeed = component.main.simulationSpeed;
        }

        protected override void SimulationAdapterMethod(float _value)
        {
            ParticleSystem.MainModule _main = component.main;
            _main.simulationSpeed = _value * startSimulationSpeed;
        }

    }
}