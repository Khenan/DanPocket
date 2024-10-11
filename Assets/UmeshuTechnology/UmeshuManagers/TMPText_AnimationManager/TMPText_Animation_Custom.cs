using UnityEngine;

namespace Umeshu.USystem.TextAnimation
{
    public abstract class TMPText_Animation_Custom : TMPText_Animation
    {
        protected internal sealed override string Key => this.GetType().Name.Replace("TMPText_Animation_", "");
        protected override AnimationType Animation => AnimationType.Custom;
    }
}