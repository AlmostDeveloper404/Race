using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private Image _foregroundImage;
    [SerializeField] private Text _policeCarCounterText;
    [SerializeField] private Text _waveCompletedText;
    [SerializeField] private Button _reloadButton;
    [SerializeField] private Text _levelText;

    [SerializeField] private float _waveAnimationTime;
    [SerializeField] private AnimationCurve _fontSizeAnim;

    [SerializeField] private Text _hitCivilCar;

    private GameObject[] _bulletIcons;

    [Header("Final Panal")]
    [SerializeField] private TMP_Text _endText;
    [SerializeField] private GameObject _finalPanal;
    [SerializeField] private Button _finalButton;
    [SerializeField] private Text _buttonText;

    private void Awake()
    {
        _bulletIcons = new GameObject[_content.childCount];
        for (int i = 0; i < _bulletIcons.Length; i++)
        {
            _bulletIcons[i] = _content.GetChild(i).gameObject;

            GameObject bulletIcon = _bulletIcons[i];
            bulletIcon.SetActive(i < 4);
        }

        FillArmo();
    }

    private void FillArmo()
    {
        for (int i = 0; i < _bulletIcons.Length; i++)
        {
            GameObject bulletIcon = _bulletIcons[i];
            bulletIcon.SetActive(i < 4);
        }
    }

    private void Discharge()
    {
        for (int i = 0; i < _bulletIcons.Length; i++)
        {
            GameObject bulletIcon = _bulletIcons[i];
            bulletIcon.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameManager.OnLevelCompleted += LevelCompleted;
        GameManager.OnGameOver += GameOver;
        GameManager.OnStartSpawnNewWave += ShowWaveEnding;
        CarShooting.OnReloading += Reload;
        CarShooting.OnShooted += UpdateBulletsIcon;
        CarManager.OnPoliceCarDestroid += UpdatePoliceCarUI;
        CarManager.OnCivilianCarDestroid += ShowDestroedCivilianText;
        SceneManager.sceneLoaded += UpdateCurrentLevel;
    }

    private void UpdateCurrentLevel(Scene arg0, LoadSceneMode arg1)
    {
        _levelText.text = $"Level {arg0.buildIndex}";
    }

    public void UpdateBulletsIcon()
    {
        RemoveBulletIcon();
    }
    private void RemoveBulletIcon()
    {
        for (int i = 0; i < _bulletIcons.Length; i++)
        {
            GameObject bulletIcon = _bulletIcons[i];
            if (bulletIcon.activeSelf)
            {
                bulletIcon.SetActive(false);
                return;
            }
            else
            {
                continue;
            }
        }
    }

    private void ShowWaveEnding()
    {
        StartCoroutine(PlayWaveCompleteAnimation());
    }

    private IEnumerator PlayWaveCompleteAnimation()
    {
        _waveCompletedText.enabled = true;

        for (float i = 0; i < _waveAnimationTime; i += Time.deltaTime * 0.5f)
        {
            _waveCompletedText.fontSize = Mathf.RoundToInt(_fontSizeAnim.Evaluate(i));
            yield return null;
        }
        _waveCompletedText.enabled = false;
    }

    private void UpdatePoliceCarUI(int currentPoliceCars, int maxPoliceCarsOnLevel)
    {
        _policeCarCounterText.text = $"{currentPoliceCars}/{maxPoliceCarsOnLevel}";
    }

    private void Reload(float _reloadingTime)
    {
        StartCoroutine(Reloading(_reloadingTime));
    }

    private IEnumerator Reloading(float reloadingTime)
    {
        Discharge();
        _reloadButton.interactable = false;
        for (float i = reloadingTime; i > 0; i -= Time.deltaTime)
        {
            float value = i / reloadingTime;

            _foregroundImage.fillAmount = value;
            yield return null;
        }
        _foregroundImage.fillAmount = 0;
        _reloadButton.interactable = true;
        FillArmo();
    }

    private void ShowDestroedCivilianText(int current, int max)
    {
        StartCoroutine(ShowMessage());

    }

    private IEnumerator ShowMessage()
    {
        _hitCivilCar.enabled = true;
        yield return Helpers.Helper.GetWait(2f);
        _hitCivilCar.enabled = false;
    }

    private void GameOver()
    {
        _finalButton.onClick.RemoveAllListeners();

        _finalPanal.SetActive(true);
        _buttonText.text = "Try again!";
        _endText.text = "Game Over";
        _finalButton.onClick.AddListener(() => GameManager.Instance.Restart());

    }

    private void LevelCompleted()
    {
        _finalPanal.SetActive(true);
        _buttonText.text = "Next Level";
        _endText.text = "Level Completed!";
        _finalButton.onClick.AddListener(() => GameManager.Instance.NextLevel());
    }

    private void OnDisable()
    {
        CarShooting.OnReloading -= Reload;
        CarShooting.OnShooted -= UpdateBulletsIcon;
        CarManager.OnPoliceCarDestroid -= UpdatePoliceCarUI;
        CarManager.OnCivilianCarDestroid -= ShowDestroedCivilianText;
        GameManager.OnStartSpawnNewWave -= ShowWaveEnding;
        GameManager.OnGameOver -= GameOver;
        GameManager.OnLevelCompleted -= LevelCompleted;
        SceneManager.sceneLoaded -= UpdateCurrentLevel;
    }

}
