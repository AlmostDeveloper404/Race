using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public enum WavesState { Spawinng, EndSpawning }

public class CarManager : Singleton<CarManager>
{
    public static Action<int, int> OnPoliceCarDestroid;
    public static Action<int, int> OnCivilianCarDestroid;

    private List<Car> _allCarsSpawned = new List<Car>();

    [SerializeField] private int _availableToDestroyCivilian;
    private int _currentPoliceCarAmount;
    private int _maxPoliceCarOnLevel;
    private int _currentlyDestroidCivilians = 0;

    private CarSpawner _carSpawner;

    private bool _isWaveCompleted = false;

    public WavesState WaveState;

    private void Awake()
    {
        _carSpawner = GetComponent<CarSpawner>();
    }

    private void UpdateCarsInWave()
    {
        _maxPoliceCarOnLevel = _carSpawner.GetAllPoliceCarsOnWave();

        _currentPoliceCarAmount = _maxPoliceCarOnLevel;

        OnPoliceCarDestroid?.Invoke(_currentPoliceCarAmount, _maxPoliceCarOnLevel);
        OnCivilianCarDestroid?.Invoke(_currentlyDestroidCivilians, _availableToDestroyCivilian);

        WaveState = WavesState.Spawinng;
    }

    private void OnEnable()
    {
        GameManager.OnWaveCompleted += WaveCompleted;
        CarSpawner.OnNextWaveReady += UpdateCarsInWave;
    }

    private void WaveCompleted()
    {
        WaveState = WavesState.EndSpawning;
        _isWaveCompleted = false;
    }

    private void OnDisable()
    {
        GameManager.OnWaveCompleted -= WaveCompleted;
        CarSpawner.OnNextWaveReady -= UpdateCarsInWave;
    }
    public void AddCar(Car car)
    {
        _allCarsSpawned.Add(car);
    }

    public void RemoveCar(Car car, bool isKilled)
    {

        if (car.CurrentCarType == CarTypes.Police)
        {
            _currentPoliceCarAmount--;
            OnPoliceCarDestroid?.Invoke(_currentPoliceCarAmount, _maxPoliceCarOnLevel);
        }
        if (car.CurrentCarType == CarTypes.Civilian && isKilled)
        {
            _currentlyDestroidCivilians++;
            OnCivilianCarDestroid?.Invoke(_currentlyDestroidCivilians, _availableToDestroyCivilian);

            if (_currentlyDestroidCivilians == _availableToDestroyCivilian)
            {
                GameManager.Instance.ChangeGameState(GameState.GameOver);
            }
        }

        _allCarsSpawned.Remove(car);

        if (!HasPoliceCars() && WaveState == WavesState.EndSpawning && !_isWaveCompleted)
        {
            _isWaveCompleted = true;
            GameManager.Instance.ChangeGameState(GameState.StartSpawning);
        }
    }

    private bool HasPoliceCars()
    {
        bool result = _allCarsSpawned.Any(t => t.CurrentCarType == CarTypes.Police);
        return result;
    }

    public bool HasCivilianCarOnTheLine(CurrentRunway currentRunway, Vector3 position)
    {
        return _allCarsSpawned.Any(t => t.CurrentRunway == currentRunway && t.CurrentCarType == CarTypes.Civilian && t.transform.position.z < position.z);
    }

    public List<Car> GetSpawnedCars() => _allCarsSpawned;
}
