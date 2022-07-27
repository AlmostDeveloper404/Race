using UnityEngine;

public class PlayerCar : ActiveCars
{

    [SerializeField] private CarShooting _carShooting;

    public override void Start()
    {
        base.Start();
        SoundManager.Instance.ResumeSounds();
        CurrentRunway = CurrentRunway.Centre;
    }


    public override void CheckRoadTrafic()
    {
        base.CheckRoadTrafic();

        _timer += Time.deltaTime;
        if (_timer > _callsInSec)
        {
            _timer = 0;
            if (GetCarInFront())
            {
                ChangeRunway();
            }
        }
    }

    private void ChangeRunway()
    {
        CurrentRunway currentRunway = GetRunwayWithCivilianTraffic();
        if (CanTurn(currentRunway))
        {
            ApplyTurn(currentRunway);
        }

    }

    public override void OnTriggerEnter(Collider other)
    {
        PoliceCar policeCar = other.GetComponent<PoliceCar>();

        if (policeCar)
        {
            policeCar.StopPoliceCar();

            _carShooting.StopShooting();
            _animator.SetTrigger(Animations.Death);
            SoundManager.Instance.PlaySound(_deathSound);
        }
    }

    public override void Death()
    {
        SoundManager.Instance.PauseSounds();

        _carShooting.enabled = false;
        Time.timeScale = 0;
        GameManager.Instance.ChangeGameState(GameState.GameOver);
        StopAllCoroutines();
    }

    public override void Turn()
    {
        base.Turn();
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
