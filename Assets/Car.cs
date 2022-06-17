using UnityEngine;

public enum CurrentRunway { Left, Centre, Right }
public enum CarTypes { Police, Civilian, Player }

public class Car : MonoBehaviour
{
    [SerializeField] protected float Speed;
    public CarTypes CurrentCarType;

    public CurrentRunway CurrentRunway;

    public virtual void Disable()
    {
        gameObject.SetActive(false);
    }



    public virtual void Enable(Transform positionToSpawn, CurrentRunway currentRunway)
    {
        gameObject.SetActive(true);
        CurrentRunway = currentRunway;
        transform.position = positionToSpawn.position;
    }

    public bool IsEnabled() => gameObject.activeSelf;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Border"))
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void Death()
    {
        gameObject.SetActive(false);
    }
}
