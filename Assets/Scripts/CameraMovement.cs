using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _playerCar;

    private Vector3 _cameraPosition;

    private bool _isGameStarted = false;

    private void OnEnable()
    {
        GameManager.OnGameStarted += StartFollow;
    }

    private void LateUpdate()
    {
        if (_isGameStarted)
        {
            _cameraPosition = transform.position;
            _cameraPosition = new Vector3(_playerCar.position.x, 0f, _playerCar.position.z);
            transform.position = Vector3.Lerp(transform.position, _cameraPosition, Time.deltaTime * 12f);
        }
    }

    private void StartFollow()
    {
        _isGameStarted = true;
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= StartFollow;
    }
}
