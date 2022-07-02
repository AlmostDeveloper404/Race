using UnityEngine;
using System.Collections;
using System;

public class CarShooting : MonoBehaviour
{
    [Header("Bullets")]
    [SerializeField] private int _bulletsAmount;
    [SerializeField] private Bullet _bulletPref;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _bulletsCapacity;
    [SerializeField] private float _intervalBetweenShots;
    [SerializeField] private float _reloadTime;

    private int _bulletsUsed;
    private float _timer;

    private Bullet[] _allBullets;

    public static Action<float> OnReloading;
    public static Action OnShooted;

    private void Awake()
    {
        _timer = _intervalBetweenShots;

        _allBullets = new Bullet[_bulletsAmount];

        for (int i = 0; i < _bulletsAmount; i++)
        {
            Bullet bullet = Instantiate(_bulletPref);
            _allBullets[i] = bullet;
            bullet.Disable();
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && !Helpers.Helper.IsOverUI() && _timer > _intervalBetweenShots)
        {
            _timer = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        for (int i = 0; i < _allBullets.Length; i++)
        {
            Bullet bullet = _allBullets[i];

            if (!bullet.isActiveAndEnabled && _bulletsUsed < _bulletsCapacity)
            {
                _bulletsUsed++;
                if (_bulletsUsed == _bulletsCapacity)
                {
                    Reload();
                }

                OnShooted?.Invoke();

                bullet.gameObject.SetActive(true);
                bullet.transform.position = _spawnPoint.transform.position;
                bullet.transform.rotation = _spawnPoint.transform.rotation;
                return;
            }
            else
            {
                continue;
            }
        }
    }

    public void Reload()
    {
        OnReloading?.Invoke(_reloadTime);
        StartCoroutine(Reloading());
    }


    private IEnumerator Reloading()
    {
        _bulletsUsed = _bulletsCapacity;
        yield return new WaitForSeconds(_reloadTime);
        _bulletsUsed = 0;
    }

}
