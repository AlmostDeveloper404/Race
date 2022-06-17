using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public enum WavesState { Spawinng, EndSpawning }

public class CarManager : Singleton<CarManager>
{
    [SerializeField] private List<Car> _allCarsSpawned = new List<Car>();

    public WavesState WaveState;

    private void Start()
    {
        WaveState = WavesState.Spawinng;
    }

    private void OnEnable()
    {
        GameManager.OnWaveCompleted += WaveCompleted;
    }

    private void WaveCompleted()
    {
        WaveState = WavesState.EndSpawning;
    }

    private void OnDisable()
    {
        GameManager.OnWaveCompleted -= WaveCompleted;
    }
    public void AddCar(Car car)
    {
        _allCarsSpawned.Add(car);
    }

    public void RemoveCar(Car car)
    {
        _allCarsSpawned.Remove(car);

        if (!HasPoliceCars() && WaveState == WavesState.EndSpawning)
        {
            GameManager.Instance.ChangeGameState(GameState.StartSpawning);
        }
    }

    private bool HasPoliceCars()
    {
        bool result = _allCarsSpawned.Any(t => t.CurrentCarType == CarTypes.Police);
        return result;
    }

    public bool HasCivilianCarOnTheLine(CurrentRunway currentRunway)
    {
        return _allCarsSpawned.Any(t => t.CurrentRunway == currentRunway && t.CurrentCarType == CarTypes.Civilian);
    }

    public List<Car> GetSpawnedCars() => _allCarsSpawned;
}
