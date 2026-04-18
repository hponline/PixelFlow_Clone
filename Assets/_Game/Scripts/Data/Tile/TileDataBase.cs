using UnityEngine;

[System.Serializable]
public enum TileType
{
    None = 0,
    Blue = 1,
    Orange = 2,
    Green = 3,
    Red = 4,
    White = 5,
    Brown = 6
}

public class TileDatabase : MonoBehaviour
{
    public TileDataSO[] tiles;

    public GameObject GetPrefab(TileType type)
    {
        foreach (var t in tiles)
        {
            if (t.type == type)
                return t.prefab;
        }
        return null;
    }
}
