using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCars : Car
{
    [SerializeField] private float _turningSpeed;

    [Header("Box")]
    [SerializeField] protected Transform _rightBox;
    [SerializeField] protected Transform _leftBox;
    [SerializeField] protected float _boxOffset;
    [SerializeField] protected float _boxWidth;
    [SerializeField] protected float _boxLength;
    [SerializeField] protected Vector2 _forwardBoxSize;

    [Header("Sounds")]
    [SerializeField] protected AudioClip _turningLeftSound;
    [SerializeField] protected AudioClip _turningRightSound;

    [SerializeField] protected float _callsInSec;

    private Vector3 _currentPos;

    private BoxCollider _boxCollider;
    private Vector3 _boxSizeLR;
    private Vector3 _boxSizeForward;
    [SerializeField] private LayerMask _turnMask;
    [SerializeField] private LayerMask _carInFrontMask;

    [SerializeField] private TrailRenderer[] _renderers;
    [SerializeField] private float _timeToEmit;



    public virtual void Start()
    {
        _animator = GetComponent<Animator>();
        FillData();
    }

    public override void CheckRoadTrafic()
    {
        base.CheckRoadTrafic();
        Turn();
    }

    private CurrentRunway GetFarestCar()
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

    protected void ApplyTurn(CurrentRunway currentRunway)
    {
        CurrentRunway = currentRunway;

        float value = 0;

        switch (currentRunway)
        {
            case CurrentRunway.Left:
                value = _lineSizes.x;
                break;
            case CurrentRunway.Centre:
                value = _lineSizes.y;
                break;
            case CurrentRunway.Right:
                value = _lineSizes.z;
                break;
            default:
                break;
        }
        if (_value != value)
        {
            _animator.SetTrigger(_value < value ? Animations.TurnRight : Animations.TurnLeft);
            SoundManager.Instance.PlaySound(_value < value ? _turningRightSound : _turningLeftSound);
            SwitchEmmiting();
            StartCoroutine(DisableEmiting());
        }

        _value = value;
    }

    protected CurrentRunway GetRunwayWithCivilianTraffic()
    {
        CarManager carManager = CarManager.Instance;


        bool isLeftEmpty = !carManager.HasCivilianCarOnTheLine(CurrentRunway.Left, transform.position);
        bool isCentreEmpty = !carManager.HasCivilianCarOnTheLine(CurrentRunway.Centre, transform.position);
        bool isRightEmpty = !carManager.HasCivilianCarOnTheLine(CurrentRunway.Right, transform.position);


        CurrentRunway currentRunway = CurrentRunway.Centre;

        if (isCentreEmpty)
        {
            currentRunway = CurrentRunway.Centre;
        }
        else if (isRightEmpty)
        {
            currentRunway = CurrentRunway.Right;
        }
        else if (isLeftEmpty)
        {
            currentRunway = CurrentRunway.Left;
        }
        else
        {
            currentRunway = GetFarestCar();
        }

        return currentRunway;
        //CurrentRunway = currentRunway;
    }

    public virtual void Turn()
    {
        _currentPos = transform.position;
        _currentPos.x = Mathf.Lerp(_currentPos.x, _value, _turningSpeed * Time.deltaTime);

        transform.position = _currentPos;
    }


    private void SwitchEmmiting()
    {
        foreach (var item in _renderers)
        {
            item.emitting = item.emitting == true ? false : true;
        }
    }

    private IEnumerator DisableEmiting()
    {
        yield return Helpers.Helper.GetWait(_timeToEmit);
        SwitchEmmiting();
    }

    protected bool CanTurn(CurrentRunway direction)
    {
        float value = 0;

        switch (direction)
        {
            case CurrentRunway.Left:
                value = _lineSizes.x;
                break;
            case CurrentRunway.Centre:
                value = _lineSizes.y;
                break;
            case CurrentRunway.Right:
                value = _lineSizes.z;
                break;
            default:
                break;
        }

        bool isLeftSide = transform.position.x > value ? true : false;

        Vector3 centreOfBox;

        centreOfBox = isLeftSide ? _leftBox.position : _rightBox.position;

        Collider[] hitCollider = Physics.OverlapBox(centreOfBox, _boxSizeLR, Quaternion.identity, _turnMask);
        return hitCollider.Length != 0 ? false : true;
    }

    protected Car GetCarInFront()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + Vector3.back * _boxOffset, _boxSizeForward/*_boxSizeLR*/, Quaternion.identity, _carInFrontMask);

        Collider nearestCollider = null;
        float nearestDistance = Mathf.Infinity;
        for (int i = 0; i < colliders.Length; i++)
        {
            float distance = Vector3.Distance(colliders[i].transform.position, transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCollider = colliders[i];
            }
        }
        return nearestCollider ? nearestCollider.GetComponent<Car>() : null;
    }

    [ContextMenu("Fill")]
    private void FillData()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxSizeLR = new Vector3(_boxCollider.size.x * _boxWidth, _boxCollider.size.y, _boxCollider.size.z * _boxLength);
        _boxSizeForward = new Vector3(_boxCollider.size.x * _forwardBoxSize.x, _boxCollider.size.y, _boxCollider.size.z * _forwardBoxSize.y);
    }


    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawCube(_leftBox.position, _boxSizeLR);
        Gizmos.DrawCube(_rightBox.position, _boxSizeLR);
        Gizmos.DrawCube(transform.position + Vector3.back * _boxOffset, _boxSizeForward);

    }
}
