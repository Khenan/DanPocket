using System;
using Umeshu.Utility;

namespace Umeshu.USystem
{
    public interface IGameModesInfos
    {
        GameModeInfo this[GameModeKey _gameModeKey] { get; }
        GameModeKey StartGameMode { get; }
        float TransitionFadeInDuration { get; }
        float TransitionFadeOutDuration { get; }
        string TransitionSceneName { get; }

        void CreateGameModeKeys();
    }
}