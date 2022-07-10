using UnityEngine;

public class CivilianCar : Car
{
    [SerializeField] private GameObject _destroedCar;

    public override void CheckRoadTrafic()
    {
        base.CheckRoadTrafic();
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
        if (other.CompareTag("Border"))
        {
            base.Death();
            CarManager.Instance.RemoveCar(this, false);
        }

    }

    public override void Death()
    {
        base.Death();
        _destroedCar.SetActive(true);
        _destroedCar.transform.parent = null;
        _destroedCar.transform.position = transform.position;

        _deathParticles.transform.position = transform.position;
        _deathParticles.Play();
        CarManager.Instance.RemoveCar(this, true);
    }

}
