using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    private GameState gameState = GameState.LOADING;
    public GameState GameState => gameState;

    public void SetGameState(GameState _state)
    {
        gameState = _state;
    }

    private void LaunchLoading()
    {
        SetGameState(GameState.LOADING);
        LoadGame();
    }

    private void LoadGame()
    {

    }

#region Updates
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1)) SetGameState(GameState.LOADING);
        else if(Input.GetKeyDown(KeyCode.Keypad2)) SetGameState(GameState.MAIN_MENU);
        else if(Input.GetKeyDown(KeyCode.Keypad3)) SetGameState(GameState.IN_GAME);
        else if(Input.GetKeyDown(KeyCode.Keypad4)) SetGameState(GameState.PAUSED);
        else if(Input.GetKeyDown(KeyCode.Keypad5)) SetGameState(GameState.GAME_OVER);

        switch (gameState)
        {
            case GameState.LOADING:
                UpdateLoading();
                break;
            case GameState.MAIN_MENU:
                UpdateMainMenu();
                break;
            case GameState.IN_GAME:
                UpdateInGame();
                break;
            case GameState.PAUSED:
                UpdatePaused();
                break;
            case GameState.GAME_OVER:
                UpdateGameOver();
                break;
        }
    }

    private void UpdateLoading()
    {
        Debug.Log("UpdateLoading");
    }

    private void UpdateMainMenu()
    {
        Debug.Log("UpdateMainMenu");
    }

    private void UpdateInGame()
    {
        Debug.Log("UpdateInGame");
    }

    private void UpdatePaused()
    {
        Debug.Log("UpdatePaused");
    }

    private void UpdateGameOver()
    {
        Debug.Log("UpdateGameOver");
    }
#endregion
}