using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { CutScene, Game, GameOver, WaveCompleted, LevelCompleted, StartSpawning }

public class GameManager : Singleton<GameManager>
{

    public static Action OnGameOver;
    public static Action OnWaveCompleted;
    public static Action OnStartSpawnNewWave;
    public static Action OnLevelCompleted;
    public static Action OnPlayingAnimation;
    public static Action OnCutSceneStarted;
    public static Action OnGameStarted;

    private static int _unloadedSceneIndex;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        
        if (SceneManager.GetActiveScene().buildIndex != 0 && _unloadedSceneIndex == 0)
        {
            Debug.Log("Nope");
            ChangeGameState(GameState.CutScene);
        }
        else
        {
            ChangeGameState(GameState.Game);
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    public void ChangeGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.CutScene:
                OnCutSceneStarted?.Invoke();
                break;
            case GameState.Game:
                OnGameStarted?.Invoke();
                break;
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

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
    }


    public void CutSceneEnd()
    {
        ChangeGameState(GameState.Game);
    }

    public void BackToMenu()
    {
        _unloadedSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(0);
    }

    [ContextMenu("Next Level")]
    public void NextLevel()
    {
        _unloadedSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Restart()
    {
        _unloadedSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Play()
    {
        _unloadedSceneIndex = 0;
        SceneManager.LoadScene(LevelManager.LevelProgression);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
