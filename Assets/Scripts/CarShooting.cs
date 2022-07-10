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

    [SerializeField] private GameObject _missile;

    private int _bulletsUsed;
    private float _timer;

    private Bullet[] _allBullets;

    public static Action<float> OnReloading;
    public static Action OnShooted;

    private Animator _animator;

    private void Awake()
    {
        _timer = _intervalBetweenShots;

        _animator = GetComponent<Animator>();

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

            if (_bulletsUsed != _bulletsCapacity)
            {
                _animator.SetTrigger(Animations.IsAttacking);
            }
        }
    }

    private void Shoot()
    {
        _missile.SetActive(false);

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
                bullet.transform.rotation = Quaternion.identity;
                return;
            }
            else
            {
                continue;
            }
        }
    }

    private void EndFastReloading()
    {
        _missile.SetActive(true);
    }

    public void Reload()
    {
        OnReloading?.Invoke(_reloadTime);
        _animator.SetBool(Animations.IsReloading, true);
        StartCoroutine(Reloading());
    }


    private IEnumerator Reloading()
    {
        _bulletsUsed = _bulletsCapacity;
        yield return Helpers.Helper.GetWait(_reloadTime);
        _missile.SetActive(true);
        _animator.SetBool(Animations.IsReloading, false);
        _bulletsUsed = 0;
    }

}
