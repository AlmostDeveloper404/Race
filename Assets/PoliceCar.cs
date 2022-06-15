using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceCar : Car
{
    private PlayerCar _playerCar;

    private float _timer;
    private float _timeToCheckForPlayer;

    private void OnEnable()
    {
        _playerCar = FindObjectOfType<PlayerCar>();
    }

    private void Update()
    {
        transform.position -= new Vector3(0f, 0f, Speed * Time.deltaTime);

        _timer += Time.deltaTime;
        if (_timer > _timeToCheckForPlayer)
        {
            _timer = 0;
        }
    }

    


}
