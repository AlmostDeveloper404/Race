using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;



public class PlayerCar : Car
{
    private float _timer;
    [SerializeField] private float _timeToCheckForCivilianCarOnAWay;

    private void Start()
    {
        CurrentRunway = CurrentRunway.Centre;
    }

    private void Update()
    {
        transform.position -= new Vector3(0f, 0f, Speed * Time.deltaTime);
        CheckForCivilianCars();
    }

    private void CheckForCivilianCars()
    {
        _timer += Time.deltaTime;
        if (_timer > _timeToCheckForCivilianCarOnAWay)
        {
            if (CarManager.Instance.HasCivilianCarOnTheLine(CurrentRunway))
            {
                ChangeTheLine();
            }
            _timer = 0;
        }

    }

    public void ChangeTheLine()
    {
        CarManager carManager = CarManager.Instance;


        bool isLeftEmpty = !carManager.HasCivilianCarOnTheLine(CurrentRunway.Left);
        bool isCentreEmpty = !carManager.HasCivilianCarOnTheLine(CurrentRunway.Centre);
        bool isRightEmpty = !carManager.HasCivilianCarOnTheLine(CurrentRunway.Right);

        float value = 0;

        if (isCentreEmpty)
        {
            CurrentRunway = CurrentRunway.Centre;
            value = 0;
        }
        else if (isRightEmpty)
        {
            CurrentRunway = CurrentRunway.Right;
            value = 3.25f;
        }
        else if (isLeftEmpty)
        {
            CurrentRunway = CurrentRunway.Left;
            value = -3.25f;

        }
        else
        {
            CurrentRunway = GetMostFarCivilianCarOnLines();
            switch (CurrentRunway)
            {
                case CurrentRunway.Left:
                    value = -3.25f;
                    break;
                case CurrentRunway.Centre:
                    value = 0;
                    break;
                case CurrentRunway.Right:
                    value = 3.25f;
                    break;
                default:
                    break;
            }
        }

        StartCoroutine(StartTurn(value));
    }

    private CurrentRunway GetMostFarCivilianCarOnLines()
    {
        List<Car> _allCars = CarManager.Instance.GetSpawnedCars();

        Car[] _allCiviliansCars = _allCars.Where(t => t.CurrentCarType == CarTypes.Civilian).ToArray();

        float _nearestLeftRunwayCivilian = _allCiviliansCars.Where(t => t.CurrentRunway == CurrentRunway.Left).Min(t => Vector3.Distance(transform.position, t.transform.position));
        float _nearestRightRunwayCivilian = _allCiviliansCars.Where(t => t.CurrentRunway == CurrentRunway.Right).Min(t => Vector3.Distance(transform.position, t.transform.position));
        float _nearestCentreRunwayCivilian = _allCiviliansCars.Where(t => t.CurrentRunway == CurrentRunway.Centre).Min(t => Vector3.Distance(transform.position, t.transform.position));



        int _rightTangent = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Abs(Mathf.Pow(_nearestRightRunwayCivilian, 2)) - Mathf.Pow(3.25f - transform.position.x, 2)));
        int _leftTangent = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Abs(Mathf.Pow(_nearestLeftRunwayCivilian, 2)) - Mathf.Pow(-3.25f - transform.position.x, 2)));
        int _centreTangent = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Abs(Mathf.Pow(_nearestCentreRunwayCivilian, 2)) - Mathf.Pow(0f - transform.position.x, 2)));

        int maxDistance = Mathf.Max(_rightTangent, _leftTangent, _centreTangent);

        Debug.Log($"Right: {_rightTangent},Left: {_leftTangent},Centre: {_centreTangent}");

        CurrentRunway currentRunway = CurrentRunway.Centre;

        if (maxDistance == _rightTangent)
        {
            currentRunway = CurrentRunway.Right;
        }

        if (maxDistance == _leftTangent)
        {
            currentRunway = CurrentRunway.Left;
        }

        if (maxDistance == _centreTangent)
        {
            currentRunway = CurrentRunway.Centre;
        }

        return currentRunway;


    }


    private IEnumerator StartTurn(float value)
    {
        for (float i = 0; i < 2; i += Time.deltaTime)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, value, 3f * Time.deltaTime);

            transform.position = pos;

            yield return null;
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        PoliceCar policeCar = other.GetComponent<PoliceCar>();

        if (policeCar)
        {
            Death();
        }
    }

    public override void Death()
    {
        base.Death();
        GameManager.Instance.ChangeGameState(GameState.GameOver);
        StopAllCoroutines();

    }
}
