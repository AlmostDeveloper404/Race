using System;
using UnityEngine;

public enum GameState { GameOver, WaveCompleted, LevelCompleted }

public class GameManager : Singleton<GameManager>
{

    public static Action OnGameOver;
    public static Action OnWaveCompleted;
    public static Action OnLevelCompleted;

    public void ChangeGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GameOver:
                OnGameOver?.Invoke();
                break;
            case GameState.LevelCompleted:
                OnLevelCompleted?.Invoke();
                break;
            case GameState.WaveCompleted:
                OnWaveCompleted?.Invoke();
                break;
        }
    }
}
