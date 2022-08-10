using UnityEngine;

public enum PoliceCarState { Chase, Attack, Idle }


public class PoliceCar : ActiveCars
{
    private PlayerCar _playerCar;

    [SerializeField] private GameObject _deathCar;
    [SerializeField] private Vector3 _offset;

    [SerializeField] private float _distanceToAttack;

    public PoliceCarState PoliceCarState;
    private float _policeCarSpeed;

    private CurrentRunway _runwayBeforeTurn;

    private void Awake()
    {
        _policeCarSpeed = Speed;
        _playerCar = FindObjectOfType<PlayerCar>();
    }

    private void OnEnable()
    {
        PoliceCarState = PoliceCarState.Chase;
        Speed = _policeCarSpeed;
        _timer = _callsInSec;
        GameManager.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= GameOver;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void CheckRoadTrafic()
    {
        base.CheckRoadTrafic();
        _timer += Time.deltaTime;
        if (_timer > _callsInSec)
        {
            _timer = 0;
            float distance = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(Vector3.Distance(_playerCar.transform.position, transform.position), 2f) -
                                Mathf.Pow(Vector3.Distance(new Vector3(_playerCar.transform.position.x, 0f, 0f), new Vector3(transform.position.x, 0f, 0f)), 2f)));
            Car car = GetCarInFront();


            switch (PoliceCarState)
            {
                case PoliceCarState.Chase:

                    if (distance < _distanceToAttack)
                    {
                        PoliceCarState = PoliceCarState.Attack;
                    }
                    if (car && CurrentRunway == car.CurrentRunway && car.CurrentCarType == CarTypes.Civilian || CurrentRunway == _playerCar.CurrentRunway)
                    {
                        ChangeRunway();
                    }
                    else if (car && CurrentRunway == car.CurrentRunway && car.CurrentCarType == CarTypes.Police)
                    {
                        Speed = car ? car.Speed : Speed;
                    }
                    else
                    {
                        Speed = _policeCarSpeed;
                    }

                    break;
                case PoliceCarState.Attack:

                    if (distance > _distanceToAttack)
                    {
                        PoliceCarState = PoliceCarState.Chase;
                    }
                    if (CanTurn(_playerCar.CurrentRunway))
                    {
                        Speed = _policeCarSpeed;
                        CurrentRunway = _playerCar.CurrentRunway;
                        ApplyTurn(_playerCar.CurrentRunway);
                        PoliceCarState = PoliceCarState.Idle;
                    }
                    else
                    {
                        if (CurrentRunway == _playerCar.CurrentRunway)
                        {
                            Speed = _policeCarSpeed;
                            ApplyTurn(_playerCar.CurrentRunway);
                        }
                        else
                        {
                            Speed = _playerCar.Speed;
                        }

                    }

                    break;
                case PoliceCarState.Idle:
                    _value = _playerCar.transform.position.x;
                    break;
                default:
                    break;
            }
        }
    }
    private void ChangeRunway()
    {
        CurrentRunway playerRunway = _playerCar.CurrentRunway;
        _runwayBeforeTurn = CurrentRunway;


        CurrentRunway currentRunway = GetRunwayWithCivilianTraffic();

        if (currentRunway == _playerCar.CurrentRunway)
        {
            if (_runwayBeforeTurn != CurrentRunway.Centre && currentRunway != CurrentRunway.Centre)
            {
                currentRunway = CanTurn(CurrentRunway.Centre) ? CurrentRunway.Centre : _playerCar.CurrentRunway;
                currentRunway = CanTurn(currentRunway) ? currentRunway : _runwayBeforeTurn;
            }
            else if (_runwayBeforeTurn != CurrentRunway.Left && currentRunway != CurrentRunway.Left)
            {
                currentRunway = CanTurn(CurrentRunway.Left) ? CurrentRunway.Left : _playerCar.CurrentRunway;
            }
            else
            if (_runwayBeforeTurn != CurrentRunway.Right && currentRunway != CurrentRunway.Right)
            {
                currentRunway = CanTurn(CurrentRunway.Right) ? CurrentRunway.Right : _playerCar.CurrentRunway;
            }
        }

        if (CanTurn(currentRunway))
        {
            Speed = _policeCarSpeed;
            ApplyTurn(currentRunway);
        }
        else
        {
            if (CanTurn(playerRunway))
            {
                Speed = _policeCarSpeed;
                ApplyTurn(playerRunway);
            }
        }
    }

    public override void Enable(Transform positionToSpawn, CurrentRunway currentRunway, Customization customization)
    {
        base.Enable(positionToSpawn, currentRunway, customization);
        switch (currentRunway)
        {
            case CurrentRunway.Left:
                _value = _lineSizes.x;
                break;
            case CurrentRunway.Centre:
                _value = _lineSizes.y;
                break;
            case CurrentRunway.Right:
                _value = _lineSizes.z;
                break;
            default:
                break;
        }
        CurrentRunway = currentRunway;
    }

    public override void Turn()
    {
        base.Turn();
    }

    public override void Disable()
    {
        base.Disable();
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Bullet bullet = other.GetComponent<Bullet>();

        if (bullet)
        {
            bullet.Disable();
            Death();
        }

    }

    public void StopPoliceCar()
    {
        Speed = 6f;
    }

    public override void Death()
    {
        base.Death();
        _deathCar.SetActive(true);
        _deathCar.transform.parent = null;
        _deathCar.transform.position = transform.position + _offset;

        SoundManager.Instance.PlaySound(_deathSound);
        CarManager.Instance.RemoveCar(this, false);
    }

    private void GameOver()
    {
        PoliceCarState = PoliceCarState.Idle;
        this.enabled = false;
    }
}
