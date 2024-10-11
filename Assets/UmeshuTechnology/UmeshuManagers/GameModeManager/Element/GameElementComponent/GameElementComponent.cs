using Umeshu.Uf;
using UnityEngine;

public abstract class GameElementComponent<T> : HeritableGameElement where T : Component
{
    public T Component => component ??= GetComponent<T>();
    private T component;

    protected override void GameElementFirstInitialize() { }
    protected override void GameElementEnableAndReset()
    {
        ResetComponent(Component);
        DesactivateComponent(Component);
    }
    protected override void GameElementPlay() => ActivateComponent(Component);
    protected override void GameElementUpdate() { }

    public override void Pause()
    {
        base.Pause();
        DesactivateComponent(Component);
    }
    public override void Play()
    {
        base.Play();
        ActivateComponent(Component);
    }
    public override void Stop()
    {
        base.Stop();
        DesactivateComponent(Component);
    }
    public override void Resume()
    {
        base.Resume();
        ActivateComponent(Component);
    }

    protected abstract void DesactivateComponent(T _component);
    protected abstract void ActivateComponent(T _component);
    protected abstract void ResetComponent(T _component);

}
