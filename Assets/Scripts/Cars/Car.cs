using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public enum CurrentRunway { Left, Centre, Right }
public enum CarTypes { Police, Civilian, Player }

[RequireComponent(typeof(Animator))]
public class Car : MonoBehaviour
{
    public float Speed;
    public CarTypes CurrentCarType;
    public CurrentRunway CurrentRunway;

    protected Animator _animator;

    //[SerializeField] private float _turningSpeed;

    //[SerializeField] private TrailRenderer[] _renderers;
    //[SerializeField] private float _timeToEmit;

    //[Header("Box")]
    //[SerializeField] protected Transform _rightBox;
    //[SerializeField] protected Transform _leftBox;
    //[SerializeField] protected float _boxOffset;
    //[SerializeField] protected float _boxWidth;
    //[SerializeField] protected float _boxLength;
    //[SerializeField] protected Vector2 _forwardBoxSize;

    [SerializeField] protected Vector3 _lineSizes;

    protected float _value;

    //private Vector3 _currentPos;

    protected float _timer;

    [Header("Clips")]
    [SerializeField] protected AudioClip _deathSound;

    private void Update()
    {
        CheckRoadTrafic();
    }

    public virtual void CheckRoadTrafic()
    {
        transform.position -= new Vector3(0f, 0f, Speed * Time.deltaTime);
    }

    public virtual void Enable(Transform positionToSpawn, CurrentRunway currentRunway, Customization customization)
    {

        gameObject.SetActive(true);
        CurrentRunway = currentRunway;
        transform.position = positionToSpawn.position;
    }
    public virtual void Disable()
    {
        gameObject.SetActive(false);
    }

    public bool IsEnabled() => gameObject.activeSelf;

    public virtual void OnTriggerEnter(Collider other)
    {

    }

    public virtual void Death()
    {
        gameObject.SetActive(false);
    }
}
