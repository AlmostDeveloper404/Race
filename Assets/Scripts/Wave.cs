using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Wave/NewWave")]
public class Wave : ScriptableObject
{
    public CarInfo[] CarInfo;
}

[System.Serializable]
public struct CarInfo
{
    public float SpawnInterval;
    public CarTypes CarType;
    public CurrentRunway CurrentRunway;
}
