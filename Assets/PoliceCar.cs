using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PoliceCarState { Chase, Attack, Idle }

public class PoliceCar : Car
{
    private PlayerCar _playerCar;

    private float _timer = 0f;
    [SerializeField] private float _timeToCheckForPlayer;


    [SerializeField] private float _timeToChangeRunway;
    [SerializeField] private float _distanceToStartAttack;

    public PoliceCarState PoliceCarState;

    private void OnEnable()
    {
        _timer = _timeToCheckForPlayer;

        _playerCar = FindObjectOfType<PlayerCar>();

        PoliceCarState = PoliceCarState.Chase;
        GameManager.OnGameOver += ChangeState;
    }

    private void ChangeState()
    {
        PoliceCarState = PoliceCarState.Idle;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ChangeState;
    }


    private void Update()
    {
        transform.position -= new Vector3(0f, 0f, Speed * Time.deltaTime);

        CheckRoadTraffic();
    }

    private void CheckRoadTraffic()
    {
        _timer += Time.deltaTime;
        if (_timer > _timeToCheckForPlayer)
        {
            _timer = 0;

            switch (PoliceCarState)
            {
                case PoliceCarState.Chase:
                    float distance = Mathf.Abs(_playerCar.transform.position.z - transform.position.z);

                    if (distance < _distanceToStartAttack)
                    {
                        PoliceCarState = PoliceCarState.Attack;
                    }
                    if (_playerCar.CurrentRunway == CurrentRunway || CarManager.Instance.HasCivilianCarOnTheLine(CurrentRunway))
                    {
                        StartCoroutine(ChangeRunway());
                    }

                    break;
                case PoliceCarState.Attack:
                    StopAllCoroutines();
                    StartCoroutine(StartTurn(_playerCar.CurrentRunway));
                    break;
            }
        }
    }

    private IEnumerator ChangeRunway()
    {
        CarManager carManager = CarManager.Instance;


        bool isLeftEmpty = !carManager.HasCivilianCarOnTheLine(CurrentRunway.Left);
        bool isCentreEmpty = !carManager.HasCivilianCarOnTheLine(CurrentRunway.Centre);
        bool isRightEmpty = !carManager.HasCivilianCarOnTheLine(CurrentRunway.Right);

        if (isCentreEmpty && _playerCar.CurrentRunway != CurrentRunway.Centre)
        {
            CurrentRunway = CurrentRunway.Centre;
        }
        else if (isRightEmpty && _playerCar.CurrentRunway != CurrentRunway.Right)
        {
            CurrentRunway = CurrentRunway.Right;
        }
        else if (isLeftEmpty && _playerCar.CurrentRunway != CurrentRunway.Left)
        {
            CurrentRunway = CurrentRunway.Left;

        }
        else
        {
            CurrentRunway = GetAvarageRunway();
        }


        yield return new WaitForSeconds(_timeToChangeRunway);
        StartCoroutine(StartTurn(CurrentRunway));
    }

    private IEnumerator StartTurn(CurrentRunway currentRunway)
    {
        float value = 0;

        switch (currentRunway)
        {
            case CurrentRunway.Left:
                value = -3.25f;
                break;
            case CurrentRunway.Centre:
                value = 0f;
                break;
            case CurrentRunway.Right:
                value = 3.25f;
                break;
            default:
                break;
        }

        for (float i = 0; i < 2f; i += Time.deltaTime)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, value, 3f * Time.deltaTime);

            transform.position = pos;

            yield return null;
        }
    }





    private CurrentRunway GetAvarageRunway()
    {
        List<Car> _allCars = CarManager.Instance.GetSpawnedCars();

        Car[] _allCiviliansCars = _allCars.Where(t => t.CurrentCarType == CarTypes.Civilian).ToArray();

        float _nearestLeftRunwayCivilian;
        float _nearestRightRunwayCivilian;
        float _nearestCentreRunwayCivilian;

        int _rightTangent;
        int _leftTangent;
        int _centreTangent;

        int maxDistance;

        CurrentRunway currentRunway = CurrentRunway.Centre;

        switch (_playerCar.CurrentRunway)
        {
            case CurrentRunway.Left:
                _nearestRightRunwayCivilian = _allCiviliansCars.Where(t => t.CurrentRunway == CurrentRunway.Right).Min(t => Vector3.Distance(transform.position, t.transform.position));
                _nearestCentreRunwayCivilian = _allCiviliansCars.Where(t => t.CurrentRunway == CurrentRunway.Centre).Min(t => Vector3.Distance(transform.position, t.transform.position));

                _rightTangent = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Abs(Mathf.Pow(_nearestRightRunwayCivilian, 2)) - Mathf.Pow(3.25f - transform.position.x, 2)));
                _centreTangent = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Abs(Mathf.Pow(_nearestCentreRunwayCivilian, 2)) - Mathf.Pow(0f - transform.position.x, 2)));

                maxDistance = Mathf.Max(_rightTangent, _centreTangent);

                currentRunway = maxDistance == _rightTangent ? CurrentRunway.Right : CurrentRunway.Centre;
                break;
            case CurrentRunway.Centre:
                _nearestRightRunwayCivilian = _allCiviliansCars.Where(t => t.CurrentRunway == CurrentRunway.Right).Min(t => Vector3.Distance(transform.position, t.transform.position));
                _nearestLeftRunwayCivilian = _allCiviliansCars.Where(t => t.CurrentRunway == CurrentRunway.Left).Min(t => Vector3.Distance(transform.position, t.transform.position));

                _rightTangent = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Abs(Mathf.Pow(_nearestRightRunwayCivilian, 2)) - Mathf.Pow(3.25f - transform.position.x, 2)));
                _leftTangent = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Abs(Mathf.Pow(_nearestLeftRunwayCivilian, 2)) - Mathf.Pow(-3.25f - transform.position.x, 2)));

                maxDistance = Mathf.Max(_rightTangent, _leftTangent);

                currentRunway = maxDistance == _rightTangent ? CurrentRunway.Right : CurrentRunway.Left;
                break;
            case CurrentRunway.Right:
                _nearestLeftRunwayCivilian = _allCiviliansCars.Where(t => t.CurrentRunway == CurrentRunway.Left).Min(t => Vector3.Distance(transform.position, t.transform.position));
                _nearestCentreRunwayCivilian = _allCiviliansCars.Where(t => t.CurrentRunway == CurrentRunway.Centre).Min(t => Vector3.Distance(transform.position, t.transform.position));

                _leftTangent = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Abs(Mathf.Pow(_nearestLeftRunwayCivilian, 2)) - Mathf.Pow(-3.25f - transform.position.x, 2)));
                _centreTangent = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Abs(Mathf.Pow(_nearestCentreRunwayCivilian, 2)) - Mathf.Pow(0f - transform.position.x, 2)));

                maxDistance = Mathf.Max(_leftTangent, _centreTangent);

                currentRunway = maxDistance == _leftTangent ? CurrentRunway.Left : CurrentRunway.Centre;
                break;
            default:
                break;
        }

        return currentRunway;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        CivilianCar civilianCar = other.GetComponent<CivilianCar>();
        Bullet bullet = other.GetComponent<Bullet>();
        if (civilianCar)
        {
            civilianCar.Death();
            Death();
        }

        if (bullet)
        {
            bullet.Disable();
            Death();
        }

    }


    public override void Death()
    {
        base.Death();
        CarManager.Instance.RemoveCar(this);
    }
}
