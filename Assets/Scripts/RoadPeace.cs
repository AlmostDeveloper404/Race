using UnityEngine;

public class RoadPeace : MonoBehaviour
{

    public void Setup(float distance)
    {
        gameObject.SetActive(true);
        transform.position = new Vector3(0f, 0f, distance);
    }
}
