using UnityEngine;

public class CivilianCar : Car
{
    private PlayerCar _playerCar;

    private float _timer;

    private void OnEnable()
    {
        _playerCar = FindObjectOfType<PlayerCar>();
    }

    private void Update()
    {
        transform.position -= new Vector3(0f, 0f, Speed * Time.deltaTime);

        _timer += Time.deltaTime;
        if (_timer > 2)
        {
            _timer = 0;
            if (transform.position.z < _playerCar.transform.position.z)
            {
                gameObject.SetActive(false);
                CarManager.Instance.RemoveCar(this);
            }
        }
    }
}
