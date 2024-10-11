using Umeshu.USystem.Time;
using UnityEngine;

namespace Umeshu.USystem.PhysicsScaler
{
    using Time = UnityEngine.Time;
    public sealed class PhysicsScalerManager : GameSystem<PhysicsScalerManager>
    {
        protected override TimeThread GetThread() => TimeThread.Player;

        protected override void SystemFirstInitialize() => Physics2D.simulationMode = SimulationMode2D.Script;
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate() { }

        private void FixedUpdate()
        {
            if (CThread == null)
                return;
            if (Physics2D.simulationMode != SimulationMode2D.Script)
                Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.Simulate(Time.fixedDeltaTime * CThread.SpeedMultiplier);
        }

    }
}