using UnityEngine;

public class RoadGenerator : Singleton<RoadGenerator>
{
    [SerializeField] private RoadPeace[] _roadPeaces;

    [SerializeField] private float _roadSize = 80f;

    private float _distance;
    private int _peaceIndex = 0;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        _roadPeaces[_peaceIndex].Setup(_distance);

        _distance -= _roadSize;
        _peaceIndex++;
        if (_peaceIndex > _roadPeaces.Length - 1)
        {
            _peaceIndex = 0;
        }
    }
}
