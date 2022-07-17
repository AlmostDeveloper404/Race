using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public static int LevelProgression;
    public readonly string _progressingKey = "Progress";

    private void Awake()
    {
        LevelProgression = PlayerPrefs.HasKey(_progressingKey) ? PlayerPrefs.GetInt(_progressingKey) : 1;
    }
}
