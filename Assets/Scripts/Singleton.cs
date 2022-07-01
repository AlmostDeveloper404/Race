using UnityEngine;


public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;


    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject newInstance = new GameObject("SINGLETON" + typeof(T));
                    _instance = newInstance.AddComponent<T>();

                }
            }
            return _instance;
        }
    }
}

