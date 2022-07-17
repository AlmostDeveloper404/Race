using UnityEngine;

public class PlayerCar : Car
{
    public override void Start()
    {
        base.Start();
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

    public override void ChangeRunway()
    {
        base.ChangeRunway();
        if (CanTurn(CurrentRunway))
        {
            ApplyTurn(CurrentRunway);
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
        SoundManager.Instance.PlaySound(_deathSound);
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
