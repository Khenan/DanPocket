using UnityEngine;

namespace Umeshu.USystem.Time
{
    public class TimeManager_AnimatorHandling : TimeManager_ComponentHandling<Animator>
    {
        private int parameterKey;
        [SerializeField] private string parameter = "SpeedMultiplier";

        protected override void GameElementFirstInitialize()
        {
            base.GameElementFirstInitialize();
            parameterKey = Animator.StringToHash(parameter);
        }

        protected override void SimulationAdapterMethod(float _value) => component.SetFloat(parameterKey, _value);


    }
}
