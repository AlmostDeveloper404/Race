using UnityEngine;
using System.Collections.Generic;

public enum Customization { BasicCar, SmallCar, Flatbed, GarbageTruck, DeliveryTruck, Muscle, Van, Convertable }

public class CivilianCar : PassiveCar
{
    [SerializeField] private GameObject _destroedCar;

    private List<VehicleType> vehicleTypes = new List<VehicleType>();
    [SerializeField] private Vector3 _offset;

    private void Awake()
    {
        LoadAllVehicles();
    }

    private void LoadAllVehicles()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            VehicleType vehicleType = transform.GetChild(i).GetComponent<VehicleType>();
            if (vehicleType)
            {
                vehicleTypes.Add(vehicleType);
            }
        }
    }

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
            SoundManager.Instance.PlaySound(_deathSound);
            bullet.Disable();
            Death();
        }
        if (other.CompareTag("Border"))
        {
            base.Death();
            CarManager.Instance.RemoveCar(this, false);
        }

    }

    public override void Enable(Transform positionToSpawn, CurrentRunway currentRunway, Customization customization)
    {
        base.Enable(positionToSpawn, currentRunway, customization);

        for (int i = 0; i < vehicleTypes.Count; i++)
        {
            VehicleType vCustomization = vehicleTypes[i];

            if (vCustomization.Customization == customization)
            {
                _destroedCar = vCustomization.DestroedCar;
                vCustomization.gameObject.SetActive(true);
            }
            else
            {
                vCustomization.gameObject.SetActive(false);
            }
        }
    }

    public override void Death()
    {
        base.Death();
        _destroedCar.SetActive(true);
        _destroedCar.transform.parent = null;
        _destroedCar.transform.position = transform.position + _offset;


        CarManager.Instance.RemoveCar(this, true);
    }
}
