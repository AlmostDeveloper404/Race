using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public static int LevelProgression;
    public readonly string _progressingKey = "Progress";

    [SerializeField] private int index = 0;

    private void Awake()
    {
        LevelProgression = PlayerPrefs.HasKey(_progressingKey) ? PlayerPrefs.GetInt(_progressingKey) : 1;
    }


    [ContextMenu("Set PP Index")]
    public void Set()
    {
        PlayerPrefs.SetInt(_progressingKey, index);
    }
}
