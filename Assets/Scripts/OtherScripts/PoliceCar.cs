using UnityEngine;

public enum PoliceCarState { Chase, Attack, Idle }


public class PoliceCar : Car
{
    private PlayerCar _playerCar;

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

        _playerCar.OnTurningStarted += ChangeRunway;
        GameManager.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        _playerCar.OnTurningStarted -= ChangeRunway;
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
            switch (PoliceCarState)
            {
                case PoliceCarState.Chase:

                    if (distance < _distanceToAttack)
                    {
                        PoliceCarState = PoliceCarState.Attack;
                    }
                    if (GetCarInFront() || CurrentRunway == _playerCar.CurrentRunway)
                    {
                        Speed = GetCarInFront() ? GetCarInFront().Speed : Speed;
                        ChangeRunway();
                    }

                    break;
                case PoliceCarState.Attack:

                    if (distance > _distanceToAttack)
                    {
                        PoliceCarState = PoliceCarState.Chase;
                    }

                    if (CurrentRunway == _playerCar.CurrentRunway && GetCarInFront())
                    {
                        if (GetCarInFront().CurrentCarType != CarTypes.Player)
                        {
                            ChangeRunway();
                            return;
                        }
                        else
                        {
                            return;
                        }
                        
                    }
                    if (CanTurn(_playerCar.CurrentRunway))
                    {
                        Speed = _policeCarSpeed;
                        CurrentRunway = _playerCar.CurrentRunway;
                        _value = _playerCar.transform.position.x;
                    }
                    else
                    {
                        Speed = GetCarInFront() ? GetCarInFront().Speed : _playerCar.Speed;

                    }

                    break;
                default:
                    break;
            }
        }
    }
    public override void ChangeRunway()
    {
        _runwayBeforeTurn = CurrentRunway;


        base.ChangeRunway();

        //Debug.Log($"{gameObject.name},{_runwayBeforeTurn},{CurrentRunway},{_playerCar.CurrentRunway}");

        if (CurrentRunway == _playerCar.CurrentRunway)
        {
            if (_runwayBeforeTurn != CurrentRunway.Centre && CurrentRunway != CurrentRunway.Centre)
            {
                CurrentRunway = CanTurn(CurrentRunway.Centre) ? CurrentRunway.Centre : _playerCar.CurrentRunway;
                CurrentRunway = CanTurn(CurrentRunway) ? CurrentRunway : _runwayBeforeTurn;
            }
            else if (_runwayBeforeTurn != CurrentRunway.Left && CurrentRunway != CurrentRunway.Left)
            {
                CurrentRunway = CanTurn(CurrentRunway.Left) ? CurrentRunway.Left : _playerCar.CurrentRunway;
            }
            else
            if (_runwayBeforeTurn != CurrentRunway.Right && CurrentRunway != CurrentRunway.Right)
            {
                CurrentRunway = CanTurn(CurrentRunway.Right) ? CurrentRunway.Right : _playerCar.CurrentRunway;
            }
        }
        if (CanTurn(CurrentRunway))
        {
            Speed = _policeCarSpeed;
            ApplyTurn(CurrentRunway);
        }
    }

    public override void Enable(Transform positionToSpawn, CurrentRunway currentRunway)
    {
        base.Enable(positionToSpawn, currentRunway);
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

    public override void Death()
    {
        base.Death();
        CarManager.Instance.RemoveCar(this, false);
    }

    private void GameOver()
    {
        PoliceCarState = PoliceCarState.Idle;
    }
}
