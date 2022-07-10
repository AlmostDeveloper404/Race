using UnityEngine;
using System.Collections;

public class DestroedCar : MonoBehaviour
{
    [SerializeField] private float _timeToDisable;

    private void OnEnable()
    {
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return Helpers.Helper.GetWait(_timeToDisable);
        gameObject.SetActive(false);
    }
}
