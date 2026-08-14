using UnityEngine;

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
