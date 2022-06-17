using System;
using System.Collections;
using UnityEngine.SceneManagement;

public enum GameState { GameOver, WaveCompleted, LevelCompleted, StartSpawning }

public class GameManager : Singleton<GameManager>
{

    public static Action OnGameOver;
    public static Action OnWaveCompleted;
    public static Action OnStartSpawnNewWave;
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
            case GameState.StartSpawning:
                OnStartSpawnNewWave?.Invoke();
                break;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
