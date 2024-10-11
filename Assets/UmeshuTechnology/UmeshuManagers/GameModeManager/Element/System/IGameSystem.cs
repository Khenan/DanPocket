namespace Umeshu.USystem
{
    public interface IGameSystem : IGameElement
    {
        public void OnEnterGameMode(GameModeKey _gameMode);
    }
}

