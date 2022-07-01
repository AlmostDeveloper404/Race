using UnityEngine;

public class EndTrackCollider : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent</*PlayerCar*/PlayerCar>())
        {
            RoadGenerator.Instance.Generate();
        }
    }
}
