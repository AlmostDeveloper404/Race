using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarSpawner : Singleton<CarManager>
{
    public static Action OnNextWaveReady;

    private Transform[] _spawnPoints;

    private List<Car> _allCars = new List<Car>();

    [SerializeField] private Wave[] _allLevelWaves;
    private int _currentWaveIndex;

    [SerializeField] private Car _policeCarPref;
    [SerializeField] private Car _civilianCarPref;

    [SerializeField] private float _timeToSpawnWaves;

    private void Awake()
    {
        _allLevelWaves = Resources.LoadAll<Wave>($"LevelsInfo/Level {SceneManager.GetActiveScene().buildIndex}");
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += StopSpawning;
        GameManager.OnStartSpawnNewWave += StartSpawning;
    }
    private void Start()
    {
        FillSpawnPoints();
        LoadAllLevelCars();

        StartSpawning();
    }

    public int GetAllPoliceCarsOnWave()
    {
        int policeCarAmount = 0;

        for (int i = 0; i < _allLevelWaves[_currentWaveIndex].CarInfo.Length; i++)
        {
            CarInfo carInfo = _allLevelWaves[_currentWaveIndex].CarInfo[i];
            if (carInfo.CarType == CarTypes.Police)
            {
                policeCarAmount++;
            }
        }
        return policeCarAmount;
    }

    private void StartSpawning()
    {
        if (_currentWaveIndex == _allLevelWaves.Length)
        {
            GameManager.Instance.ChangeGameState(GameState.LevelCompleted);
            return;
        }
        StartCoroutine(SpawnNewWave());
    }


    private void StopSpawning()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= StopSpawning;
        GameManager.OnStartSpawnNewWave -= StartSpawning;
    }

    private void FillSpawnPoints()
    {
        _spawnPoints = new Transform[transform.childCount];

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _spawnPoints[i] = transform.GetChild(i);
        }
    }

    private void LoadAllLevelCars()
    {
        for (int p = 0; p < 10; p++)
        {
            Car car = Instantiate(_policeCarPref, Vector3.zero, Quaternion.identity);
            car.name = p.ToString();
            _allCars.Add(car);
            car.Disable();
        }

        for (int c = 0; c < 10; c++)
        {
            Car car = Instantiate(_civilianCarPref, Vector3.zero, Quaternion.identity);
            _allCars.Add(car);
            car.Disable();
        }
    }

    private IEnumerator SpawnNewWave()
    {

        OnNextWaveReady?.Invoke();

        yield return Helpers.Helper.GetWait(_timeToSpawnWaves);

        StartCoroutine(StartSpawningCars(_allLevelWaves[_currentWaveIndex]));
        _currentWaveIndex++;
    }


    private IEnumerator StartSpawningCars(Wave currentWave)
    {
        for (int i = 0; i < currentWave.CarInfo.Length; i++)
        {
            yield return new WaitForSeconds(currentWave.CarInfo[i].SpawnInterval);

            for (int c = 0; c < _allCars.Count; c++)
            {
                if (_allCars[c].CurrentCarType == currentWave.CarInfo[i].CarType && !_allCars[c].IsEnabled())
                {
                    Spawn(_allCars[c], currentWave.CarInfo[i].CurrentRunway);
                    break;
                }
                else
                {
                    continue;
                }
            }

        }

        GameManager.Instance.ChangeGameState(GameState.WaveCompleted);
    }



    private void Spawn(Car carToSpawn, CurrentRunway currentRunway)
    {
        int lineIndex = 0;

        switch (currentRunway)
        {
            case CurrentRunway.Left:
                lineIndex = 2;
                break;
            case CurrentRunway.Centre:
                lineIndex = 1;
                break;
            case CurrentRunway.Right:
                lineIndex = 0;
                break;
            default:
                break;
        }

        Transform spawnPoint = carToSpawn.CurrentCarType == CarTypes.Civilian ? _spawnPoints[lineIndex + 3] : _spawnPoints[lineIndex];

        carToSpawn.Enable(spawnPoint, currentRunway);
        CarManager.Instance.AddCar(carToSpawn);
    }
}
