using System;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.USystem
{
    public sealed class GameManager : UmeshuGameManager
    {
        protected override IGameModesInfos GameModesInfos => gameModesInfos;
        [SerializeField] private GameModesInfos gameModesInfos;

        public static new GameManager Instance => UmeshuGameManager.Instance as GameManager;
        public GameMode LastGameModeID => gameModesInfos.GetGameModeID(LastGameModeKey);
        public GameMode CurrentGameModeID => gameModesInfos.GetGameModeID(CurrentGameModeKey);
        public bool IsInGameMode(GameMode _gameModeID) => CurrentGameModeID == _gameModeID;
        public void GoToGameMode(GameMode _gameModeID) => GoToGameMode(gameModesInfos.GetGameModeKey(_gameModeID));

        private const string LOG_CATEGORY = "GameManager";

        protected override void GameModeStart()
        {
            "GameModeStart".Log(Color.green, LOG_CATEGORY);
        }

        protected override void GameModeEnd()
        {
            if (CurrentGameModeKey == null) return;
        }

        public int GetLastGameMode()
        {
            return LastGameModeKey != null ? (int)LastGameModeID : -1;
        }

        public void GoTo_TitleScreen() => GoToGameMode(GameMode.TitleScreen);
        public void GoTo_LevelSelection() => GoToGameMode(GameMode.LevelSelection);
        public void GoTo_Game() => GoToGameMode(GameMode.Game);

        public override UnityEngine.AddressableAssets.AssetLabelReference[] GetPermanentPackages()
        {
            throw new NotImplementedException();
        }
    }

    [System.Serializable]
    public class GameModesInfos : UmeshuGameModesInfos<GameMode> { }

    public enum GameMode
    {
        TitleScreen,
        LevelSelection,
        Game
    }
}