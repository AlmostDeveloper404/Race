using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CarManager : Singleton<CarManager>
{
    [SerializeField] private List<Car> _allCarsSpawned = new List<Car>();

    

    public void AddCar(Car car)
    {
        _allCarsSpawned.Add(car);
    }

    public void RemoveCar(Car car)
    {
        _allCarsSpawned.Remove(car);

        if (!HasPoliceCars()) GameManager.Instance.ChangeGameState(GameState.WaveCompleted);
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
