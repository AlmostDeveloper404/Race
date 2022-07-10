using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private Image _foregroundImage;
    [SerializeField] private TMP_Text _policeCarCounterText;
    [SerializeField] private TMP_Text _civilianCarCounterText;
    [SerializeField] private Text _waveCompletedText;
    [SerializeField] private Button _reloadButton;

    [SerializeField] private float _waveAnimationTime;
    [SerializeField] private AnimationCurve _fontSizeAnim;

    private GameObject[] _bulletIcons;
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
        CarShooting.OnReloading += Reload;
        CarShooting.OnShooted += UpdateBulletsIcon;
        CarManager.OnPoliceCarDestroid += UpdatePoliceCarUI;
        CarManager.OnCivilianCarDestroid += UpdateCivilianCarUI;
        GameManager.OnStartSpawnNewWave += ShowWaveEnding;
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

    private void UpdateCivilianCarUI(int currentCivilianCars, int maxCivilianCarsOnLevel)
    {
        _civilianCarCounterText.text = $"{currentCivilianCars}/{maxCivilianCarsOnLevel}";
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

    private void GameOver()
    {
        _waveCompletedText.text = "Game Over!";
        ShowWaveEnding();
    }

    private void LevelCompleted()
    {
        _waveCompletedText.text = "Level Completed!";
    }

    private void OnDisable()
    {
        CarShooting.OnReloading -= Reload;
        CarShooting.OnShooted -= UpdateBulletsIcon;
        CarManager.OnPoliceCarDestroid -= UpdatePoliceCarUI;
        CarManager.OnCivilianCarDestroid -= UpdateCivilianCarUI;
        GameManager.OnStartSpawnNewWave -= ShowWaveEnding;
        GameManager.OnGameOver -= GameOver;
        GameManager.OnLevelCompleted -= LevelCompleted;
    }

}
