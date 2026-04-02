using UnityEngine;

[System.Serializable]
public enum TileType
{
    Blue = 0,
    Orange = 1,
    Green = 2,
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
