using UnityEngine;

public class CivilianCar : Car
{
    private void Update()
    {
        transform.position -= new Vector3(0f, 0f, Speed * Time.deltaTime);
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
        CarManager.Instance.RemoveCar(this);
    }
}
