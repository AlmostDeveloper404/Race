using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _musicButton;

    [SerializeField] private Image _imageSound;
    [SerializeField] private Image _imageMusic;

    [Header("Icons")]
    [SerializeField] private Sprite _soundEnable, _soundDisable, _musicEnable, _musicDisable;


    private void Start()
    {
        int soundIndex = PlayerPrefs.GetInt(SoundManager.SoundsKey);
        int musicIndex = PlayerPrefs.GetInt(SoundManager.MusicKey);

        _imageSound.sprite = soundIndex == 0 ? _soundEnable : _soundDisable;
        _imageMusic.sprite = musicIndex == 0 ? _musicEnable : _musicDisable;

        _soundButton.onClick.AddListener(() => SoundManager.Instance.SwitchSound());
        _musicButton.onClick.AddListener(() => SoundManager.Instance.SwitchMusic());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Helpers.Helper.IsOverUI())
        {
            GameManager.Instance.Play();
        }
    }

    public void UpdateSoundIcon()
    {
        _imageSound.sprite = _imageSound.sprite == _soundEnable ? _soundDisable : _soundEnable;

    }

    public void UpdateMusicIcon()
    {
        _imageMusic.sprite = _imageMusic.sprite == _musicEnable ? _musicDisable : _musicEnable;

    }
}
