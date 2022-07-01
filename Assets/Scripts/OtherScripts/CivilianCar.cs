using UnityEngine;

public class CivilianCar : Car
{
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
            CarManager.Instance.RemoveCar(this,false);
        }

    }

    public override void Death()
    {
        base.Death();
        CarManager.Instance.RemoveCar(this, true);
    }

}
