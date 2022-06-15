using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform _playerCar;

    private Vector3 _cameraPosition;

    private void LateUpdate()
    {
        _cameraPosition = transform.position;
        _cameraPosition = new Vector3(0f, 0f, _playerCar.position.z);
        transform.position = _cameraPosition;
    }
}
