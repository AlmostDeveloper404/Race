using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource[] _allSounds;
    [SerializeField] private AudioSource _music;

    public const string SoundsKey = "Sound";
    public const string MusicKey = "Music";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _allSounds = new AudioSource[transform.childCount];

        for (int i = 0; i < _allSounds.Length; i++)
        {
            _allSounds[i] = transform.GetChild(i).GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        AudioListener.pause = PlayerPrefs.GetInt(SoundsKey) == 0 ? false : true;
        _music.mute = PlayerPrefs.GetInt(MusicKey) == 0 ? false : true;
    }

    public void PlaySound(AudioClip audioClip)
    {
        for (int i = 0; i < _allSounds.Length; i++)
        {
            AudioSource audioSource = _allSounds[i];

            if (audioSource.clip == audioClip)
            {
                audioSource.Play();
            }

        }
    }


    public void SwitchSound()
    {
        Debug.Log("Yep");
        AudioListener.pause = AudioListener.pause == true ? false : true;
        PlayerPrefs.SetInt(SoundsKey, AudioListener.pause == false ? 0 : 1);
    }

    public void SwitchMusic()
    {
        Debug.Log("Yep");
        _music.mute = _music.mute == true ? false : true;
        PlayerPrefs.SetInt(MusicKey, _music.mute == false ? 0 : 1);
    }

}
