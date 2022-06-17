using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    private Transform[] _spawnPoints;

    private List<Car> _allCars = new List<Car>();

    [SerializeField] private Wave[] _allLevelWaves;
    private int _currentWaveIndex;

    [SerializeField] private Car _policeCarPref;
    [SerializeField] private Car _civilianCarPref;

    [SerializeField] private float _timeToSpawnWaves;

    private void Awake()
    {
        FillSpawnPoints();
        LoadAllLevelCars();
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += StopSpawning;
        GameManager.OnStartSpawnNewWave += StartSpawning;
    }
    private void Start()
    {
        StartSpawning();
    }

    private void StartSpawning()
    {
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
        Debug.Log("Yep");
        yield return new WaitForSeconds(_timeToSpawnWaves);
        StartCoroutine(StartSpawningCars(_allLevelWaves[_currentWaveIndex]));
        _currentWaveIndex++;
    }


    private IEnumerator StartSpawningCars(Wave currentWave)
    {
        for (int i = 0; i < currentWave.CarInfo.Length; i++)
        {

            for (int c = 0; c < _allCars.Count; c++)
            {
                if (_allCars[c].CurrentCarType == currentWave.CarInfo[i].CarType && !_allCars[c].IsEnabled())
                {
                    Spawn(_allCars[c]);
                    break;
                }
                else
                {
                    continue;
                }
            }

            yield return new WaitForSeconds(currentWave.CarInfo[i].SpawnInterval);
        }

        GameManager.Instance.ChangeGameState(GameState.WaveCompleted);
    }



    private void Spawn(Car carToSpawn)
    {
        int lineIndex = Random.Range(0, _spawnPoints.Length);

        CurrentRunway currentRunway = CurrentRunway.Centre;

        switch (lineIndex)
        {
            case 0:
                currentRunway = CurrentRunway.Right;
                break;
            case 1:
                currentRunway = CurrentRunway.Centre;
                break;
            case 2:
                currentRunway = CurrentRunway.Left;
                break;
        }

        Transform spawnPoint = _spawnPoints[lineIndex];

        carToSpawn.Enable(spawnPoint, currentRunway);
        CarManager.Instance.AddCar(carToSpawn);

        if (_allCars.Count == 0) GameManager.Instance.ChangeGameState(GameState.LevelCompleted);
    }
}
