using UnityEngine;
using UnityEngine.EventSystems;

public class CarShooting : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private GameObject _bulletPref;
    [SerializeField] private GameObject[] _poolBullets;

    private void Start()
    {
        _poolBullets = new GameObject[10];

        for (int i = 0; i < _poolBullets.Length; i++)
        {
            GameObject newBullet = Instantiate(_bulletPref, _spawnPoint.localPosition, _spawnPoint.localRotation);
            _poolBullets[i] = newBullet;
            newBullet.SetActive(false);
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        for (int i = 0; i < _poolBullets.Length; i++)
        {
            GameObject bullet = _poolBullets[i];
            if (!bullet.activeSelf)
            {
                bullet.SetActive(true);
                bullet.transform.position = _spawnPoint.position;
                bullet.transform.rotation = _spawnPoint.rotation;
                return;
            }
        }
    }
}
