namespace Umeshu.USystem
{
    public abstract class ScriptableSystem : ScriptableElement, IGameSystem
    {
        public virtual void OnEnterGameMode(GameModeKey _gameMode) => IGameElement.ActOverSubSystem(this, _sub => (_sub as IGameSystem)?.OnEnterGameMode(_gameMode));
    }
}