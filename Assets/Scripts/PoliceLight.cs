using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PoliceLight : MonoBehaviour
{
    [SerializeField] private Material _emissionMat;
    [SerializeField] private Material _basicMat;

    [SerializeField] private MeshRenderer _leftLight;
    [SerializeField] private MeshRenderer _rightLight;

    [SerializeField] private float _interval;

    private void OnEnable()
    {
        StartCoroutine(LightSwitching());
    }

    private IEnumerator LightSwitching()
    {
        _leftLight.sharedMaterial =_leftLight.sharedMaterial == _basicMat ? _emissionMat : _basicMat;
        _rightLight.sharedMaterial = _rightLight.sharedMaterial == _basicMat ? _emissionMat : _basicMat;

        yield return Helpers.Helper.GetWait(_interval);

        StartCoroutine(LightSwitching());
    }

    private void OnDisable()
    {
        StopCoroutine(LightSwitching());
    }
}
